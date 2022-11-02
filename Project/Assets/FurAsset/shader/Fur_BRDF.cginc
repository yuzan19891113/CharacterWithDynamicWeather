#ifndef Fur_BRDF_INCLUDED
    #define Fur_BRDF_INCLUDED

    #include "UnityCG.cginc"
    #include "UnityStandardConfig.cginc"
    #include "UnityLightingCommon.cginc"
    #if 1
        
        inline float FabricD (float NdotH, float roughness)
        {
            return 0.96 * pow(1 - NdotH, 2) + 0.057; 
        }

        inline half FabricScatterFresnelLerp(half nv, half scale)
        {
            half t0 = Pow4 (1 - nv); 
            half t1 = 0.4 * (1 - nv);
            return (t1 - t0) * scale + t0;
        }
        inline half AnisoDCore(half smoothness, half3 normalWorld, half3 tangentWorld, half3 halfDir, half nh, half D, half gloss, half spec, half mask)
        {
            half3 Y = cross(normalWorld, tangentWorld);
            half RoughnessX = SmoothnessToRoughness(saturate(smoothness * gloss));
            RoughnessX += !RoughnessX * 1e-4f;
            half mx = RoughnessX * RoughnessX;
            half XdotH = dot(tangentWorld, halfDir);
            half YdotH = dot(Y, halfDir);
            half d = XdotH * XdotH / (mx * mx) + YdotH * YdotH + nh * nh;
            d += !d * 1e-4f;
            half Da = 1 / (UNITY_PI * mx * d * d);
            D = lerp(Da, D, mask);
            
            D *= lerp(spec, 1, mask);

            return D;
        }

        inline half3 JitterTangent(half3 T, half3 N, float shift)
        {
            half3 shiftedT = T + shift * N;
            return normalize(shiftedT);
        }

        inline half AnisoD(half smoothness, half3 normalWorld, half3 tangentWorld, half3 halfDir, half nh, half D, half2 anisoCtrl)
        {
            half jitter = anisoCtrl.r;
            half mask = anisoCtrl.g;//mask used to control D(normal distribution function)

            half3 tangentWorld1 = JitterTangent(tangentWorld, normalWorld, 0 + _TangentShift1);
            half AnisoDLow = AnisoDCore(smoothness, normalWorld, tangentWorld1, halfDir, nh, D, _AnisoGloss1, _AnisoSpec1, mask);
            
            half3 tangentWorld2 = JitterTangent(tangentWorld, normalWorld, jitter + _TangentShift2);
            half AnisoDHigh = AnisoDCore(smoothness, normalWorld, tangentWorld2, halfDir, nh, D, _AnisoGloss2, _AnisoSpec2, mask);        

            return AnisoDLow + AnisoDHigh;//return the two layer D
        }
        half4 FUR_BRDF_PBS (half3 diffColor, half3 specColor, half oneMinusReflectivity, half smoothness,
        float3 normal, float3 viewDir,
        UnityLight light, UnityIndirect gi,float3 tangentWorld = float3(0, 0, 1))
        {
            float perceptualRoughness = SmoothnessToPerceptualRoughness (smoothness);
            float3 halfDir = Unity_SafeNormalize (float3(light.dir) + viewDir);
            
            //normal is useless,now we use tangent to calculate the fake normal

            // half nv = dot(normal, viewDir);  
            // half nl = saturate(dot(normal, light.dir));
            // float nh = saturate(dot(normal, halfDir));
            half nv = sqrt(1-dot(tangentWorld, viewDir)*dot(tangentWorld, viewDir));  
            half nl = sqrt(1-dot(tangentWorld, light.dir)*dot(tangentWorld, light.dir));
            float nh = sqrt(1-dot(tangentWorld, halfDir)*dot(tangentWorld, halfDir));

            half lv = saturate(dot(light.dir, viewDir));
            half lh = saturate(dot(light.dir, halfDir));

            // Diffuse term
            half diffuseTerm = DisneyDiffuse(nv, nl, lh, perceptualRoughness) * nl;

            // Specular term
            // HACK: theoretically we should divide diffuseTerm by Pi and not multiply specularTerm!
            // BUT 1) that will make shader look significantly darker than Legacy ones
            // and 2) on engine side "Non-important" lights have to be divided by Pi too in cases when they are injected into ambient SH
            float roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
            roughness = max(roughness, 0.002);
            half V = SmithJointGGXVisibilityTerm (nl, nv, roughness);
            float D = GGXTerm (nh, roughness);

            //Aniso Specular

            //because we have used tangent to calculate anisotropic,so we don't use this D

            //D = AnisoD(smoothness, normal, tangentWorld, halfDir, nh, D, anisoCtrl);

            float VxD= roughness > 0.99 ? 1 * FabricD (nh, roughness) : V * D;

            

            //Whether scatter?
            half specularTerm = VxD * UNITY_PI;

            #ifdef UNITY_COLORSPACE_GAMMA
                specularTerm = sqrt(max(1e-4h, specularTerm));
            #endif

            // specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
            specularTerm = max(0, specularTerm * nl);
            #if defined(_SPECULARHIGHLIGHTS_OFF)
                specularTerm = 0.0;
            #endif

            // surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(roughness^2+1)
            half surfaceReduction;
            #ifdef UNITY_COLORSPACE_GAMMA
                surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;      // 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
            #else
                surfaceReduction = 1.0 / (roughness*roughness + 1.0);           // fade \in [0.5;1]
            #endif

            // To provide true Lambert lighting, we need to be able to kill specular completely.
            specularTerm *= any(specColor) ? 1.0 : 0.0;

            half grazingTerm = saturate(smoothness + (1-oneMinusReflectivity));

            //Changed Color
            // half3 color =   diffColor * (gi.diffuse + light.color * diffuseTerm)
            // + specularTerm * light.color * FresnelTerm (specColor, lh)
            // + _FabricScatterColor * (nv*0.5 + 0.5) * FabricScatterFresnelLerp(nv, _FabricScatterScale);

            //Standard Color

            half3 color =   diffColor * (gi.diffuse + light.color * diffuseTerm)
            + 1*specularTerm * light.color * FresnelTerm (specColor, lh)
            + surfaceReduction * gi.specular * FresnelLerp (specColor, grazingTerm, nv);

            return half4(color, 1);
        }
    #endif
    // Include deprecated function
    // #define INCLUDE_UNITY_STANDARD_BRDF_DEPRECATED
    // #include "UnityDeprecated.cginc"
    // #undef INCLUDE_UNITY_STANDARD_BRDF_DEPRECATED
#endif
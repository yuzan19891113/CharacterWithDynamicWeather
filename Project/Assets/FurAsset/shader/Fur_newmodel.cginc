// Upgrade NOTE: replaced 'defined _FUR' with 'defined (_FUR)'
// Upgrade NOTE: replaced 'defined _HAIR' with 'defined (_HAIR)'

#ifndef FUR_NEWMODEL
    #define FUR_NEWMODEL

    inline half3 MJitterTangent(half3 oriTangent,half3 shift,half jitter){
        return normalize(oriTangent+shift*jitter);
    }

    half CalSpec(half3 viewdir,half3 lightdir,half3 tangent,half specExponent){
        half3 h=normalize(normalize(viewdir)+normalize(lightdir));
        half tdoth=dot(normalize(tangent),h);
        half term=sqrt(1-tdoth*tdoth);

        // half atten=pow(smoothstep(0,-1,tdoth),1);
        half atten=pow(smoothstep(-1,0,tdoth),1);
        //half atten=tdoth;
        return atten*pow(term,specExponent);
    }

    half4 KajiyaFurShading(half3 diffcolor,half3 speccolor,half smothness,half3 viewdir,half3 tangent,half3 normal,
    UnityLight light,UnityIndirect gi,half2 anisoCtrl=half2(1,1)){
        half proughness=SmoothnessToPerceptualRoughness(smothness);
        //diffuse color
        half diffuseterm=saturate(dot(light.dir,normal));
        half3 diffuse=diffcolor*(gi.diffuse+diffuseterm*light.color);

        //specular1
        half moveoffset=anisoCtrl.r;
        half mask=anisoCtrl.g;
        half3 t1=MJitterTangent(tangent,normal,_TangentShift1);
        #if (defined (_HAIR))||(defined (_FUR))
            half3 spec1=_SpecColor1*CalSpec(viewdir,light.dir,t1,_AnisoSpec1);
        #else
            half3 spec1=speccolor*CalSpec(viewdir,light.dir,t1,_AnisoSpec1);
        #endif
        //SPECULAR2
        half3 t2=MJitterTangent(tangent,normal,moveoffset+_TangentShift2);
        #if (defined (_HAIR))||(defined (_FUR))
            half3 spec2=_SpecColor2*CalSpec(viewdir,light.dir,t2,_AnisoSpec2);
        #else
            half3 spec2=speccolor*CalSpec(viewdir,light.dir,t2,_AnisoSpec2);
        #endif
        return half4(diffuse+spec1+spec2,1.0);
    }
#endif
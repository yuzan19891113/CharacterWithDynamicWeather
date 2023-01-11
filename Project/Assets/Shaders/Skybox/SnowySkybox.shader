
Shader "Custom/SnowSkybox"
{

    Properties
    {
        _TimeMapping("Time Mapping", Range(-1, 1)) = 0//这个参数是脚本传参过来的，控制全部的变化过程，
        
        [Header(Sky Settings)]
        _NoiseTexture("Star Noise Texture", 2D) = "black"{}

        //雪天
        _DaySkyColor("Day Sky Color", Color) = (0.64, 0.87, 0.93, 1)//A4DFEE
         _NightSkyColor("Night Sky Color", Color) = (0.06, 0.09, 0.12, 1)//0F171F
       


         [Header(Horizon Settings)]
         _HorizonStrength("Horizon Strength", Range(100, 200)) = 100
         _HorizonContract("Horizon Contract", Range(10, 100)) = 100
         _HorizonBias("Horizon Bias", Range(-1, 1)) = 0

        //雪天
         _DayHorizonColor("Day Horizon Color", Color) = (0.97, 0.97, 0.97, 1)//F8F8F8
         _SunsetHorizonColor("Sunset Horizon Color", Color) = (0.95, 0.75, 0.59, 1)//F1C096
         
         _NightHorizonColor("Night Horizon Color", Color) = (0.04, 0.02, 0.1, 1)
         _GroundLineColor("Ground Line Color", Color) = (1, 1, 1, 1)

         [Header(Sun and Moon Settings)]
         //雪天
         _SunColor("Sun Color", Color) = (0.97, 0.81, 0.51, 1)//F8CE81
         _SunRadius("Sun Radius", Range(1, 10)) = 1
         _SunIntensity("Sun Intensity", Range(1, 10)) = 3.9
         _MoonColor("Moon Color", Color) = (0.5, 0.5, 0.5, 1)//808080
         _MoonRadius("Moon Radius", Range(1, 10)) = 1.5
         _MoonIntensity("Moon Intensity", Range(1, 10)) = 2
         _MoonMask("Moon Mask", Range(-1, 1)) = 0.15

         [Header(Stars Settings)]
         _StarsBrightness("Stars Brightness", Range(1, 1000)) = 100
         _StarsHeight("Stars Height", Range(0, 1)) = 0.2
         _StarsScale("Stars Scale", Range(1, 5)) = 2.5


         [Header(Testing Properties)]

         _ShiftHorizonColorRampBias("Shift Horizon Color Ramp Bias", Range(-0.5, 0.5)) = -0.2
         _ShiftHorizonIntensityRamp("Shift Horizon Intensity Ramp", Range(0, 1)) = 0.8
         _SunsetHorizonColorRamp("Day Horizon Color Ramp", Range(0, 1)) = 1
         _ShiftSunAndMoonColorRamp("Shift Sun And Moon Color Ramp", Range(0, 1)) = 0.75
    }
        SubShader
         {
             Tags
             {
                 //"RenderPipeline" = "UniversalPipeline"
                 "RenderType" = "Opaque"
             }



             Pass
             {
                 CGPROGRAM

                 #pragma vertex vert
                 #pragma fragment frag
                 #include "UnityCG.cginc"

                 //#pragma shader_feature MIRROR
                 //#pragma shader_feature ADDCLOUD

                 struct Attributes
                 {
                     float4 positionOS : POSITION;
                     float3 uv : TEXCOORD0;
                 };

                 struct Varyings
                 {
                     float4 positionCS : SV_POSITION;
                     float3 positionWS : TEXCOORD0;
                     float3 uv : TEXCOORD1;
                 };

                 //debug
                 float _TimeMapping;

                 sampler2D _NoiseTexture;
                 fixed4 _NoiseTexture_ST;
                 fixed4 _DaySkyColor;
                 fixed4 _NightSkyColor;

                 int _HorizonStrength;
                 int _HorizonContract;
                 fixed _HorizonBias;

                 fixed4 _DayHorizonColor;
                 fixed4 _SunsetHorizonColor;
                 fixed4 _NightHorizonColor;
                 fixed4 _GroundLineColor;

                 fixed4 _SunColor;
                 fixed _SunRadius;
                 fixed _SunIntensity;

                 fixed4 _MoonColor;
                 fixed _MoonRadius;
                 fixed _MoonIntensity;
                 fixed _MoonMask;

                 fixed _StarsBrightness;
                 fixed _StarsHeight;
                 fixed _StarsScale;

                 fixed _ShiftHorizonColorRampBias;
                 fixed _ShiftHorizonIntensityRamp;
                 fixed _SunsetHorizonColorRamp;
                 fixed _ShiftSunAndMoonColorRamp;

                 Varyings vert(Attributes v)
                 {
                     Varyings o;
                     o.positionCS = UnityObjectToClipPos(v.positionOS);
                     o.positionWS = mul(unity_ObjectToWorld, v.positionOS);
                     o.uv = v.uv;
                     return o;
                 }

                 fixed4 frag(Varyings i) : SV_Target
                 {
                     fixed3 lightDirection = normalize(_WorldSpaceLightPos0);
                     fixed4 noise = 0.5 * tex2D(_NoiseTexture, (i.uv.xz * 5 - fixed2(_TimeMapping * 0.25, 0)) / _StarsScale) + 0.5 * tex2D(_NoiseTexture, (i.uv.xz * 4 - fixed2(0, _TimeMapping * 0.25)) / _StarsScale);

                     fixed yFactor = min(i.uv.y + _HorizonBias * 0.5, 1);
                     fixed3 stars = pow(noise.rgb, 50) * _StarsBrightness * saturate(yFactor - _StarsHeight) * pow(abs(_TimeMapping), 0.75);
                     fixed horizonControlFactor = pow(1 - yFactor, _HorizonContract / min(abs(_TimeMapping * 10) + 0.5 - _ShiftHorizonColorRampBias, 1)) * saturate(yFactor) * _HorizonStrength;//控制地平线区域的参数计算

                     _SunsetHorizonColor += fixed4((_DayHorizonColor.rgb - _SunsetHorizonColor.rgb) * pow(_SunsetHorizonColorRamp, 5) * saturate(_TimeMapping), 0);//用于控制从日出到白天到日落的地平线颜色变化

                     fixed sunDistance = saturate(1.5 - distance(i.uv.xyz, lightDirection));
                     fixed moonDistance;

                     fixed3 dayHorizonColor = _SunColor.rgb * horizonControlFactor * (0.5 + _ShiftHorizonColorRampBias + max(min(_TimeMapping * 10, (0.5 - _ShiftHorizonColorRampBias)), -0.5 - _ShiftHorizonColorRampBias)) * sunDistance;
                     dayHorizonColor += _SunsetHorizonColor.rgb * horizonControlFactor * (0.5 + _ShiftHorizonColorRampBias + max(min(_TimeMapping * 10, (0.5 - _ShiftHorizonColorRampBias)), -0.5 - _ShiftHorizonColorRampBias));
                     fixed3 nightHorizonColor = _NightHorizonColor.rgb * horizonControlFactor * (0.5 - _ShiftHorizonColorRampBias - max(min(_TimeMapping * 10, 0.5 + _ShiftHorizonColorRampBias), -0.5 + _ShiftHorizonColorRampBias));
                     fixed3 horizionColor = dayHorizonColor + nightHorizonColor;//昼夜交替时昼夜地平线混色过度
                     horizionColor *= min(pow(_TimeMapping * 20, 4), _ShiftHorizonIntensityRamp) + (1 - _ShiftHorizonIntensityRamp);//昼夜交替时地平线颜色淡化过度
                     fixed3 dayColor = (_DaySkyColor.rgb) * smoothstep(0, 0.4, saturate(dot(normalize(lightDirection), fixed3(0, 1, 0)))) * ceil(yFactor);
                     fixed3 nightColor = (_NightSkyColor.rgb) * smoothstep(0, 0.4, saturate(dot(normalize(lightDirection), fixed3(0, 1, 0)))) * ceil(yFactor);
                     nightColor += stars;

                     fixed3 groundColor = _GroundLineColor.rgb * abs(_TimeMapping) * (ceil(_TimeMapping) * (_DaySkyColor.r + _DaySkyColor.g + _DaySkyColor.b) / 3 + ceil(-_TimeMapping) * (_NightSkyColor.r + _NightSkyColor.g + _NightSkyColor.b) / 3) * ceil(-yFactor);

                     fixed3 sunColor = _SunColor.rgb * 100 * _SunIntensity * max((1 - distance(i.uv.xyz, lightDirection) / (0.01 * _SunRadius)), 0) * pow(saturate(yFactor), _ShiftSunAndMoonColorRamp);
                     fixed3 moonColor = _MoonColor.rgb * 10 * _MoonIntensity * saturate((1 - distance(i.uv.xyz, -lightDirection) / (0.025 * _MoonRadius)) * 50) * pow(saturate(yFactor), _ShiftSunAndMoonColorRamp);
                     fixed3 moonMask = -_MoonColor.rgb * 10 * _MoonIntensity * saturate((1 - distance(i.uv.xyz + fixed3(0.1 * _MoonMask, 0.1 * _MoonMask, 0.1 * _MoonMask), -lightDirection) / (0.025 * _MoonRadius)) * 50) * pow(saturate(yFactor), _ShiftSunAndMoonColorRamp);
                     moonColor += moonMask;
                     moonColor = max(moonColor, 0);

                     sunDistance = saturate(1 - distance(i.uv.xyz, lightDirection));
                     moonDistance = saturate(1 - distance(i.uv.xyz, -lightDirection));

                     sunColor += _SunColor.rgb * pow(sunDistance, 2 / saturate(_TimeMapping * 10)) * ceil(yFactor);
                     moonColor += _MoonColor.rgb * pow(moonDistance, 5 / saturate(-_TimeMapping * 10)) * 0.1 * ceil(yFactor);

                     fixed3 color = dayColor * ceil(_TimeMapping) + nightColor * ceil(-_TimeMapping) + horizionColor + sunColor + moonColor + groundColor;

                     return fixed4(color, 1);
                 }

                 ENDCG
             }
         }
}




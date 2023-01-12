Shader "Custom/WetFloor"
{
    Properties
    {
        _RainColor("Water Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Normal("Normal",2D) = "bump"{}
        [NoScaleOffset]_RippleTex("Ripple Tex", 2D) = "white" {}
        [NoScaleOffset]_MaskTex("Mask Tex", 2D) = "white" {}
        _RippleScale("Ripple Scale",Range(0,7)) = 3.2
        _Glossiness("Smoothness", Range(0,1)) = 1
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Ripple("Ripple Strength", Range(0,5)) = 1.7
        _WaterRange("Water Range", Range(0,1)) = 0.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows
            #pragma target 3.0

            sampler2D _MainTex;
            sampler2D _Normal;
            sampler2D _RippleTex;
            //R通道代表了涟漪生成的范围，并且带有淡出的效果，
            //GB两个通道是高度：法线图的效果，
            //A通道存储时间差，从白到黑不同的颜色值代表不同的时间。
            sampler2D _MaskTex;

            struct Input
            {
                half2 uv_MainTex;
                half2 uv_Normal;
                half2 uv_FlowMap;
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _RainColor;
            half _RippleScale;
            half _Ripple;
            half _WaterRange;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

            half3 ComputeRipple(half2 uv,half t)
            {
                //波纹贴图采样，并把采样的高度值扩展到-1到1
                half4 ripple = tex2D(_RippleTex,uv);
                ripple.gb = ripple.gb * 2 - 1;
                //获取波纹的时间,从A通道获取不同的波纹时间,
                //frac返回输入值的小数部分。
                half dropFrac = frac(ripple.a + t);
                //把时间限制在R通道内,(dropFrac-1+ripple.r<0时，计算final时0*UNITY_PI)
                half timeFrac = dropFrac - 1 + ripple.r;
                //做淡出处理
                half dropFactor = 1 - saturate(dropFrac);
                //计算最终的高度，用一个sin计算出随时间的振幅，
                half final = dropFactor * sin(clamp(timeFrac * 9,0,4) * UNITY_PI);
                return half3(ripple.gb * final,1);
                //return half3(ripple.gb, 1);
            }

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                half3 ripple = ComputeRipple(IN.uv_MainTex * _RippleScale,_Time.y) * _Ripple;
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                half3 normal = UnpackNormal(tex2D(_Normal,IN.uv_MainTex));
                fixed mask = tex2D(_MaskTex,IN.uv_Normal).r;

                half rainMask = pow(saturate(lerp(-0.6,3,_WaterRange) - (1 - mask) * 2),8);

                fixed3 waterColor = _RainColor.rgb * c.rgb;
                o.Albedo = lerp(c.rgb,waterColor,rainMask);
                o.Normal = normalize(lerp(normal,half3(0,0,1),rainMask) + ripple * rainMask);
                o.Metallic = lerp(_Metallic,0.5,rainMask);
                o.Smoothness = lerp(_Glossiness,1,rainMask);
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}


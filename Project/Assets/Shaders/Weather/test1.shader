﻿Shader "Custom/wet"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Normal("NormalMap",2D) = "bump"{}
		_NormalScale("NormalScale",Range(0,5)) = 1
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_WetColor("WetColor",Color) = (1,1,1,1)
		_WetMap("WetMap",2D) = "white"{}
		_WetGlossiness("Smoothness", Range(0,1)) = 0.5
		_WetMetallic("Metallic", Range(0,1)) = 0.0
		_Wetness("Wetness",Range(0,1)) = 0.0
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
		sampler2D _WetMap;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_Normal;
			float2 uv_WetMap;
		};

		half _Glossiness;
		half _Metallic;
		half _WetGlossiness;
		half _WetMetallic;
		fixed4 _Color;
		half _NormalScale;
		fixed4 _WetColor;
		half _Wetness;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed wetness = tex2D(_WetMap, IN.uv_WetMap).r * _Wetness;
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * lerp(_Color, _WetColor, wetness);
			o.Albedo = c.rgb;
			o.Normal = lerp(UnpackScaleNormal(tex2D(_Normal, IN.uv_Normal), _NormalScale), half3(0, 0, 1), wetness);
			o.Metallic = lerp(_Metallic, _WetMetallic, wetness);
			o.Smoothness = lerp(_Glossiness, _WetGlossiness, wetness);
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}


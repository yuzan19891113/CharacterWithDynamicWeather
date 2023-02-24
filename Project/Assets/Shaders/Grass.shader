Shader "Teafox/GeometryGrass"
{
    Properties
    {
		[Header(Color)]
        _TopColor("Top Color", Color) = (1,1,1,1)
		_BottomColor("Bottom Color", Color) = (1,1,1,1)
		_ShadowColor("Shadow Color", Range(0,1)) = 0.5
		_TranslucentGain("Translucent Gain", Range(0,1)) = 0.5

		[Header(Bending)]
		//草的弯曲相关变量
		_BendRotationRandom("Bend Rotation Random", Range(0,1)) = 0.2
		_BladeForward("Blade Forward Amount", Float) = 0.38
		_BladeCurve("Blade Curvature Amount", Range(1, 4)) = 2

		[Header(Shape)]
		//草的形状相关变量
		_BladeWidth("Blade Width", Range(0,0.05)) = 0.01
		_BladeWidthRandom("Blade Width Random", Range(0,0.5)) = 0.02
		_BladeHeight("Blade Height", Range(0,1)) = 0.5
		_BladeHeightRandom("Blade Height Random", Range(0,1)) = 0.3
		_BladeBottomWidthScale("Blade Bottom Width Scale", Range(0,0.5)) = 0.5
		_BladeMidWidthScale("Blade Mid Width Scale", Range(0,0.5)) = 0.5

		[Header(Tessellation)]
		//地面的细分变量
		_TessellationUniform("Tessellation Uniform", Range(1, 64)) = 1

		[Header(Wind)]
		//与风相关变量
		_WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
		_WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
		_WindStrength("Wind Strength", Float) = 1
    }

	CGPROGRAM

	#pragma target 3.0

	#include "Unity.cginc"
	#include "AutoLight.cginc"
	#include "Tessellation.hlsl"

	#define BLADE_SEGMENTS 3

	//草的弯曲相关变量
	float _BendRotationRandom;
	float _BladeForward;
	float _BladeCurve;

	//草的形状相关变量
	float _BladeWidth;
	float _BladeWidthRandom;
	float _BladeHeight;
	float _BladeHeightRandom;
	float _BladeBottomWidthScale;
	float _BladeMidWidthScale;

	//与风相关变量
	float4 _WindDistortionMap_ST;
	float2 _WindFrequency;
	float _WindStrength;


	sampler2D _WindDistortionMap;

	uniform float _PlayerRadius;
	uniform float _InteractWidth;
	uniform float3 _PlayerPosition;

	

	// Simple noise function, sourced from http://answers.unity.com/answers/624136/view.html
	// Extended discussion on this function can be found at the following link:
	// https://forum.unity.com/threads/am-i-over-complicating-this-random-function.454887/#post-2949326
	// Returns a number in the 0...1 range.
	float rand(float3 co)
	{
		return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
	}

	// Construct a rotation matrix that rotates around the provided axis, sourced from:
	// https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
	float3x3 AngleAxis3x3(float angle, float3 axis)
	{
		float c, s;
		sincos(angle, s, c);

		float t = 1 - c;
		float x = axis.x;
		float y = axis.y;
		float z = axis.z;

		return float3x3(
			t * x * x + c, t * x * y - s * z, t * x * z + s * y,
			t * x * y + s * z, t * y * y + c, t * y * z - s * x,
			t * x * z - s * y, t * y * z + s * x, t * z * z + c
			);
	}

	//vs Already defined in the CustomTessellation.cginc

	struct geometryOutput
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float3 normal : NORMAL;
		half4 shadowCoord : TEXCOORD1;
	};

	geometryOutput VertexOutput(float3 pos, float2 uv, float3 normal)
	{
		geometryOutput o;

		float3 worldPos = TransformObjectToWorld(pos);
		//interacting 
		float dis = distance(_PlayerPosition, worldPos);
		float radius = 1 - saturate(dis / _PlayerRadius);
		float3 sphereDisp = worldPos - _PlayerPosition;
		sphereDisp *= radius;
		worldPos.xz += clamp(sphereDisp.xz * step(0.1f,worldPos.y), -_InteractWidth, _InteractWidth);

		o.pos = TransformWorldToHClip(worldPos);
		o.uv = uv;
		o.normal = TransformObjectToWorldNormal(normal);

		#if SHADOWS_SCREEN
            o.shadowCoord = ComputerScreenPos(o.pos);
        #else
            o.shadowCoord = TransformWorldToShadowCoord(worldPos);
        #endif

		//#if UNITY_PASS_SHADOWCASTER
		//	o.pos = UnityApplyLinearShadowBias(o.pos);
		//#endif
		return o;
	}

	geometryOutput GenerateGrassVertex(float3 vertexPosition, float width, float height, float forward, float2 uv, float3x3 transformMatrix)
	{
		float3 tangentPoint = float3(width, forward ,height);

		float3 tangentNormal = float3(0, -1, forward);
		float3 localNormal = mul(transformMatrix, tangentNormal);
		
		float3 localPosition = vertexPosition + mul(transformMatrix, tangentPoint);
		return VertexOutput(localPosition, uv, localNormal);
	}




	[maxvertexcount(BLADE_SEGMENTS * 2 + 1)]
	void geo(triangle vertexOutput IN[3] : SV_POSITION, inout TriangleStream<geometryOutput> triStream)
	{

		float3 pos = IN[0].vertex;
		//Caculate Matrix from tangent to local
		float3 vNormal = IN[0].normal;
		float4 vTangent = IN[0].tangent;
		float3 vBinoraml = cross(vNormal, vTangent) * vTangent.w;

		//根据罗德里格斯公式，绕z轴（切线空间的up）旋转随机角度(0-360度)
		float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * 2 * PI, float3(0, 0, 1));

		//根据罗德里格斯公式，绕x轴（切线空间）旋转随机角度(0-90*_BendRotationRandom度)
		float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * PI * 0.5, float3(-1, 0, 0));

		float3x3 tangentToLocal = float3x3(
			vTangent.x, vBinoraml.x, vNormal.x,
			vTangent.y, vBinoraml.y, vNormal.y,
			vTangent.z, vBinoraml.z, vNormal.z
		);

		//设置对wind贴图的采样uv
		float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
		float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;
		float3 wind = normalize(float3(windSample.x, windSample.y, 0));
		float3x3 windRotation = AngleAxis3x3(PI * windSample, wind);

		float3x3 transformationMatrixFacing = mul(tangentToLocal, facingRotationMatrix);
		float3x3 transformationMatrix = mul(mul(mul(tangentToLocal, windRotation), facingRotationMatrix), bendRotationMatrix);

		//随机设置草的高度和宽度
		float height = (rand(pos.zyx) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
		float width = (rand(pos.xzy) * 2 - 1)* _BladeWidthRandom + _BladeWidth;
		float forward = rand(pos.yyz) * _BladeForward;


		for(int i = 0; i < BLADE_SEGMENTS; i++)
		{
			float t = i / (float)BLADE_SEGMENTS;
			float segmentHeight = height * t;
			//float segmentWidth = width * (1 - t);
			//使用二次函数让草中间宽两头窄
			//float segmentWidth = width * (i * (BLADE_SEGMENTS - i) * 4 / BLADE_SEGMENTS / BLADE_SEGMENTS * _BladeMidWidthScale) + width * (1 - t) * 0.5 * _BladeBottomWidthScale;
			float segmentWidth = width * (i * (BLADE_SEGMENTS - i) * 4.0 / (BLADE_SEGMENTS * BLADE_SEGMENTS) * _BladeMidWidthScale + (1 - t) * _BladeBottomWidthScale);
			float segmentForward = pow(t, _BladeCurve) * forward;

			float3x3 transformMatrix = i == 0 ? transformationMatrixFacing : transformationMatrix;
			triStream.Append(GenerateGrassVertex(pos, segmentWidth, segmentHeight, segmentForward, float2(0, t), transformMatrix));
			triStream.Append(GenerateGrassVertex(pos, -segmentWidth, segmentHeight, segmentForward, float2(1, t), transformMatrix));
		}
		//The up direction be along the Z axis in tangent space
		triStream.Append(GenerateGrassVertex(pos, 0, height, forward, float2(0.5, 1), transformationMatrix));
	}

	ENDCG

	

    SubShader
    {
		Cull Off

        Pass
        {
			Tags
			{
				"LightMode" = "ForwardBase" 
				"RenderType" = "Opaque"
			}

			CGPROGRAM

			float4 _TopColor;
			float4 _BottomColor;
			float _ShadowColor;
			float _TranslucentGain;

			ENDCG

            CGPROGRAM
            #pragma vertex vert
			#pragma hull hull
			#pragma domain domain
			#pragma geometry geo
            #pragma fragment frag

			#pragma require geometry
			#pragma require geometry tessellation tessHW
			
			#pragma multi_compile _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _SOFT_SHADOW

			float4 frag (geometryOutput i, float facing : VFACE) : SV_Target
            {	
				#if _MAIN_LIGHT_SHADOWS || _MAIN_LIGHT_SHADOWS_CASCADE
					Light light = GetMainLight(i.shadowCoord);
					//return float4(1,1,1,1);
				#else
					Light light = GetMainLight();
					//return float4(0,0,0,1);
				#endif
				float shadow = light.shadowAttenuation;
				float3 normal = facing > 0 ? i.normal : -i.normal;

				float NdotL = saturate(saturate(dot(normal, _MainLightPosition)) + _TranslucentGain);

				float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
				float4 lightIntensity = NdotL * _MainLightColor * shadow + float4(ambient, 1);

				float4 col = lerp(_BottomColor, _TopColor * lightIntensity, i.uv.y);
				return col;

				//With ShadowColor
				float4 shadowColor = _BottomColor * _ShadowColor;
				float4 shadowedCol = lerp(shadowColor, col, shadow);
				return shadowedCol;
            }
            ENDCG
        }

		Pass
		{
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma hull hull
			#pragma domain domain
			#pragma geometry geo
			#pragma fragment frag

			#pragma require geometry
			#pragma require geometry tessellation tessHW
			#pragma multi_compile _MAIN_LIGHT_SHADOW_CASCADE

			//#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#include "Unity.cginc"

			float4 frag(geometryOutput i) : SV_TARGET
			{
				return 0;
			}

			ENDCG
		}
    }
}
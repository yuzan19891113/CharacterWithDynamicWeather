shader "Custom/snow2"{
	Properties{
		_MainTex("MainTex",2D)="white"{}//纹理贴图
		_NormalMap("NormalMap",2D)="bump"{}//法线贴图
		_Snow("Snow",Range(0,1))=0.06//积雪程度，1为完全覆盖
		_SnowColor("SnowColor",Color)=(1,1,1,1)//雪的颜色
		_SnowDir("SnowDir",vector)=(0,1,0)//雪的方向
		_BumpScale("BumpScale",Range(0,10))=1//使用法线贴图的程度，1为完全使用，0为完全不使用。
		_Diffuse("Diffuse",Color)=(1,1,1,1)//漫反射颜色
	}
	SubShader{
		Tags{"RenderType"="Opaque"}
		LOD 100
		pass{
			CGPROGRAM
			#pragma vertex vert 
			#pragma fragment frag 
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NormalMap;
			float4 _NormalMap_ST;
			float _Snow;
			fixed4 _SnowColor;
			float3 _SnowDir;
			float _BumpScale;
			fixed4 _Diffuse;
 
			struct v2f{
				float4 vertex:SV_POSITION;
				float4 uv:TEXCOORD0;//纹理贴图和法线贴图的uv坐标(分别为xy与zw)
				float3 TangentSnowDir:TEXCOORD2;//切线空间下雪的方向
				fixed3 TangentLightDir:TEXCOORD4;//切线空间下光的方向
			};
 
			v2f vert(appdata_tan  v){
				v2f o;
				o.vertex=UnityObjectToClipPos(v.vertex);
				o.uv.xy=TRANSFORM_TEX(v.texcoord,_MainTex);
				o.uv.zw=TRANSFORM_TEX(v.texcoord,_NormalMap);
				
				float3 ObjSnowDir=UnityWorldToObjectDir(_SnowDir);//模型空间下雪的方向
				TANGENT_SPACE_ROTATION;//得到一个矩阵rotation用于把方向从模型空间转换到切线空间
				o.TangentSnowDir=mul(rotation,ObjSnowDir);//切线空间下雪的方向
 
				o.TangentLightDir=normalize(mul(rotation,ObjSpaceLightDir(v.vertex)));
				return o;
 
			}
 
			fixed4 frag(v2f i):SV_TARGET{
				float3 tangentNormal= UnpackNormal(tex2D(_NormalMap,i.uv.zw)).xyz;//从法线贴图得到法线
				tangentNormal=normalize(float3(tangentNormal.xy*_BumpScale,tangentNormal.z));//添加_BumpScale系数对法线的影响
				fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz;//环境光
				fixed3 albedo=tex2D(_MainTex,i.uv.xy);//纹理贴图颜色
				fixed3 diffuse=albedo*_Diffuse*(dot(i.TangentLightDir,tangentNormal)*0.5+0.5);//漫反射
				fixed4 Color=fixed4(ambient+albedo,1);
				//将法线方向与雪的方向接近的部分替换为雪的颜色。lerp为差值。
				if(dot(i.TangentSnowDir,tangentNormal)>lerp(1,-1,_Snow)){
					Color=_SnowColor;
				}
				
				return Color;
			}
			ENDCG
		}
	}
}
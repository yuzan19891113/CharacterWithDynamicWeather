Shader "Custom/SnowCover" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_SnowStrength ("SnowStrength", Range (0.0, 6)) = 0.2
}

SubShader {
	Tags { "RenderType"="Opaque"  }
	LOD 100
	
	Pass {  
			tags{	"LightMode" = "ShadowCaster"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_shadowcaster
			

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {

				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed3 normal : NORMAL;
				UNITY_FOG_COORDS(1)
			};
			//纹理
			sampler2D _MainTex;
			float4 _MainTex_ST;
			//积雪的强度
			float _SnowStrength;
			v2f vert (appdata_t v)
			{
				v2f o;
				//顶点转成世界坐标
				o.vertex = UnityObjectToClipPos(v.vertex);
				//转换UV
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				//法线
				o.normal = v.normal;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
				UNITY_TRANSFER_FOG(o, o.vertex);
				
				return o;
				
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//将法线转换到世界坐标系，方便下面的计算
				float3 wn = UnityObjectToWorldNormal(i.normal);
				//默认一个向上的向量，与法向量做点积，让其取值范围在[0,1]之间。1-点积的数值 ，法向量与向上的向量夹角为0是值为0 ，90度为1.
				float flag = 1 - saturate(dot(float3(0.0,1.0,0.0),wn));
				//使用强度值用于指数运算，将提高数值的变化率。
				flag = saturate(pow(flag,_SnowStrength*3));
				fixed4 col = tex2D(_MainTex, i.texcoord);
				//默认积雪是白色，当然应用于其他，可以修改这个颜色或将其做成变量。
				fixed4 sc = fixed4(1.0,1.0,1.0,1.0);
				//使用lerp做差值运算 ==> sc*(1-flag)+col*flag
				col = lerp(sc,col,flag);
				UNITY_OPAQUE_ALPHA(col.a);
				SHADOW_CASTER_FRAGMENT(o);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(1,1,1,1));
				
				return col;
			}
		ENDCG
	}
}

}

Shader "Fur_Standard"{
    Properties{
        [Enum(PBR,0,KajiyaKay,1)] _RenderChoice("Render Choice",Float)=0
        [Enum(TenLayers,0,TwentyLayers,1)] _LayerChoice("LayerNumber Choice",Int)=0
        _Color("Color", Color) = (1,1,1,1)

        _TipColor("Tip Color",Color)=(1,1,1,1)
        _TipControl("TipControl",Range(0,10))=5
        _TipChoice("TipChoice",Range(0,1))=0
        _TipLocateMap("Tip Locate Map",2D)="black"{}

        _SpecColor1("SpecColor1", Color) = (1,1,1,1)
        _SpecColor2("SpecColor2", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}
        _AO("AO",Range(0,1.0))=0.2
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        [Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DetailMask("Detail Mask", 2D) = "white" {}

        _DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
        _DetailNormalMapScale("Scale", Float) = 1.0
        _DetailNormalMap("Normal Map", 2D) = "bump" {}

        [Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0

        [Space(20)]
        _FabricScatterColor("Fabric Scatter Color", Color) = (1,1,1,1)
        _FabricScatterScale("Fabric Scatter Scale", Range(0, 1)) = 0
        
        //fur characteristic
        [Space(20)]
        _LayerTex("Layer", 2D) = "white" {}
        _FurLength("Fur Length", Range(.0002, 1)) = .25
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5 // how "thick"
        _CutoffEnd("Alpha Cutoff end", Range(0,1)) = 0.0 // how thick they are at the end
        _EdgeFade("Edge Fade", Range(0,1)) = 0.4
        //
        _Gravity("Gravity Direction", Vector) = (0,-1,0,0)
        _RotationSpot("Rotation Spot",Vector)=(0,0,0)
        _GravityStrength("Gravity Strength", Range(-1,1)) = 0.25

        // Blending state
        _Mode("__mode", Float) = 0.0
        _SrcBlend("__src", Float) = 5.0
        _DstBlend("__dst", Float) = 10.0
        _ZWrite("__zw", Float) = 1.0

        [Space(20)]
        //各向异性
        _AnisoMap("Jitter(R) AnisoMask(G) FlowMask(B) EmissionMap(A)", 2D) = "white" {}
        _TangentDir("TangentDir", Range(0, 1)) = 0
        _AnisoGloss1("AnisoGloss1", Range(0, 10)) = 0.7
        _AnisoSpec1("AnisoSpec1", Range(0, 100)) = 3
        _AnisoGloss2("AnisoGloss2", Range(0, 10)) = 0.7
        _AnisoSpec2("AnisoSpec2", Range(0, 100)) = 3
        _TangentShift1("TangentShift1", Range(-1, 1)) = 0
        _TangentShift2("TangentShift2", Range(-1, 1)) = 0
        //fur distribution
        _ForceMap("Forcr Map",2D)="white"{}
        _ForceMapScale("Force Map Scale",Range(0,1))=0.5
        //GUI problem
        _Parallax("Height Scale", Range(0.0, 1.0)) = 0.1
        _ParallaxMap("Height Map", 2D) = "black" {}
        //
        _FurGrowth("Fur Height scale",Range(0,1))=0.5
        _FurGrowthMap("Fur Height Control Map",2D)="white"{}
    }
    SubShader{

        //Tags{ "RenderType" = "Opaque" "PerformanceChecks" = "False" }
        Tags{ "Queue" = "Transparent" "PerformanceChecks" = "False" }
        LOD 300

        CGINCLUDE
        #define UNITY_SETUP_BRDF_INPUT MetallicSetup

        #define _NORMALMAP 1

        #define _FABRIC 1
        #define _FUR 1

        #define _TENLAYERS 1
        
        half3 _FabricScatterColor;
        half  _FabricScatterScale;
        float _AO;

        sampler2D _LayerTex;
        float4 _LayerTex_ST;
        half _FurLength;
        half _CutoffStart;
        half _CutoffEnd;
        half _EdgeFade;
        half3 _Gravity;
        half _GravityStrength;
        float4 _SpecColor1;
        float4 _SpecColor2;
        //Hair tip color
        float4 _TipColor;
        float _TipControl;
        float _TipChoice;
        sampler2D _TipLocateMap;
        float4 _TipLocateMap_ST;

        sampler2D _AnisoMap; 
        half4 _AnisoMap_ST;
        half _TangentDir;
        half _AnisoGloss1;
        half _AnisoGloss2;		
        half _AnisoSpec1;
        half _AnisoSpec2;
        half _TangentShift1;
        half _TangentShift2;

        sampler2D _ForceMap;
        float4 _ForceMap_ST;
        float _ForceMapScale;
        
        float3 _RotationSpot;

        // sampler2D _ParallaxMap;
        // float4 _ParallaxMap_ST;
        // float _Parallax;
        //float _RenderChoice;
        sampler2D _FurGrowthMap;
        float4 _FurGrowthMap_ST;
        float _FurGrowth;

        ENDCG
        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase
            #pragma fragment fragBase
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer1
            #pragma fragment fragBase_FurLayer1
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer2
            #pragma fragment fragBase_FurLayer2
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer3
            #pragma fragment fragBase_FurLayer3
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer4
            #pragma fragment fragBase_FurLayer4
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer5
            #pragma fragment fragBase_FurLayer5
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer6
            #pragma fragment fragBase_FurLayer6
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer7
            #pragma fragment fragBase_FurLayer7
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer8
            #pragma fragment fragBase_FurLayer8
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer9
            #pragma fragment fragBase_FurLayer9
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }

        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer10
            #pragma fragment fragBase_FurLayer10
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }
        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer11
            #pragma fragment fragBase_FurLayer11
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }
        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer12
            #pragma fragment fragBase_FurLayer12
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }
        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer13
            #pragma fragment fragBase_FurLayer13
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }
        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer14
            #pragma fragment fragBase_FurLayer14
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }
        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer15
            #pragma fragment fragBase_FurLayer15
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }
        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer16
            #pragma fragment fragBase_FurLayer16
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }
        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer17
            #pragma fragment fragBase_FurLayer17
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }
        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer18
            #pragma fragment fragBase_FurLayer18
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }
        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer19
            #pragma fragment fragBase_FurLayer19
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }
        Pass
        {
            Name "FORWARD"
            Tags{ "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _TIPLOCATEMAP
            #pragma shader_feature _BRDF _KK
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase_FurLayer20
            #pragma fragment fragBase_FurLayer20
            #include "Fur_UnityStandardCoreForward.cginc"

            ENDCG
        }
        
        

        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass
        {
            Name "ShadowCaster"
            Tags{ "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            //#pragma target 5.0

            // -------------------------------------
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature _PARALLAXMAP
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"

            ENDCG
        }
    }
    Fallback "VertexLit"
    CustomEditor "Fur_StandardShaderGUI"
}
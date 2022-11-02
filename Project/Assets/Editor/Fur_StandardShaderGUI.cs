//using System.Security;
//using System.Globalization;

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

using System;
using UnityEngine;
//using TargetAttributes = UnityEditor.BuildTargetDiscovery.TargetAttributes;

#if UNITY_EDITOR
namespace UnityEditor
{
    internal class Fur_StandardShaderGUI : ShaderGUI
    {
        private enum WorkflowMode
        {
            Specular,
            Metallic,
            Dielectric
        }

        private enum RenderingChoices
        {
            BRDF,
            KajiyaKay
        }

        public enum SmoothnessMapChannel
        {
            SpecularMetallicAlpha,
            AlbedoAlpha,
        }

        private static class Styles
        {
            public static GUIContent uvSetLabel = EditorGUIUtility.TrTextContent("UV Set");

            public static GUIContent albedoText = EditorGUIUtility.TrTextContent("Albedo", "Albedo (RGB) and Transparency (A)");
            public static GUIContent alphaCutoffText = EditorGUIUtility.TrTextContent("Alpha Cutoff", "Threshold for alpha cutoff");
            public static GUIContent specularMapText = EditorGUIUtility.TrTextContent("Specular", "Specular (RGB) and Smoothness (A)");
            public static GUIContent metallicMapText = EditorGUIUtility.TrTextContent("Metallic", "Metallic (R) and Smoothness (A)");
            public static GUIContent smoothnessText = EditorGUIUtility.TrTextContent("Smoothness", "Smoothness value");
            public static GUIContent smoothnessScaleText = EditorGUIUtility.TrTextContent("Smoothness", "Smoothness scale factor");
            public static GUIContent smoothnessMapChannelText = EditorGUIUtility.TrTextContent("Source", "Smoothness texture and channel");
            public static GUIContent highlightsText = EditorGUIUtility.TrTextContent("Specular Highlights", "Specular Highlights");
            public static GUIContent reflectionsText = EditorGUIUtility.TrTextContent("Reflections", "Glossy Reflections");
            public static GUIContent normalMapText = EditorGUIUtility.TrTextContent("Normal Map", "Normal Map");
            public static GUIContent heightMapText = EditorGUIUtility.TrTextContent("Height Map", "Height Map (G)");
            public static GUIContent occlusionText = EditorGUIUtility.TrTextContent("Occlusion", "Occlusion (G)");
            public static GUIContent emissionText = EditorGUIUtility.TrTextContent("Color", "Emission (RGB)");
            public static GUIContent detailMaskText = EditorGUIUtility.TrTextContent("Detail Mask", "Mask for Secondary Maps (A)");
            public static GUIContent detailAlbedoText = EditorGUIUtility.TrTextContent("Detail Albedo x2", "Albedo (RGB) multiplied by 2");
            public static GUIContent detailNormalMapText = EditorGUIUtility.TrTextContent("Normal Map", "Normal Map");
            // public static GUIContent rimPowerText = EditorGUIUtility.TrTextContent("Rim Power", "Rim Power");
            // public static GUIContent denierText = EditorGUIUtility.TrTextContent("Denier", "Denier");
            public static GUIContent layerMapText = EditorGUIUtility.TrTextContent("Layer Map", "Noise texture used to control the growth of fur");
            public static GUIContent furLengthText = EditorGUIUtility.TrTextContent("Fur Length", "control fur length");
            public static GUIContent CutoffText = EditorGUIUtility.TrTextContent("Cutoff", "how thick");
            public static GUIContent CutoffEndText = EditorGUIUtility.TrTextContent("CutoffEnd", "how thick they are at the end");
            public static GUIContent EdgeFadeText = EditorGUIUtility.TrTextContent("EdgeFade", "EdgeFade");
            public static GUIContent ForceDirectionText = EditorGUIUtility.TrTextContent("Force Direction", "the direction of force");
            public static GUIContent ForceScaleText = EditorGUIUtility.TrTextContent("Force Scale", "ForceScale");
            public static GUIContent SrcBlendText = EditorGUIUtility.TrTextContent("SrcBlend", "");
            public static GUIContent DstBlendText = EditorGUIUtility.TrTextContent("DstBlend", "");
            public static GUIContent ZwriteText = EditorGUIUtility.TrTextContent("Zwrite", "");
            public static GUIContent AnisoMapText = EditorGUIUtility.TrTextContent("AnisoMap", "Jitter(R) AnisoMask(G) FlowMask(B) EmissionMap(A)");
            public static GUIContent TangentDirText = EditorGUIUtility.TrTextContent("TangentDir", "");
            public static GUIContent AnisoGloss1Text = EditorGUIUtility.TrTextContent("AnisoGloss1", "");
            public static GUIContent AnisoSpec1Text = EditorGUIUtility.TrTextContent("AnisoSpec1", "");
            public static GUIContent AnisoGloss2Text = EditorGUIUtility.TrTextContent("AnisoGloss2", "");
            public static GUIContent AnisoSpec2Text = EditorGUIUtility.TrTextContent("AnisoSpec2", "");
            public static GUIContent TS1Text = EditorGUIUtility.TrTextContent("TangentShift1", "");
            public static GUIContent TS2Text = EditorGUIUtility.TrTextContent("TangentShift2", "");
            public static GUIContent AOText = EditorGUIUtility.TrTextContent("Ambient Occlusion", "");
            public static GUIContent SC1 = EditorGUIUtility.TrTextContent("Specular Color 1", "");
            public static GUIContent SC2 = EditorGUIUtility.TrTextContent("Specular Color 2", "");
            public static GUIContent FGText = EditorGUIUtility.TrTextContent("Fur Height Control Scale", "");
            public static GUIContent FGMText = EditorGUIUtility.TrTextContent("Fur Height Map", "");
            public static GUIContent ForeMapText = EditorGUIUtility.TrTextContent("Force Map", "");
            public static GUIContent ForceMapScaleText = EditorGUIUtility.TrTextContent("Force Map Scale", "");

            public static GUIContent tipcolorText = EditorGUIUtility.TrTextContent("Tip Color", "choose the color of the fur tip");
            public static GUIContent tipcontrolText = EditorGUIUtility.TrTextContent("Tip Control", "control the strength of the tip color");
            public static GUIContent tipchoiceText = EditorGUIUtility.TrTextContent("Tip Choice", "whether add tip color");
            public static GUIContent tipLocateText = EditorGUIUtility.TrTextContent("Tip Locate Map", "Locate area where have tip color");
            public static GUIContent scattercolorText = EditorGUIUtility.TrTextContent("Fabric Scatter Color", "");
            public static GUIContent scatterscaleText = EditorGUIUtility.TrTextContent("Fabric Scatter Scale", "");

            public static string primaryMapsText = "Main Maps";
            public static string secondaryMapsText = "Secondary Maps";
            public static string forwardText = "Forward Rendering Options";
            public static string renderingMode = "Rendering Mode";
            public static string advancedText = "Advanced Options";
            //这里用于添加拓展材质选择
            public static string fur_characteristic = "Fur Characteristic";
            public static string fur_control = "Fur Growth Control";
            //public static readonly string[] renderNames = Enum.GetNames(typeof(RenderingMode));
        }

        MaterialProperty RenderMode = null;

        MaterialProperty tipcolor = null;
        MaterialProperty tipcontrol = null;
        MaterialProperty tipchoice = null;
        MaterialProperty tipLocate = null;

        MaterialProperty albedoMap = null;
        MaterialProperty albedoColor = null;
        MaterialProperty alphaCutoff = null;
        MaterialProperty specularMap = null;
        MaterialProperty specularColor = null;
        MaterialProperty metallicMap = null;
        MaterialProperty metallic = null;
        MaterialProperty smoothness = null;
        MaterialProperty smoothnessScale = null;
        MaterialProperty smoothnessMapChannel = null;
        MaterialProperty highlights = null;
        MaterialProperty reflections = null;
        MaterialProperty bumpScale = null;
        MaterialProperty bumpMap = null;
        MaterialProperty occlusionStrength = null;
        MaterialProperty occlusionMap = null;
        MaterialProperty heigtMapScale = null;
        MaterialProperty heightMap = null;
        MaterialProperty emissionColorForRendering = null;
        MaterialProperty emissionMap = null;
        MaterialProperty detailMask = null;
        MaterialProperty detailAlbedoMap = null;
        MaterialProperty detailNormalMapScale = null;
        MaterialProperty detailNormalMap = null;
        MaterialProperty uvSetSecondary = null;

        //Fur characteristic
        MaterialProperty layerMap = null;
        MaterialProperty furLength = null;
        MaterialProperty Cutoff = null;
        MaterialProperty CutoffEnd = null;
        MaterialProperty EdgeFade = null;
        MaterialProperty ForceDirection = null;
        MaterialProperty ForeceScale = null;
        MaterialProperty SrcBlend = null;
        MaterialProperty DstBlend = null;
        MaterialProperty Zwrite = null;
        MaterialProperty AnisoMap = null;
        MaterialProperty TangentDir = null;
        MaterialProperty specularColor1 = null;
        MaterialProperty specularColor2 = null;
        MaterialProperty AG1 = null;
        MaterialProperty AG2 = null;
        MaterialProperty AS1 = null;
        MaterialProperty AS2 = null;
        MaterialProperty TS1 = null;
        MaterialProperty TS2 = null;
        MaterialProperty AO = null;

        MaterialProperty FG = null;
        MaterialProperty FGM = null;
        MaterialProperty ForceMap = null;
        MaterialProperty ForceMapScale = null;

        MaterialProperty FabricScatterColor = null;
        MaterialProperty FabricScatterScale = null;
        //Fur distribution
        //MaterialProperty Hei;
        MaterialEditor m_MaterialEditor;
        WorkflowMode m_WorkflowMode = WorkflowMode.Specular;

        bool m_FirstTimeApply = true;

        public void FindProperties(MaterialProperty[] props)
        {
            RenderMode = FindProperty("_RenderChoice", props);

            tipcolor = FindProperty("_TipColor", props);
            tipcontrol = FindProperty("_TipControl", props);
            tipchoice = FindProperty("_TipChoice", props);
            tipLocate = FindProperty("_TipLocateMap", props);

            FabricScatterColor = FindProperty("_FabricScatterColor", props);
            FabricScatterScale = FindProperty("_FabricScatterScale", props);

            albedoMap = FindProperty("_MainTex", props);
            albedoColor = FindProperty("_Color", props);
            alphaCutoff = FindProperty("_Cutoff", props);
            specularMap = FindProperty("_SpecGlossMap", props, false);
            specularColor = FindProperty("_SpecColor", props, false);
            specularColor1 = FindProperty("_SpecColor1", props, false);
            specularColor2 = FindProperty("_SpecColor2", props, false);
            metallicMap = FindProperty("_MetallicGlossMap", props, false);
            metallic = FindProperty("_Metallic", props, false);
            if (specularMap != null && specularColor != null)
                m_WorkflowMode = WorkflowMode.Specular;
            else if (metallicMap != null && metallic != null)
                m_WorkflowMode = WorkflowMode.Metallic;
            else
                m_WorkflowMode = WorkflowMode.Dielectric;
            smoothness = FindProperty("_Glossiness", props);
            smoothnessScale = FindProperty("_GlossMapScale", props, false);
            smoothnessMapChannel = FindProperty("_SmoothnessTextureChannel", props, false);
            highlights = FindProperty("_SpecularHighlights", props, false);
            reflections = FindProperty("_GlossyReflections", props, false);
            bumpScale = FindProperty("_BumpScale", props);
            bumpMap = FindProperty("_BumpMap", props);
            heigtMapScale = FindProperty("_Parallax", props);
            heightMap = FindProperty("_ParallaxMap", props);
            occlusionStrength = FindProperty("_OcclusionStrength", props);
            occlusionMap = FindProperty("_OcclusionMap", props);
            emissionColorForRendering = FindProperty("_EmissionColor", props);
            emissionMap = FindProperty("_EmissionMap", props);
            detailMask = FindProperty("_DetailMask", props);
            detailAlbedoMap = FindProperty("_DetailAlbedoMap", props);
            detailNormalMapScale = FindProperty("_DetailNormalMapScale", props);
            detailNormalMap = FindProperty("_DetailNormalMap", props);
            uvSetSecondary = FindProperty("_UVSec", props);
            // denier = FindProperty("_Denier", props);
            // rimPower = FindProperty("_RimPower", props);
            layerMap = FindProperty("_LayerTex", props);
            furLength = FindProperty("_FurLength", props);
            Cutoff = FindProperty("_Cutoff", props);
            CutoffEnd = FindProperty("_CutoffEnd", props);
            EdgeFade = FindProperty("_EdgeFade", props);
            ForceDirection = FindProperty("_Gravity", props);
            ForeceScale = FindProperty("_GravityStrength", props);
            DstBlend = FindProperty("_DstBlend", props);
            SrcBlend = FindProperty("_SrcBlend", props);
            Zwrite = FindProperty("_ZWrite", props);
            AnisoMap = FindProperty("_AnisoMap", props);
            TangentDir = FindProperty("_TangentDir", props);
            AG1 = FindProperty("_AnisoGloss1", props);
            AG2 = FindProperty("_AnisoGloss2", props);
            AS1 = FindProperty("_AnisoSpec1", props);
            AS2 = FindProperty("_AnisoSpec2", props);
            TS1 = FindProperty("_TangentShift1", props);
            TS2 = FindProperty("_TangentShift2", props);
            AO = FindProperty("_AO", props);

            FG = FindProperty("_FurGrowth", props);
            FGM = FindProperty("_FurGrowthMap", props);
            ForceMap = FindProperty("_ForceMap", props);
            ForceMapScale = FindProperty("_ForceMapScale", props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            FindProperties(props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
            m_MaterialEditor = materialEditor;
            Material material = materialEditor.target as Material;

            // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
            // material to a standard shader.
            // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
            if (m_FirstTimeApply)
            {
                MaterialChanged(material, m_WorkflowMode, false);
                m_FirstTimeApply = false;
            }

            ShaderPropertiesGUI(material);
        }

        public void ShaderPropertiesGUI(Material material)
        {
            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            {
                // blendModeChanged = BlendModePopup();
                //EditorGUI.showMixedValue = RenderingMode.hasMixedValue;
                // Primary properties

                m_MaterialEditor.ShaderProperty(RenderMode, "Rendering Choices");
                GUILayout.Label(Styles.primaryMapsText, EditorStyles.boldLabel);
                DoAlbedoArea(material);
                EditorGUILayout.Space();
                m_MaterialEditor.ShaderProperty(FabricScatterColor, Styles.scattercolorText);
                m_MaterialEditor.ShaderProperty(FabricScatterScale, Styles.scatterscaleText);
                EditorGUILayout.Space();
                m_MaterialEditor.ShaderProperty(tipcolor, Styles.tipcolorText);
                m_MaterialEditor.ShaderProperty(tipcontrol, Styles.tipcontrolText);
                m_MaterialEditor.ShaderProperty(tipchoice, Styles.tipchoiceText);
                m_MaterialEditor.ShaderProperty(tipLocate, Styles.tipLocateText);
                EditorGUILayout.Space();
                DoSpecularMetallicArea();
                DoNormalArea();
                // m_MaterialEditor.ShaderProperty(rimPower, Styles.rimPowerText);
                // m_MaterialEditor.ShaderProperty(denier, Styles.denierText);
                m_MaterialEditor.TexturePropertySingleLine(Styles.heightMapText, heightMap, heightMap.textureValue != null ? heigtMapScale : null);
                m_MaterialEditor.TexturePropertySingleLine(Styles.occlusionText, occlusionMap, occlusionMap.textureValue != null ? occlusionStrength : null);
                m_MaterialEditor.TexturePropertySingleLine(Styles.detailMaskText, detailMask);
                DoEmissionArea(material);

                EditorGUI.BeginChangeCheck();
                m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);

                if (EditorGUI.EndChangeCheck())
                    emissionMap.textureScaleAndOffset = albedoMap.textureScaleAndOffset; // Apply the main texture scale and offset to the emission texture as well, for Enlighten's sake

                EditorGUILayout.Space();

                // Secondary properties
                GUILayout.Label(Styles.secondaryMapsText, EditorStyles.boldLabel);
                m_MaterialEditor.TexturePropertySingleLine(Styles.detailAlbedoText, detailAlbedoMap);
                m_MaterialEditor.TexturePropertySingleLine(Styles.detailNormalMapText, detailNormalMap, detailNormalMapScale);
                m_MaterialEditor.TextureScaleOffsetProperty(detailAlbedoMap);
                m_MaterialEditor.ShaderProperty(uvSetSecondary, Styles.uvSetLabel.text);

                // Third properties
                GUILayout.Label(Styles.forwardText, EditorStyles.boldLabel);
                if (highlights != null)
                    m_MaterialEditor.ShaderProperty(highlights, Styles.highlightsText);
                if (reflections != null)
                    m_MaterialEditor.ShaderProperty(reflections, Styles.reflectionsText);

                EditorGUILayout.Space();

                GUILayout.Label(Styles.advancedText, EditorStyles.boldLabel);
                m_MaterialEditor.RenderQueueField();
                EditorGUILayout.Space();
                // Fur Properties
                GUILayout.Label(Styles.fur_characteristic, EditorStyles.boldLabel);
                EditorGUILayout.Space();
                m_MaterialEditor.TexturePropertySingleLine(Styles.layerMapText, layerMap);
                m_MaterialEditor.TextureScaleOffsetProperty(layerMap);
                m_MaterialEditor.ShaderProperty(furLength, Styles.furLengthText);
                m_MaterialEditor.ShaderProperty(AO, Styles.AOText);
                m_MaterialEditor.ShaderProperty(Cutoff, Styles.CutoffText);
                m_MaterialEditor.ShaderProperty(CutoffEnd, Styles.CutoffEndText);
                m_MaterialEditor.ShaderProperty(EdgeFade, Styles.EdgeFadeText);
                m_MaterialEditor.ShaderProperty(ForceDirection, Styles.ForceDirectionText);
                m_MaterialEditor.ShaderProperty(ForeceScale, Styles.ForceScaleText);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                //Blend
                m_MaterialEditor.ShaderProperty(SrcBlend, Styles.SrcBlendText);
                m_MaterialEditor.ShaderProperty(DstBlend, Styles.DstBlendText);
                m_MaterialEditor.ShaderProperty(Zwrite, Styles.ZwriteText);
                EditorGUILayout.Space();
                //Aniso
                m_MaterialEditor.TexturePropertySingleLine(Styles.AnisoMapText, AnisoMap);
                m_MaterialEditor.TextureScaleOffsetProperty(AnisoMap);
                m_MaterialEditor.ShaderProperty(specularColor1, Styles.SC1);
                m_MaterialEditor.ShaderProperty(specularColor2, Styles.SC2);
                m_MaterialEditor.ShaderProperty(TangentDir, Styles.TangentDirText);
                m_MaterialEditor.ShaderProperty(AG1, Styles.AnisoGloss1Text);
                m_MaterialEditor.ShaderProperty(AS1, Styles.AnisoSpec1Text);
                m_MaterialEditor.ShaderProperty(AG2, Styles.AnisoGloss2Text);
                m_MaterialEditor.ShaderProperty(AS2, Styles.AnisoSpec2Text);
                m_MaterialEditor.ShaderProperty(TS1, Styles.TS1Text);
                m_MaterialEditor.ShaderProperty(TS2, Styles.TS2Text);
                EditorGUILayout.Space();
                //Fur Growth Control
                m_MaterialEditor.ShaderProperty(ForceMap, Styles.ForeMapText);
                m_MaterialEditor.ShaderProperty(ForceMapScale, Styles.ForceMapScaleText);
                m_MaterialEditor.ShaderProperty(FGM, Styles.FGMText);
                m_MaterialEditor.ShaderProperty(FG, Styles.FGText);

            }
            if (EditorGUI.EndChangeCheck())
            {
                MaterialChanged(material, m_WorkflowMode, true);
            }


            m_MaterialEditor.EnableInstancingField();
            m_MaterialEditor.DoubleSidedGIField();
        }

        internal void DetermineWorkflow(MaterialProperty[] props)
        {
            if (FindProperty("_SpecGlossMap", props, false) != null && FindProperty("_SpecColor", props, false) != null)
                m_WorkflowMode = WorkflowMode.Specular;
            else if (FindProperty("_MetallicGlossMap", props, false) != null && FindProperty("_Metallic", props, false) != null)
                m_WorkflowMode = WorkflowMode.Metallic;
            else
                m_WorkflowMode = WorkflowMode.Dielectric;
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                return;
            }



            DetermineWorkflow(MaterialEditor.GetMaterialProperties(new Material[] { material }));
            MaterialChanged(material, m_WorkflowMode, true);
        }


        void DoNormalArea()
        {
            m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap, bumpMap.textureValue != null ? bumpScale : null);
            if (bumpScale.floatValue != 1
                && UnityEditorInternal.InternalEditorUtility.IsMobilePlatform(EditorUserBuildSettings.activeBuildTarget))
                if (m_MaterialEditor.HelpBoxWithButton(
                    EditorGUIUtility.TrTextContent("Bump scale is not supported on mobile platforms"),
                    EditorGUIUtility.TrTextContent("Fix Now")))
                {
                    bumpScale.floatValue = 1;
                }
        }

        void DoAlbedoArea(Material material)
        {
            m_MaterialEditor.TexturePropertySingleLine(Styles.albedoText, albedoMap, albedoColor);

        }

        void DoEmissionArea(Material material)
        {
            // Emission for GI?
            if (m_MaterialEditor.EmissionEnabledProperty())
            {
                bool hadEmissionTexture = emissionMap.textureValue != null;

                // Texture and HDR color controls
                m_MaterialEditor.TexturePropertyWithHDRColor(Styles.emissionText, emissionMap, emissionColorForRendering, false);

                // If texture was assigned and color was black set color to white
                float brightness = emissionColorForRendering.colorValue.maxColorComponent;
                if (emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                    emissionColorForRendering.colorValue = Color.white;

                // change the GI flag and fix it up with emissive as black if necessary
                m_MaterialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true);
            }
        }

        void DoSpecularMetallicArea()
        {
            bool hasGlossMap = false;
            if (m_WorkflowMode == WorkflowMode.Specular)
            {
                hasGlossMap = specularMap.textureValue != null;
                m_MaterialEditor.TexturePropertySingleLine(Styles.specularMapText, specularMap, hasGlossMap ? null : specularColor);
            }
            else if (m_WorkflowMode == WorkflowMode.Metallic)
            {
                hasGlossMap = metallicMap.textureValue != null;
                m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapText, metallicMap, hasGlossMap ? null : metallic);
            }

            bool showSmoothnessScale = hasGlossMap;
            if (smoothnessMapChannel != null)
            {
                int smoothnessChannel = (int)smoothnessMapChannel.floatValue;
                if (smoothnessChannel == (int)SmoothnessMapChannel.AlbedoAlpha)
                    showSmoothnessScale = true;
            }

            int indentation = 2; // align with labels of texture properties
            m_MaterialEditor.ShaderProperty(showSmoothnessScale ? smoothnessScale : smoothness, showSmoothnessScale ? Styles.smoothnessScaleText : Styles.smoothnessText, indentation);

            ++indentation;
            if (smoothnessMapChannel != null)
                m_MaterialEditor.ShaderProperty(smoothnessMapChannel, Styles.smoothnessMapChannelText, indentation);
        }



        static SmoothnessMapChannel GetSmoothnessMapChannel(Material material)
        {
            int ch = (int)material.GetFloat("_SmoothnessTextureChannel");
            if (ch == (int)SmoothnessMapChannel.AlbedoAlpha)
                return SmoothnessMapChannel.AlbedoAlpha;
            else
                return SmoothnessMapChannel.SpecularMetallicAlpha;
        }

        static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
        {
            // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
            // (MaterialProperty value might come from renderer material property block)
            SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap") || material.GetTexture("_DetailNormalMap"));
            if (workflowMode == WorkflowMode.Specular)
                SetKeyword(material, "_SPECGLOSSMAP", material.GetTexture("_SpecGlossMap"));
            else if (workflowMode == WorkflowMode.Metallic)
                SetKeyword(material, "_METALLICGLOSSMAP", material.GetTexture("_MetallicGlossMap"));
            SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));
            SetKeyword(material, "_DETAIL_MULX2", material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap"));

            // A material's GI flag internally keeps track of whether emission is enabled at all, it's enabled but has no effect
            // or is enabled and may be modified at runtime. This state depends on the values of the current flag and emissive color.
            // The fixup routine makes sure that the material is in the correct state if/when changes are made to the mode or color.
            MaterialEditor.FixupEmissiveFlag(material);
            bool shouldEmissionBeEnabled = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
            SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);

            if (material.HasProperty("_SmoothnessTextureChannel"))
            {
                SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", GetSmoothnessMapChannel(material) == SmoothnessMapChannel.AlbedoAlpha);
            }
            //Custom Txture
            SetKeyword(material, "_TIPLOCATEMAP", material.GetTexture("_TipLocateMap"));
        }

        static void MaterialChanged(Material material, WorkflowMode workflowMode, bool overrideRenderQueue)
        {
            SetMaterialKeywords(material, workflowMode);
            int ch = (int)material.GetFloat("_RenderChoice");
            if (ch == 0)
            {
                material.DisableKeyword("_KK");
                material.EnableKeyword("_BRDF");

            }
            else
            {
                material.DisableKeyword("_BRDF");
                material.EnableKeyword("_KK");
            }
        }

        static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
                m.EnableKeyword(keyword);
            else
                m.DisableKeyword(keyword);
        }
    }
} // namespace UnityEditor
#endif
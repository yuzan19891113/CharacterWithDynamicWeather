// Magica Cloth.
// Copyright (c) MagicaSoft, 2020-2021.
// https://magicasoft.jp

namespace MagicaCloth
{
    /// <summary>
    /// コンポーネント種類
    /// </summary>
    public enum ComponentType
    {
        None,

        SphereCollider = 100,
        CapsuleCollider = 101,
        PlaneCollider = 102,

        DirectionalWind = 200,

        BoneCloth = 1000,
        BoneSpring = 1001,

        MeshCloth = 2000,
        MeshSpring = 2001,

        RenderDeformer = 3000,
        VirtualDeformer = 3001,

        Avatar = 4000,
        AvatarParts = 4001,
    }
}
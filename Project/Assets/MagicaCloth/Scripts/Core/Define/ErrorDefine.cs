﻿// Magica Cloth.
// Copyright (c) MagicaSoft, 2020-2021.
// https://magicasoft.jp

using System.Text;

namespace MagicaCloth
{
    public static partial class Define
    {
        /// <summary>
        /// 結果コード
        /// </summary>
        public enum Error
        {
            None = 0, // なし

            // エラー
            EmptyData = 100,
            InvalidDataHash = 101,
            TooOldDataVersion = 102,

            MeshDataNull = 200,
            MeshDataHashMismatch = 201,
            MeshDataVersionMismatch = 202,

            ClothDataNull = 300,
            ClothDataHashMismatch = 301,
            ClothDataVersionMismatch = 302,

            ClothSelectionHashMismatch = 400,
            ClothSelectionVersionMismatch = 401,

            ClothTargetRootCountMismatch = 500,

            UseTransformNull = 600,
            UseTransformCountZero = 601,
            UseTransformCountMismatch = 602,

            DeformerNull = 700,
            DeformerHashMismatch = 701,
            DeformerVersionMismatch = 702,
            DeformerCountZero = 703,
            DeformerCountMismatch = 704,

            VertexCountZero = 800,
            VertexUseCountZero = 801,
            VertexCountMismatch = 802,

            RootListCountMismatch = 900,

            SelectionDataCountMismatch = 1000,
            SelectionCountZero = 1001,

            CenterTransformNull = 1100,

            SpringDataNull = 1200,
            SpringDataHashMismatch = 1201,
            SpringDataVersionMismatch = 1202,

            TargetObjectNull = 1300,

            SharedMeshNull = 1400,
            SharedMeshCannotRead = 1401,
            SharedMeshDifferent = 1402,
            SharedMeshDifferentVertexCount = 1403,

            MeshOptimizeMismatch = 1500,
            MeshVertexCount65535Over = 1501,

            BoneListZero = 1600,
            BoneListNull = 1601,

            // ここからはランタイムエラー(10000～)

            // ここからはワーニング(20000～)
            OverlappingTransform = 20000,
            AddOverlappingTransform = 20001,
            OldDataVersion = 20002,
        }

        /// <summary>
        /// コードがエラーが無く正常か判定する
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static bool IsNormal(Error err)
        {
            return err == Error.None;
        }

        /// <summary>
        /// コードがエラーか判定する
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static bool IsError(Error err)
        {
            return err != Error.None && (int)err < 20000;
        }

        /// <summary>
        /// コードがワーニングか判定する
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static bool IsWarning(Error err)
        {
            return (int)err >= 20000;
        }

        /// <summary>
        /// エラーメッセージを取得する
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static string GetErrorMessage(Error err)
        {
            StringBuilder sb = new StringBuilder(512);

            // 基本エラーコード
            sb.AppendFormat("{0} ({1}) : {2}", IsError(err) ? "Error" : "Warning", (int)err, err.ToString());
            //if ((int)err < 20000)
            //    sb.AppendFormat("Error ({0}) : {1}", (int)err, err.ToString());
            //else
            //    sb.AppendFormat("Warning ({0}) : {1}", (int)err, err.ToString());

            // 個別の詳細メッセージ
            switch (err)
            {
                case Error.SharedMeshCannotRead:
                    sb.AppendLine();
                    sb.Append("Please turn On the [Read/Write Enabled] flag of the mesh importer.");
                    break;
                case Error.OldDataVersion:
                    sb.Clear();
                    sb.Append("Old data format!");
                    sb.AppendLine();
                    sb.Append("It may not work or the latest features may not be available.");
                    sb.AppendLine();
                    sb.Append("It is recommended to press the [Create] button and rebuild the data.");
                    break;
                case Error.EmptyData:
                    sb.Clear();
                    sb.Append("No Data.");
                    break;
            }

            return sb.ToString();

        }
    }
}

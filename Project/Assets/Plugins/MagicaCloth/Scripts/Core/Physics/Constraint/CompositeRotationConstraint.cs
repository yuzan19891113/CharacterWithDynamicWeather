// Magica Cloth.
// Copyright (c) MagicaSoft, 2020-2021.
// https://magicasoft.jp
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace MagicaCloth
{
    /// <summary>
    /// 複合回転拘束(v1.11.0より)
    /// [Algorithm 2]
    /// ClampRotationとRestoreRotationを融合させたもの
    /// ルートラインベースに反復することで振動を抑える
    /// </summary>
    public class CompositeRotationConstraint : PhysicsManagerConstraint
    {
        /// <summary>
        /// 拘束データ
        /// Clmap/Restore共通
        /// </summary>
        [System.Serializable]
        public struct RotationData
        {
            /// <summary>
            /// 計算頂点インデックス
            /// </summary>
            public int vertexIndex;

            /// <summary>
            /// 親頂点インデックス
            /// </summary>
            public int parentVertexIndex;

            /// <summary>
            /// 親から自身への本来のローカル方向（単位ベクトル）
            /// </summary>
            public float3 localPos;

            /// <summary>
            /// 親から自身への本来のローカル回転
            /// </summary>
            public quaternion localRot;

            /// <summary>
            /// データが有効か判定する
            /// </summary>
            /// <returns></returns>
            public bool IsValid()
            {
                return vertexIndex > 0 || parentVertexIndex > 0;
            }
        }
        FixedChunkNativeArray<RotationData> dataList;

        [System.Serializable]
        public struct RootInfo
        {
            public ushort startIndex;
            public ushort dataLength;
        }
        FixedChunkNativeArray<RootInfo> rootInfoList;

        /// <summary>
        /// グループデータ
        /// </summary>
        public struct GroupData
        {
            public int teamId;
            public int useClamp;
            public int useRestore;

            /// <summary>
            /// 最大角度
            /// </summary>
            public CurveParam maxAngle;

            public CurveParam restorePower;

            /// <summary>
            /// 速度影響
            /// </summary>
            public float restoreVelocityInfluence;

            public ChunkData dataChunk;
            public ChunkData rootInfoChunk;
        }
        public FixedNativeList<GroupData> groupList;

        /// <summary>
        /// ルートごとのチームインデックス
        /// </summary>
        FixedChunkNativeArray<int> rootTeamList;

        /// <summary>
        /// 拘束データごとの作業バッファ
        /// </summary>
        FixedChunkNativeArray<float> lengthBuffer;

        //=========================================================================================
        public override void Create()
        {
            dataList = new FixedChunkNativeArray<RotationData>();
            rootInfoList = new FixedChunkNativeArray<RootInfo>();
            groupList = new FixedNativeList<GroupData>();
            rootTeamList = new FixedChunkNativeArray<int>();
            lengthBuffer = new FixedChunkNativeArray<float>();
        }

        public override void Release()
        {
            dataList.Dispose();
            rootInfoList.Dispose();
            groupList.Dispose();
            rootTeamList.Dispose();
            lengthBuffer.Dispose();
        }

        //=========================================================================================
        public int AddGroup(
            int teamId,
            bool useClamp,
            BezierParam maxAngle,
            bool useRestore,
            BezierParam restorePower,
            float velocityInfluence,
            RotationData[] dataArray,
            RootInfo[] rootInfoArray
            )
        {
            if (dataArray == null || dataArray.Length == 0 || rootInfoArray == null || rootInfoArray.Length == 0)
                return -1;

            var gdata = new GroupData();
            gdata.teamId = teamId;
            gdata.useClamp = useClamp ? 1 : 0;
            gdata.maxAngle.Setup(maxAngle);
            gdata.useRestore = useRestore ? 1 : 0;
            gdata.restorePower.Setup(restorePower);
            gdata.restoreVelocityInfluence = velocityInfluence;
            gdata.dataChunk = dataList.AddChunk(dataArray.Length);
            gdata.rootInfoChunk = rootInfoList.AddChunk(rootInfoArray.Length);

            // チャンクデータコピー
            dataList.ToJobArray().CopyFromFast(gdata.dataChunk.startIndex, dataArray);
            rootInfoList.ToJobArray().CopyFromFast(gdata.rootInfoChunk.startIndex, rootInfoArray);

            int group = groupList.Add(gdata);

            // ルートごとのチームインデックス
            var c = rootTeamList.AddChunk(rootInfoArray.Length);
            rootTeamList.Fill(c, teamId);

            // 作業バッファ
            lengthBuffer.AddChunk(dataArray.Length);

            return group;
        }

        public override void RemoveTeam(int teamId)
        {
            var teamData = MagicaPhysicsManager.Instance.Team.teamDataList[teamId];

            int group = teamData.compositeRotationGroupIndex;
            if (group >= 0)
            {
                var cdata = groupList[group];

                // チャンクデータ削除
                dataList.RemoveChunk(cdata.dataChunk);
                rootInfoList.RemoveChunk(cdata.rootInfoChunk);
                rootTeamList.RemoveChunk(cdata.rootInfoChunk);
                lengthBuffer.RemoveChunk(cdata.dataChunk);

                // データ削除
                groupList.Remove(group);
            }
        }

        public void ChangeParam(
            int teamId,
            bool useClamp,
            BezierParam maxAngle,
            bool useRestore,
            BezierParam restorePower,
            float velocityInfluence
            )
        {
            var teamData = MagicaPhysicsManager.Instance.Team.teamDataList[teamId];
            int group = teamData.compositeRotationGroupIndex;
            if (group < 0)
                return;

            var gdata = groupList[group];
            gdata.useClamp = useClamp ? 1 : 0;
            gdata.maxAngle.Setup(maxAngle);
            gdata.useRestore = useRestore ? 1 : 0;
            gdata.restorePower.Setup(restorePower);
            gdata.restoreVelocityInfluence = velocityInfluence;
            groupList[group] = gdata;
        }

        //=========================================================================================
        /// <summary>
        /// 拘束の解決
        /// </summary>
        /// <param name="dtime"></param>
        /// <param name="jobHandle"></param>
        /// <returns></returns>
        public override JobHandle SolverConstraint(int runCount, float dtime, float updatePower, int iteration, JobHandle jobHandle)
        {
            if (groupList.Count > 0)
            {
                // 回転拘束（ルートラインごと）
                var job = new RotationRootLineJob()
                {
                    updatePower = updatePower,
                    runCount = runCount,
                    maxMoveSpeed = dtime * Define.Compute.ClampRotationMaxVelocity2, // 最大2.0m/s

                    dataList = dataList.ToJobArray(),
                    rootInfoList = rootInfoList.ToJobArray(),
                    rootTeamList = rootTeamList.ToJobArray(),
                    groupList = groupList.ToJobArray(),

                    teamDataList = Manager.Team.teamDataList.ToJobArray(),
                    teamGravityList = Manager.Team.teamGravityList.ToJobArray(),

                    depthList = Manager.Particle.depthList.ToJobArray(),
                    flagList = Manager.Particle.flagList.ToJobArray(),
                    frictionList = Manager.Particle.frictionList.ToJobArray(),
                    baseRotList = Manager.Particle.baseRotList.ToJobArray(),
                    nextPosList = Manager.Particle.InNextPosList.ToJobArray(),
                    nextRotList = Manager.Particle.InNextRotList.ToJobArray(),
                    posList = Manager.Particle.posList.ToJobArray(),

                    lengthBuffer = lengthBuffer.ToJobArray(),
                };
                jobHandle = job.Schedule(rootTeamList.Length, 8, jobHandle);
            }

            return jobHandle;
        }

        /// <summary>
        /// 回転拘束ジョブ[Algorithm 2]
        /// </summary>
        [BurstCompile]
        struct RotationRootLineJob : IJobParallelFor
        {
            public float updatePower;
            public int runCount;
            public float maxMoveSpeed;

            [Unity.Collections.ReadOnly]
            public NativeArray<RotationData> dataList;
            [Unity.Collections.ReadOnly]
            public NativeArray<RootInfo> rootInfoList;
            [Unity.Collections.ReadOnly]
            public NativeArray<int> rootTeamList;
            [Unity.Collections.ReadOnly]
            public NativeArray<GroupData> groupList;

            [Unity.Collections.ReadOnly]
            public NativeArray<PhysicsManagerTeamData.TeamData> teamDataList;
            [Unity.Collections.ReadOnly]
            public NativeArray<CurveParam> teamGravityList;

            [Unity.Collections.ReadOnly]
            public NativeArray<float> depthList;
            [Unity.Collections.ReadOnly]
            public NativeArray<PhysicsManagerParticleData.ParticleFlag> flagList;
            [Unity.Collections.ReadOnly]
            public NativeArray<float> frictionList;
            [Unity.Collections.ReadOnly]
            public NativeArray<quaternion> baseRotList;
            [NativeDisableParallelForRestriction]
            public NativeArray<float3> nextPosList;
            [NativeDisableParallelForRestriction]
            public NativeArray<quaternion> nextRotList;
            [NativeDisableParallelForRestriction]
            public NativeArray<float3> posList;

            [NativeDisableParallelForRestriction]
            public NativeArray<float> lengthBuffer;

            // ルートラインごと
            public void Execute(int rootIndex)
            {
                // チーム
                int teamIndex = rootTeamList[rootIndex];
                if (teamIndex == 0)
                    return;
                var team = teamDataList[teamIndex];
                if (team.IsActive() == false || team.compositeRotationGroupIndex < 0)
                    return;

                // 更新確認
                if (team.IsUpdate(runCount) == false)
                    return;

                // グループデータ
                var gdata = groupList[team.compositeRotationGroupIndex];
                if (gdata.useClamp == 0 && gdata.useRestore == 0)
                    return;

                // データ
                var rootInfo = rootInfoList[rootIndex];
                int dataIndex = rootInfo.startIndex + gdata.dataChunk.startIndex;
                int dataCount = rootInfo.dataLength;
                int pstart = team.particleChunk.startIndex;

                // （１）現在の親からのベクトル長を保持する
                if (gdata.useClamp == 1)
                {
                    for (int i = 0; i < dataCount; i++)
                    {
                        var data = dataList[dataIndex + i];
                        int pindex = data.parentVertexIndex;
                        if (pindex < 0)
                            continue;

                        var index = data.vertexIndex;
                        index += pstart;
                        pindex += pstart;

                        var npos = nextPosList[index];
                        var ppos = nextPosList[pindex];

                        // 現在ベクトル長
                        float vlen = math.distance(npos, ppos);

                        lengthBuffer[dataIndex + i] = vlen;
                    }
                }

                // ２回以上反復することで安定する
                const int iteration = 2;
                for (int j = 0; j < iteration; j++)
                {
                    // （２）ルートラインを親から処理する
                    for (int i = 0; i < dataCount; i++)
                    {
                        var data = dataList[dataIndex + i];
                        int pindex = data.parentVertexIndex;
                        if (pindex < 0)
                            continue;

                        int index = data.vertexIndex;

                        index += pstart;
                        pindex += pstart;

                        // 子の情報
                        var cflag = flagList[index];
                        if (cflag.IsValid() == false)
                            continue;
                        var cpos = nextPosList[index];
                        //var crot = nextRotList[index];
                        float cdepth = depthList[index];
                        float cfriction = frictionList[index];
                        float cmoveratio = math.saturate(1.0f - cfriction * Define.Compute.FrictionMoveRatio);

                        // 親の情報
                        var pflag = flagList[pindex];
                        var ppos = nextPosList[pindex];
                        var pbrot = baseRotList[pindex];
                        var prot = nextRotList[pindex];
                        float pfriction = frictionList[pindex];
                        float pmoveratio = math.saturate(1.0f - pfriction * Define.Compute.FrictionMoveRatio);

                        // 重力ベクトルを決定する
                        var gravity = math.abs(teamGravityList[teamIndex].Evaluate(cdepth));
                        float3 gravityVector = gravity > Define.Compute.Epsilon ? team.gravityDirection : 0;

                        //=====================================================
                        // Restore
                        //=====================================================
                        if (gdata.useRestore == 1)
                        {
                            // 現在ベクトル
                            float3 v = cpos - ppos;

                            // 本来のベクトル（常にベース回転から計算）
                            float3 tv = math.mul(pbrot, data.localPos * team.scaleDirection); // マイナススケール対応

                            // ベクトル長修正
                            //float vlen = math.length(v); // 最新の距離（※これは伸びる場合があるが、一番安定している）
                            //float blen = lengthBuffer[dataIndex + i]; // 計算前の距離
                            //vlen = math.clamp(vlen, 0.0f, blen * 1.2f); // 1.2?
                            //v = math.normalize(v) * vlen;

                            float restorePower = gdata.restorePower.Evaluate(cdepth);
                            restorePower = 1.0f - math.pow(1.0f - restorePower, updatePower);

                            // 球面線形補間
                            var q = MathUtility.FromToRotation(v, tv, restorePower);
                            float3 rv = math.mul(q, v);

                            // 回転中心割合
                            float rotRatio = GetRotRatio(tv, gravityVector, gravity);
                            float3 rotPos = ppos + v * rotRatio;

                            // 親と子のそれぞれの更新位置
                            float3 pfpos = rotPos - rv * rotRatio;
                            float3 cfpos = rotPos + rv * (1.0f - rotRatio);

                            // 加算
                            float3 padd = pfpos - ppos;
                            float3 cadd = cfpos - cpos;

                            // 摩擦考慮
                            padd *= pmoveratio;
                            cadd *= cmoveratio;

                            // 書き出し
                            float influence = gdata.restoreVelocityInfluence;
                            if (cflag.IsMove())
                            {
                                nextPosList[index] = cpos + cadd;
                                // 速度影響
                                posList[index] = posList[index] + (cadd * (1.0f - influence));

                                cpos += cadd;
                            }
                            if (pflag.IsMove())
                            {
                                nextPosList[pindex] = ppos + padd;
                                // 速度影響
                                posList[pindex] = posList[pindex] + (padd * (1.0f - influence));

                                ppos += padd;
                            }
                        }

                        //=====================================================
                        // Clamp
                        //=====================================================
                        if (gdata.useClamp == 1)
                        {
                            // 親基準回転
                            var trot = pflag.IsMove() ? prot : pbrot;

                            // 現在ベクトル
                            float3 v = cpos - ppos;

                            // 本来のベクトル
                            float3 tv = math.mul(trot, data.localPos * team.scaleDirection); // マイナススケール対応

                            // ベクトル長修正
                            float vlen = math.length(v); // 最新の距離（※これは伸びる場合があるが、一番安定している）
                            float blen = lengthBuffer[dataIndex + i]; // 計算前の距離
                            vlen = math.clamp(vlen, 0.0f, blen * 1.2f); // 1.2?
                            v = math.normalize(v) * vlen;

                            // ベクトル角度クランプ
                            float maxAngle = gdata.maxAngle.Evaluate(cdepth);
                            maxAngle = math.radians(maxAngle);
                            float angle = math.acos(math.dot(v, tv));

                            if (cflag.IsFixed() == false)
                            {
                                float3 rv = v;
                                if (angle > maxAngle)
                                {
                                    MathUtility.ClampAngle(v, tv, maxAngle, out rv);
                                }

                                // 回転中心割合
                                float rotRatio = GetRotRatio(tv, gravityVector, gravity);
                                float3 rotPos = ppos + v * rotRatio;

                                // 親と子のそれぞれの更新位置
                                float3 pfpos = rotPos - rv * rotRatio;
                                float3 cfpos = rotPos + rv * (1.0f - rotRatio);

                                // 加算
                                float3 padd = pfpos - ppos;
                                float3 cadd = cfpos - cpos;

                                // 最大速度
                                padd = MathUtility.ClampVector(padd, 0.0f, maxMoveSpeed);
                                cadd = MathUtility.ClampVector(cadd, 0.0f, maxMoveSpeed);

                                // 摩擦考慮
                                padd *= pmoveratio;
                                cadd *= cmoveratio;

                                // 書き出し
                                const float influence = 0.2f;
                                if (cflag.IsMove())
                                {
                                    nextPosList[index] = cpos + cadd;
                                    // 速度影響
                                    posList[index] = posList[index] + (cadd * (1.0f - influence));

                                    cpos += cadd;
                                }
                                if (pflag.IsMove())
                                {
                                    nextPosList[pindex] = ppos + padd;
                                    // 速度影響
                                    posList[pindex] = posList[pindex] + (padd * (1.0f - influence));

                                    ppos += padd;
                                }

                                // ベクトル補正
                                v = cpos - ppos;
                            }

                            // 回転補正
                            var nrot = math.mul(trot, new quaternion(data.localRot.value * team.quaternionScale)); // マイナススケール対応(v1.7.6)
                            var q = MathUtility.FromToRotation(tv, v);
                            nrot = math.mul(q, nrot);
                            nextRotList[index] = nrot;
                        }
                    }
                }
            }

            /// <summary>
            /// 補間目標ベクトルと重力ベクトルにより回転の中央割合を決定する
            /// 0.4では大きく安定するが動きが鈍くなる、0.2では動きは良いが補間ベクトルが重力天井方向だと安定しない
            /// そのため重力ベクトルとの角度により一番安定する割合を決定する
            /// </summary>
            /// <param name="tv"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            float GetRotRatio(float3 tv, float3 gravityVector, float gravity)
            {
#if true
                // 重力方向割合(0.0-1.0)
                float dot = math.dot(math.normalize(tv), gravityVector);
                dot = dot * 0.5f + 0.5f;

                // 角度による増加曲線は重力の強さにより調整
                float pow = math.lerp(4.0f, 1.0f, math.saturate(gravity / 9.8f)); // 4.0 - 1.0?

                // 角度による増加曲線(0.0-1.0)
                dot = math.pow(dot, pow);

                // 最終的な回転割合は角度率により線形補間する
                // 重力０の場合は中間値が使用される
                float rotRatio = math.lerp(0.25f, 0.45f, dot);

                return rotRatio;
#else
                return 0.3f; // 最初の調整
#endif
            }
        }

    }
}

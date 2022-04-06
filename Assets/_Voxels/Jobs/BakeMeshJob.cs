using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Voxels {
    [BurstCompile]
    public struct BakeMeshJob : IJobFor {
        [ReadOnly] public NativeArray<int> ids;

        public void Execute(int index) => Physics.BakeMesh(ids[index], false);
    }
}
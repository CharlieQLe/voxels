using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels {
    [BurstCompile]
    public struct GenerateMeshJob : IJob {
        [ReadOnly] public NativeArray<Voxel> batchedChunks;
        [WriteOnly] public NativeArray<CountTuple> chunkCounts;
        [WriteOnly] public NativeList<Vertex> vertices;
        [WriteOnly] public NativeList<int> indices;

        public void Execute() {
            int batchSize = batchedChunks.Length / VoxelUtility.CHUNK_VOLUME;
            for (int i = 0; i < batchSize; i++) {
                int indexOffset = i * VoxelUtility.CHUNK_VOLUME;
                CountTuple count = new CountTuple();
                for (int j = 0; j < VoxelUtility.CHUNK_VOLUME; j++) {
                    if (batchedChunks[indexOffset + j] == Voxel.Null) continue;
                    int3 pos = VoxelUtility.IndexToPositionInt3(j);
                    for (int k = 0; k < 6; k++) {
                        int3 n = VoxelUtility.VoxelPositionOffsets[k];
                        int3 adjPos = pos + n;
                        if (VoxelUtility.InBounds(adjPos) && batchedChunks[indexOffset + VoxelUtility.PositionToIndex(adjPos)] != Voxel.Null) continue;
                        vertices.Add(new Vertex(pos + VoxelUtility.FaceVertexPositions[VoxelUtility.FaceVertexIndices[4 * k]]    , n, new float2(0, 1)));
                        vertices.Add(new Vertex(pos + VoxelUtility.FaceVertexPositions[VoxelUtility.FaceVertexIndices[4 * k + 1]], n, new float2(1, 1)));
                        vertices.Add(new Vertex(pos + VoxelUtility.FaceVertexPositions[VoxelUtility.FaceVertexIndices[4 * k + 2]], n, new float2(0, 0)));
                        vertices.Add(new Vertex(pos + VoxelUtility.FaceVertexPositions[VoxelUtility.FaceVertexIndices[4 * k + 3]], n, new float2(1, 0)));
                        indices.Add(count.vertexCount);
                        indices.Add(count.vertexCount + 1);
                        indices.Add(count.vertexCount + 2);
                        indices.Add(count.vertexCount + 2);
                        indices.Add(count.vertexCount + 1);
                        indices.Add(count.vertexCount + 3);
                        count.vertexCount += 4;
                        count.indexCount += 6;
                    }
                }
                chunkCounts[i] = count;
            }
        }
    }
}
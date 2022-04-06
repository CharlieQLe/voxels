using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace Voxels {
    [BurstCompile]
    public struct SetMeshJob : IJobFor {
        public Mesh.MeshDataArray meshDataArray;
        [ReadOnly] public NativeArray<CountTuple> chunkCounts;
        [ReadOnly] public NativeArray<Vertex> vertices;
        [ReadOnly] public NativeArray<int> indices;

        public void Execute(int index) {
            Mesh.MeshData meshData = meshDataArray[index];
            CountTuple count = chunkCounts[index];
            int vertexStart = 0;
            int indexStart = 0;
            for (int i = 0; i < index; i++) {
                CountTuple c = chunkCounts[i];
                vertexStart += c.vertexCount;
                indexStart += c.indexCount;
            }
            meshData.SetVertexBufferParams(count.vertexCount, new NativeArray<VertexAttributeDescriptor>(3, Allocator.Temp) {
                [0] = new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3, stream: 0),
                [1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3, stream: 0),
                [2] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2, stream: 0)
            });
            meshData.GetVertexData<Vertex>().CopyFrom(vertices.GetSubArray(vertexStart, count.vertexCount));
            meshData.SetIndexBufferParams(count.indexCount, IndexFormat.UInt32);
            meshData.GetIndexData<int>().CopyFrom(indices.GetSubArray(indexStart, count.indexCount));
            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, count.indexCount), MeshUpdateFlags.DontRecalculateBounds);
        }
    }
}
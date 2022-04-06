using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace Voxels {
    /// <summary>
    /// Vertex data to be used in the mesh data buffer
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Vertex {
        /// <summary>
        /// The position of the vertex
        /// </summary>
        public readonly float3 pos;
        
        /// <summary>
        /// The normal of the vertex
        /// </summary>
        public readonly float3 n;
        
        /// <summary>
        /// The UV0 coordinate of the vertex
        /// </summary>
        public readonly float2 uv0;

        public Vertex(float3 pos, float3 n, float2 uv0) {
            this.pos = pos;
            this.n = n;
            this.uv0 = uv0;
        }
    }
}
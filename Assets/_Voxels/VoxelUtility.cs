using Unity.Mathematics;
using UnityEngine;

namespace Voxels {
    public static class VoxelUtility {
        /// <summary>
        /// The length of each dimension of a chunk.
        /// </summary>
        public const int CHUNK_SIZE = 32;
        
        /// <summary>
        /// The volume of a chunk.
        /// </summary>
        public const int CHUNK_VOLUME = CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE;
        
        /// <summary>
        /// Stores the directions to check voxels in.
        /// </summary>
        public static readonly int3[] VoxelPositionOffsets = {
            new int3(-1,  0, 0),  // X-
            new int3( 1,  0, 0),  // X+
            new int3( 0, -1, 0),  // Y-
            new int3( 0,  1, 0),  // Y+
            new int3( 0,  0, -1), // Z-
            new int3( 0,  0,  1)  // Z+
        };
        
        /// <summary>
        /// Stores the positions for each vertex of a voxel block.
        /// </summary>
        public static readonly float3[] FaceVertexPositions = {
            float3.zero,
            new float3(0, 0, 1),
            new float3(1, 0, 1),
            new float3(1, 0, 0),
            new float3(0, 1, 0),
            new float3(0, 1, 1),
            new float3(1, 1, 1),
            new float3(1, 1, 0)
        };
        
        /// <summary>
        /// Stores the indices for each vertex of each face of a voxel block.
        /// </summary>
        public static readonly int[] FaceVertexIndices = {
            5, 4, 1, 0, // X-
            7, 6, 3, 2, // X+
            2, 1, 3, 0, // Y-
            7, 4, 6, 5, // Y+
            4, 7, 0, 3, // Z-
            6, 5, 2, 1  // Z+
        };
        
        /// <summary>
        /// Returns an index from a position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static int PositionToIndex(int x, int y, int z) => x + y * CHUNK_SIZE + z * CHUNK_SIZE * CHUNK_SIZE;

        /// <summary>
        /// Returns an index from a position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static int PositionToIndex(Vector3Int position) => PositionToIndex(position.x, position.y, position.z);
        
        /// <summary>
        /// Returns an index from a position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static int PositionToIndex(int3 position) => PositionToIndex(position.x, position.y, position.z);

        /// <summary>
        /// Returns a position from an index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Vector3Int IndexToPositionVector3Int(int index) => new Vector3Int(index % CHUNK_SIZE, index / CHUNK_SIZE % CHUNK_SIZE, index / (CHUNK_SIZE * CHUNK_SIZE));
        
        /// <summary>
        /// Returns a position from an index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int3 IndexToPositionInt3(int index) => new int3(index % CHUNK_SIZE, index / CHUNK_SIZE % CHUNK_SIZE, index / (CHUNK_SIZE * CHUNK_SIZE));

        /// <summary>
        /// Returns true if the position is within the bounds of a chunk.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static bool InBounds(int x, int y, int z) => x >= 0 && x < CHUNK_SIZE && y >= 0 && y < CHUNK_SIZE && z >= 0 && z < CHUNK_SIZE;

        /// <summary>
        /// Returns true if the position is within the bounds of a chunk.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool InBounds(Vector3Int position) => InBounds(position.x, position.y, position.z);
        
        /// <summary>
        /// Returns true if the position is within the bounds of a chunk.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool InBounds(int3 position) => InBounds(position.x, position.y, position.z);

        /// <summary>
        /// Returns true if the index is within the bounds of a chunk.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool InBounds(int index) => index >= 0 && index < CHUNK_VOLUME;
        
        /// <summary>
        /// Convert the world-space position to a chunk position and an index.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="chunkPosition"></param>
        /// <param name="index"></param>
        public static void PositionToChunkPositionAndIndex(Vector3 position, out Vector3Int chunkPosition, out int index) {
            // Floor the position
            Vector3Int flooredPoint = Vector3Int.FloorToInt(position);
            
            // Use proper modulo division to retrieve the position local to the chunk
            Vector3Int localPosition = flooredPoint;
            localPosition.x %= CHUNK_SIZE;
            if (localPosition.x < 0) localPosition.x += CHUNK_SIZE;
            localPosition.y %= CHUNK_SIZE;
            if (localPosition.y < 0) localPosition.y += CHUNK_SIZE;
            localPosition.z %= CHUNK_SIZE;
            if (localPosition.z < 0) localPosition.z += CHUNK_SIZE;
            
            // Set the voxel index
            index = PositionToIndex(localPosition);
            
            // If the component is negative, subtract by one to avoid truncation problems
            if (flooredPoint.x < 0) flooredPoint.x -= CHUNK_SIZE;
            if (flooredPoint.y < 0) flooredPoint.y -= CHUNK_SIZE;
            if (flooredPoint.z < 0) flooredPoint.z -= CHUNK_SIZE;
            
            // Set the chunk position
            chunkPosition = flooredPoint / CHUNK_SIZE;
        }
    }
}
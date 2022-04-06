using System.Collections.Generic;
using UnityEngine;

namespace Voxels {
    public class VoxelWorld : MonoBehaviour {
        private Dictionary<Vector3Int, VoxelChunk> _chunks;

        private void Awake() => _chunks = new Dictionary<Vector3Int, VoxelChunk>();

        private void Start() {
            // Temporary: set a single voxel
            if (VoxelManager.Singleton) {
                SetVoxel(Vector3.zero, new Voxel(1));
                return;
            }
            
            // Destroy if no VoxelManager exists
            Destroy(this);
        }

        /// <summary>
        /// Returns true if a voxel was found.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="voxel"></param>
        /// <returns></returns>
        public bool TryGetVoxel(Vector3 position, out Voxel voxel) {
            VoxelUtility.PositionToChunkPositionAndIndex(position, out Vector3Int chunkPosition, out int index);
            if (!_chunks.TryGetValue(chunkPosition, out VoxelChunk chunk)) {
                voxel = Voxel.Null;
                return false;
            }
            voxel = chunk.Data[index];
            return true;
        }

        /// <summary>
        /// Returns true if the voxel was able to be placed at the position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="voxel"></param>
        /// <returns></returns>
        public bool SetVoxel(Vector3 position, Voxel voxel) {
            // Get the chunk position and index
            VoxelUtility.PositionToChunkPositionAndIndex(position, out Vector3Int chunkPosition, out int index);
            
            // Create the chunk if necessary
            if (!_chunks.TryGetValue(chunkPosition, out VoxelChunk chunk)) {
                GameObject go = new GameObject($"({chunkPosition.x}, {chunkPosition.y}, {chunkPosition.z})", typeof(MeshCollider), typeof(MeshFilter), typeof(MeshRenderer));
                go.transform.SetParent(transform);
                go.transform.localPosition = chunkPosition * VoxelUtility.CHUNK_SIZE;
                go.transform.localRotation = Quaternion.identity;
                chunk = go.AddComponent<VoxelChunk>();
                chunk.World = this;
                _chunks[chunkPosition] = chunk;
            }
            
            // Validate the voxel placement
            Voxel currentVoxel = chunk.Data[index];
            
            if (currentVoxel == voxel) return false;
            
            //
            // EXTEND THIS FOR EXTRA VALIDATION!
            //
            
            // Place the voxel and mark the chunk as updated
            chunk.Data[index] = voxel;
            VoxelManager.Singleton.EnqueueChunkToUpdate(chunk);
            return true;
        }
    }
}
using UnityEngine;
using UnityEngine.Rendering;

namespace Voxels {
    [DisallowMultipleComponent, RequireComponent(typeof(MeshCollider)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]
    public class VoxelChunk : MonoBehaviour {
        private MeshRenderer _renderer;
        private MeshFilter _filter;
        private MeshCollider _collider;
        
        /// <summary>
        /// The world this chunk belongs to.
        /// </summary>
        public VoxelWorld World { get; internal set; }
        
        /// <summary>
        /// The voxel data for this chunk.
        /// </summary>
        internal Voxel[] Data { get; private set; }
        
        /// <summary>
        /// The mesh used for rendering and collision.
        /// </summary>
        internal Mesh Mesh { get; private set; }

        private void Awake() {
            // Instantiate the voxel data
            Data = new Voxel[VoxelUtility.CHUNK_VOLUME];
            
            // Instantiate the mesh
            Mesh = new Mesh {
                indexFormat = IndexFormat.UInt32,
                bounds = new Bounds(Vector3.one * VoxelUtility.CHUNK_SIZE / 2, Vector3.one * VoxelUtility.CHUNK_SIZE) 
            };
            Mesh.MarkDynamic();
            
            // Initialize the renderer
            _renderer = GetComponent<MeshRenderer>();
            _renderer.sharedMaterial = VoxelManager.Singleton.BaseMaterial;
            
            // Initialize the filter
            _filter = GetComponent<MeshFilter>();
            _filter.sharedMesh = Mesh;
            
            // Get the collider
            _collider = GetComponent<MeshCollider>();
        }

        /// <summary>
        /// Update the mesh collider.
        /// </summary>
        internal void UpdateCollider() => _collider.sharedMesh = Mesh;
    }
}
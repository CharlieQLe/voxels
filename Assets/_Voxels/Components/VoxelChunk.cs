using UnityEngine;
using UnityEngine.Rendering;

namespace Voxels {
    [DisallowMultipleComponent, RequireComponent(typeof(MeshCollider)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]
    public class VoxelChunk : MonoBehaviour {
        private MeshRenderer _renderer;
        private MeshFilter _filter;
        private MeshCollider _collider;
        
        internal Voxel[] Data { get; private set; }
        internal Mesh Mesh { get; private set; }
        public VoxelWorld World { get; internal set; }

        private void Awake() {
            Data = new Voxel[VoxelUtility.CHUNK_VOLUME];
            Mesh = new Mesh {
                indexFormat = IndexFormat.UInt32,
                bounds = new Bounds(Vector3.one * VoxelUtility.CHUNK_SIZE / 2, Vector3.one * VoxelUtility.CHUNK_SIZE) 
            };
            Mesh.MarkDynamic();
            _renderer = GetComponent<MeshRenderer>();
            _renderer.sharedMaterial = VoxelManager.Singleton.BaseMaterial;
            _filter = GetComponent<MeshFilter>();
            _filter.sharedMesh = Mesh;
            _collider = GetComponent<MeshCollider>();
        }

        internal void UpdateCollider() => _collider.sharedMesh = Mesh;
    }
}
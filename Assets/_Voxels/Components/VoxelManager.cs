using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Voxels {
    public class VoxelManager : MonoBehaviour {
        /// <summary>
        /// The single VoxelManager instance.
        /// </summary>
        public static VoxelManager Singleton { get; private set; }
        
        /// <summary>
        /// The number of chunks to update at a time.
        /// </summary>
        [SerializeField, Min(1)] private int batchSize = 5;
        
        /// <summary>
        /// The base material of the chunk meshes.
        /// </summary>
        [field: SerializeField] public Material BaseMaterial { get; private set; }
        
        private bool _working;
        private bool _allocatedMeshData;
        private List<Mesh> _meshesToUpdate;
        private List<VoxelChunk> _updatedChunks;
        private UniqueQueue<VoxelChunk> _chunksAwaitingUpdate;
        private JobHandle _job;
        private Mesh.MeshDataArray _meshDataArray;
        private NativeList<Voxel> _batchedChunks;
        private NativeList<CountTuple> _chunkCounts;
        private NativeList<Vertex> _vertices;
        private NativeList<int> _indices;
        private NativeList<int> _meshIds;

        private void Awake() {
            // Setup singleton
            if (Singleton) {
                Destroy(this);
                return;
            }
            Singleton = this;
            DontDestroyOnLoad(this);
            
            // Instantiate and allocate data
            _meshesToUpdate = new List<Mesh>();
            _updatedChunks = new List<VoxelChunk>();
            _chunksAwaitingUpdate = new UniqueQueue<VoxelChunk>();
            _batchedChunks = new NativeList<Voxel>(batchSize * VoxelUtility.CHUNK_VOLUME, Allocator.Persistent);
            _chunkCounts = new NativeList<CountTuple>(batchSize, Allocator.Persistent);
            _vertices = new NativeList<Vertex>(Allocator.Persistent);
            _indices = new NativeList<int>(Allocator.Persistent);
            _meshIds = new NativeList<int>(batchSize, Allocator.Persistent);
        }

        private void OnDestroy() {
            // Handle singleton
            if (!ReferenceEquals(Singleton, this)) return;
            Singleton = null;
            
            // Complete the job then dispose all unmanaged data
            _job.Complete();
            _batchedChunks.Dispose();
            _chunkCounts.Dispose();
            _vertices.Dispose();
            _indices.Dispose();
            if (_allocatedMeshData) _meshDataArray.Dispose();
        }

        private void FixedUpdate() {
            // Start updating chunks if there is no update going on
            if (!_working && _chunksAwaitingUpdate.Count > 0) StartCoroutine(UpdateChunks());
        }

        private IEnumerator UpdateChunks() {
            _working = true;
            while (_chunksAwaitingUpdate.Count > 0) {
                // Get the batch size
                int size = Math.Min(batchSize, _chunksAwaitingUpdate.Count);
                
                // Prepare the data
                _meshesToUpdate.Clear();
                _updatedChunks.Clear();
                _vertices.Clear();
                _indices.Clear();
                _meshIds.Clear();
                _batchedChunks.ResizeUninitialized(size * VoxelUtility.CHUNK_VOLUME);
                _chunkCounts.ResizeUninitialized(size);
                for (int i = 0; i < size; i++) {
                    VoxelChunk chunk = _chunksAwaitingUpdate.Dequeue();
                    _batchedChunks.AsArray().GetSubArray(i * VoxelUtility.CHUNK_VOLUME, VoxelUtility.CHUNK_VOLUME).CopyFrom(chunk.Data);
                    _meshesToUpdate.Add(chunk.Mesh);
                    _updatedChunks.Add(chunk);
                    _meshIds.Add(chunk.Mesh.GetInstanceID());
                }
                _allocatedMeshData = true;
                _meshDataArray = Mesh.AllocateWritableMeshData(size);
                
                // Schedule the jobs to generate and set the mesh
                GenerateMeshJob generateMeshJob = new GenerateMeshJob {
                    batchedChunks = _batchedChunks,
                    chunkCounts = _chunkCounts,
                    vertices = _vertices,
                    indices = _indices
                };
                SetMeshJob setMeshJob = new SetMeshJob {
                    meshDataArray = _meshDataArray,
                    chunkCounts = _chunkCounts,
                    vertices = _vertices.AsDeferredJobArray(),
                    indices = _indices.AsDeferredJobArray()
                };
                _job = generateMeshJob.Schedule();
                _job = setMeshJob.ScheduleParallel(size, size, _job);
                
                // Wait for job completion
                while (!_job.IsCompleted) yield return null;
                _job.Complete();
                
                // Set the meshes
                Mesh.ApplyAndDisposeWritableMeshData(_meshDataArray, _meshesToUpdate);
                _allocatedMeshData = false;
                
                // Schedule the job to bake the mesh collider 
                BakeMeshJob bakeMeshJob = new BakeMeshJob {ids = _meshIds};
                _job = bakeMeshJob.ScheduleParallel(size, size, default);
                
                // Wait for job completion
                while (!_job.IsCompleted) yield return null;
                _job.Complete();
                
                // Update the colliders
                for (int i = 0; i < _updatedChunks.Count; i++) _updatedChunks[i].UpdateCollider();
            }
            _working = false;
        }

        internal void EnqueueChunkToUpdate(VoxelChunk chunk) => _chunksAwaitingUpdate.Enqueue(chunk);
    }
}
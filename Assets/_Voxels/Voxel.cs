using JetBrains.Annotations;

namespace Voxels {
    /// <summary>
    ///
    ///     Extend this for extra data!
    /// 
    /// </summary>
    public struct Voxel {
        public static Voxel Null => new Voxel(0);
        
        public readonly byte id;

        internal Voxel(byte id) {
            this.id = id;
        }

        public override bool Equals([CanBeNull] object obj) => obj is Voxel v && Equals(v);

        public bool Equals(Voxel v) => id == v.id;

        public static bool operator ==(Voxel lhs, Voxel rhs) => lhs.Equals(rhs);

        public static bool operator !=(Voxel lhs, Voxel rhs) => !(lhs == rhs);
    }
}
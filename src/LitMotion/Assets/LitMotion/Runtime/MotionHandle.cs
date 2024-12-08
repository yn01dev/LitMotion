using System;

namespace LitMotion
{
    /// <summary>
    /// An identifier that represents a specific motion entity.
    /// </summary>
    public struct MotionHandle : IEquatable<MotionHandle>
    {
        /// <summary>
        /// The ID of motion storage.
        /// </summary>
        public int StorageId;

        /// <summary>
        /// The ID of motion entity.
        /// </summary>
        public int Index;

        /// <summary>
        /// The generational version of motion entity.
        /// </summary>
        public int Version;

        /// <summary>
        /// Motion time
        /// </summary>
        public readonly double Time
        {
            get
            {
                return MotionManager.GetDataRef(this, MotionStoragePermission.Admin).Time;
            }
            set
            {
                MotionManager.SetTime(this, value, MotionStoragePermission.User);
            }
        }

        /// <summary>
        /// The total duration of the motion
        /// </summary>
        public readonly double Duration
        {
            get
            {
                return MotionHelper.GetTotalDuration(ref MotionManager.GetDataRef(this, MotionStoragePermission.Admin));
            }
        }

        /// <summary>
        /// The number of loops completed
        /// </summary>
        public readonly int ComplatedLoops
        {
            get
            {
                return MotionManager.GetDataRef(this, MotionStoragePermission.User).ComplpetedLoops;
            }
        }

        /// <summary>
        /// Motion playback speed.
        /// </summary>
        public readonly float PlaybackSpeed
        {
            get
            {
                return MotionManager.GetDataRef(this, MotionStoragePermission.User).PlaybackSpeed;
            }
            set
            {
                MotionManager.GetDataRef(this, MotionStoragePermission.User).PlaybackSpeed = value;
            }
        }

        public override readonly string ToString()
        {
            return $"MotionHandle`{StorageId} ({Index}:{Version})";
        }

        public readonly bool Equals(MotionHandle other)
        {
            return Index == other.Index && Version == other.Version && StorageId == other.StorageId;
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is MotionHandle handle) return Equals(handle);
            return false;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Index, Version, StorageId);
        }

        public static bool operator ==(MotionHandle a, MotionHandle b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(MotionHandle a, MotionHandle b)
        {
            return !(a == b);
        }
    }
}
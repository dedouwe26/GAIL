using System.Numerics;

namespace GAIL.Core
{
    /// <summary>
    /// Contains rotation, scale and translation for 3D / 2D objects.
    /// </summary>
    public class Transform : IEquatable<Transform> {
        Vector3 translation = Vector3.Zero;
        Vector3 scale = Vector3.One;
        Quaternion rotation = Quaternion.Identity;
        public Matrix4x4 ToModelMatrix() {
            return Matrix4x4.CreateFromQuaternion(rotation) * Matrix4x4.CreateScale(scale) * Matrix4x4.CreateTranslation(translation);
        }
        public bool Equals(Transform? other) {
            if (other == null) { return false; }
            return translation == other.translation && scale == other.scale && rotation == other.rotation;
        }
        public override bool Equals(object? obj) { return Equals(obj as Transform); }
        public override int GetHashCode() { throw new NotImplementedException(); }
    }
}
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
        /// <summary>
        /// Creates a model matrix for a model with this transform.
        /// </summary>
        /// <returns>The created model matrix.</returns>
        public Matrix4x4 ToModelMatrix() {
            return Matrix4x4.CreateFromQuaternion(rotation) * Matrix4x4.CreateScale(scale) * Matrix4x4.CreateTranslation(translation);
        }
        /// <inheritdoc/>
        public bool Equals(Transform? other) {
            if (other == null) { return false; }
            return translation == other.translation && scale == other.scale && rotation == other.rotation;
        }
        /// <inheritdoc/>
        public override bool Equals(object? obj) { return Equals(obj as Transform); }
        /// <inheritdoc/>
        public override int GetHashCode() { return translation.GetHashCode()+scale.GetHashCode()+rotation.GetHashCode(); }
    }
}
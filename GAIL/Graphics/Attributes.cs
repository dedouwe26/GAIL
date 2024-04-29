using System.Numerics;
using GAIL.Core;

namespace GAIL.Graphics
{
    /// <summary>
    /// A basic per-vertex color attribute (Float4).
    /// </summary>
    /// <remarks>
    /// Creates a color attribute (Float4).
    /// </remarks>
    /// <param name="color">The color.</param>
    public class ColorAttribute(Color color) : VertexAttribute(AttributeType.Float4) {
        /// <summary>
        /// The color of this color attribute.
        /// </summary>
        public Color color = color;

        /// <inheritdoc/>
        public override byte[] Use() {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// A basic per-vertex normal attribute (Float3).
    /// </summary>
    /// <remarks>
    /// Creates a normal attribute.
    /// </remarks>
    /// <param name="normal">The normal vector.</param>
    public class NormalAttribute(Vector3 normal) : VertexAttribute(AttributeType.Float3) {
        /// <summary>
        /// The normal vector of this normal attribute.
        /// </summary>
        public Vector3 normal = normal;

        /// <inheritdoc/>
        public override byte[] Use() {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// A basic per-vertex uv (texture coordinates) attribute (Float2).
    /// </summary>
    /// <remarks>
    /// Creates a uv (texture coordinates) attribute (Float2).
    /// </remarks>
    /// <param name="uv">The uv vector.</param>
    public class UVAttribute(Vector2 uv) : VertexAttribute(AttributeType.Float2) {
        /// <summary>
        /// The uv vector of this uv attribute.
        /// </summary>
        public Vector2 uv = uv;

        /// <inheritdoc/>
        public override byte[] Use() {
            throw new NotImplementedException();
        }
    }
}
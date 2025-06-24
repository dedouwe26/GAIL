using System.Numerics;
using GAIL.Core;

namespace GAIL.Graphics.Mesh
{
    /// <summary>
    /// A basic position attribute (Float3).
    /// </summary>
    /// <param name="position">The position of this attribute.</param>
    public class PositionAttribute(Vector3 position) : VertexAttribute(FormatType.Float3) {
        /// <summary>
        /// The color of this color attribute.
        /// </summary>
        public Vector3 position = position;

        /// <inheritdoc/>
        public override byte[] Use() {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// A basic per-vertex color attribute (Float4).
    /// </summary>
    /// <remarks>
    /// Creates a color attribute (Float4).
    /// </remarks>
    /// <param name="color">The color of this attribute.</param>
    public class ColorAttribute(Color color) : VertexAttribute(FormatType.Float4) {
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
    /// <param name="normal">The normal vector of this attribute.</param>
    public class NormalAttribute(Vector3 normal) : VertexAttribute(FormatType.Float3) {
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
    /// <param name="uv">The uv vector of this attribute.</param>
    public class UVAttribute(Vector2 uv) : VertexAttribute(FormatType.Float2) {
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
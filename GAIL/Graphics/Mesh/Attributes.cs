using System.Numerics;
using GAIL.Core;

namespace GAIL.Graphics.Mesh
{
    // TODO:
    /// <summary>
    /// A basic position attribute (Float3).
    /// </summary>
    /// <param name="position">The position of this attribute.</param>
    public class PositionAttribute(Vector3 position) : VertexAttribute(new(FormatType.Float3, 3*sizeof(float))) {
        /// <summary>
        /// The color of this color attribute.
        /// </summary>
        public Vector3 position = position;

        /// <inheritdoc/>
        public override byte[] Use() {
			return [.. BitConverter.GetBytes(position.X), .. BitConverter.GetBytes(position.Y), .. BitConverter.GetBytes(position.Z)];
		}
    }
    /// <summary>
    /// A basic per-vertex color attribute (Float4).
    /// </summary>
    /// <remarks>
    /// Creates a color attribute (Float4).
    /// </remarks>
    /// <param name="color">The color of this attribute.</param>
    public class ColorAttribute(Color color) : VertexAttribute(new(FormatType.Float4, 4*sizeof(float))) {
        /// <summary>
        /// The color of this color attribute.
        /// </summary>
        public Color color = color;

        /// <inheritdoc/>
        public override byte[] Use() {
			return [.. BitConverter.GetBytes(color.R), .. BitConverter.GetBytes(color.G), .. BitConverter.GetBytes(color.B), .. BitConverter.GetBytes(color.A)];
        }
    }
    /// <summary>
    /// A basic per-vertex normal attribute (Float3).
    /// </summary>
    /// <remarks>
    /// Creates a normal attribute.
    /// </remarks>
    /// <param name="normal">The normal vector of this attribute.</param>
    public class NormalAttribute(Vector3 normal) : VertexAttribute(new(FormatType.Float3, 3*sizeof(float))) {
        /// <summary>
        /// The normal vector of this normal attribute.
        /// </summary>
        public Vector3 normal = normal;

        /// <inheritdoc/>
        public override byte[] Use() {
			return [.. BitConverter.GetBytes(normal.X), .. BitConverter.GetBytes(normal.Y), .. BitConverter.GetBytes(normal.Z)];
        }
    }
    /// <summary>
    /// A basic per-vertex uv (texture coordinates) attribute (Float2).
    /// </summary>
    /// <remarks>
    /// Creates a uv (texture coordinates) attribute (Float2).
    /// </remarks>
    /// <param name="uv">The uv vector of this attribute.</param>
    public class UVAttribute(Vector2 uv) : VertexAttribute(new(FormatType.Float2, 2*sizeof(float))) {
        /// <summary>
        /// The uv vector of this uv attribute.
        /// </summary>
        public Vector2 uv = uv;

        /// <inheritdoc/>
        public override byte[] Use() {
			return [.. BitConverter.GetBytes(uv.X), .. BitConverter.GetBytes(uv.Y)];
        }
    }
}
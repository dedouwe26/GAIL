using GAIL.Core.File;
using OxDED.Terminal.Logging;
using Silk.NET.GLFW;

namespace GAIL.Core
{
    /// <summary>
    /// An image or texture.
    /// </summary>
    public class Texture : IEquatable<Texture> {
        /// <summary>
        /// The width of this texture.
        /// </summary>
        public readonly uint Width;
        /// <summary>
        /// The height of this texture;
        /// </summary>
        public readonly uint Height;
        /// <summary>
        /// Format: each row has the length of the width. Height is the amount of rows.
        /// Left-to-right, top-to-bottom.
        /// </summary>
        public Color[] colors;

        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        public Texture(uint width, uint height) {
            Width = width;
            Height = height;
            colors = new Color[width * height];
        }

        /// <summary>
        /// Returns the color at the given position.
        /// </summary>
        /// <param name="x">The x (horizontal) coordinate.</param>
        /// <param name="y">The y (vertical) coordinate.</param>
        /// <returns>The color at the given position.</returns>
        /// <exception cref="IndexOutOfRangeException">outside file</exception>
        public Color GetColor(uint x, uint y) {
            if (x >= Width||y >= Height) { throw new IndexOutOfRangeException(); }
            return colors[(y*Width)+x];
        }

        /// <summary>
        /// Creates a texture from an PNG file.
        /// </summary>
        /// <param name="path">The path to the PNG file.</param>
        /// <returns>The texture of the PNG file.</returns>
        public static Texture FromPNG(string path) {
            return PNG.Parse(path);
        }
        /// <summary>
        /// Creates a GLFW image from this texture (RGBa).
        /// </summary>
        /// <returns>The GLFW image for this texture (RGBa).</returns>
        public Image ToGLFW() {
            List<byte> byteList = [];
            foreach (Color color in colors) {
                byteList = [.. byteList, .. color.ToBytes()];
            }
            byte[] bytes = [.. byteList];
            return new Image{Width = (int)Width, Height = (int)Height, Pixels = Pointer<byte>.FromArray(ref bytes)};
        }

        /// <inheritdoc/>
        public bool Equals(Texture? other) {
            if (other is null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            if (GetType() != other.GetType()) { return false; }
            
            return colors==other.colors;
        }
        /// <inheritdoc/>
        public override bool Equals(object? obj) {
            return Equals(obj as Texture);
        }
        
        ///
        public static bool operator ==(Texture? a, Texture? b) {
            if (a is null&&b is null) { return true; }

            return a?.Equals(b) ?? false;
        }
        ///
        public static bool operator !=(Texture? a, Texture? b) {
            return !(a==b);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            return colors.GetHashCode();
        }
    }
}
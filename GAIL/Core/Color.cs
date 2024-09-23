namespace GAIL.Core
{
    /// <summary>
    /// Represents a color with RGBa.
    /// </summary>
    public class Color : IEquatable<Color> {
        /// <summary>
        /// The red component of this color (0-255).
        /// </summary>
        public byte r;
        /// <summary>
        /// The green component of this color (0-255).
        /// </summary>
        public byte g;
        /// <summary>
        /// The blue component of this color (0-255).
        /// </summary>
        public byte b;
        /// <summary>
        /// The alpha value of this color (0-255).
        /// </summary>
        public byte a;

        /// <summary>
        /// The red component of this color (0-1).
        /// </summary>
        public float R { get => r/255; set => r=Convert.ToByte(Math.Round(value*255)); }
        /// <summary>
        /// The green component of this color (0-1).
        /// </summary>
        public float G { get => g/255; set => g=Convert.ToByte(Math.Round(value*255)); }
        /// <summary>
        /// The blue component of this color (0-1).
        /// </summary>
        public float B { get => b/255; set => b=Convert.ToByte(Math.Round(value*255)); }
        /// <summary>
        /// The alpha value of this color (0-1).
        /// </summary>
        public float A { get => a/255; set => a=Convert.ToByte(Math.Round(value*255)); }
        /// <summary>
        /// Creates a color from RGBa.
        /// </summary>
        /// <param name="r">The red component between 0-1.</param>
        /// <param name="g">The green component between 0-1.</param>
        /// <param name="b">The blue component between 0-1.</param>
        /// <param name="a">The alpha value between 0-1.</param>
        public Color(float r, float g, float b, float a = 1) {
            R=r;
            G=g;
            B=b;
            A=a;
        }
        /// <summary>
        /// Creates a color from RGBa.
        /// </summary>
        /// <param name="r">The red component between 0-255.</param>
        /// <param name="g">The green component between 0-255.</param>
        /// <param name="b">The blue component between 0-255.</param>
        /// <param name="a">The alpha value between 0-1.</param>
        public Color(byte r, byte g, byte b, byte a = 255) {
            this.r=r;
            this.g=g;
            this.b=b;
            this.a=a;
        }
        /// <summary>
        /// Returns a byte array from the RGBA components.
        /// </summary>
        /// <returns>A byte array from the RGBA components.</returns>
        public byte[] ToBytes() {
            return [.. ToBytesRGB(), a];
        }
        /// <summary>
        /// Returns a byte array from the RGB components.
        /// </summary>
        /// <returns>A byte array from the RGB components.</returns>
        public byte[] ToBytesRGB() {
            return [r, g, b];
        }

        /// <inheritdoc/>
        public bool Equals(Color? other) {
            if (other == null) { return false; }
            return (r==other.r)&&(g==other.g)&&(b==other.b)&&(a==other.a);
        }
        /// <inheritdoc/>
        public override bool Equals(object? obj) {
            return Equals(obj as Color);
        }
        
        ///
        public static bool operator ==(Color? a, Color? b) {
            if (a==null&&b==null) { return true; }
            if (a==null) { return false; }
            return a.Equals(b);
        }
        ///
        public static bool operator !=(Color? a, Color? b) {
            if (a==null&&b==null) { return false; }
            if (a==null) { return true; }
            return !a.Equals(b);
        }
        /// <inheritdoc/>
        public override int GetHashCode() { return (r<<24)^(g<<16)^(b<<8)^(a); }
    }
}
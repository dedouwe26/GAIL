namespace GAIL.Core
{
    /// <summary>
    /// Represents a color with RGBa.
    /// </summary>
    public class Color : IEquatable<Color> {
        /// <summary>
        /// The red component of this color (0-1)
        /// </summary>
        public float r;
        /// <summary>
        /// The green component of this color (0-1)
        /// </summary>
        public float g;
        /// <summary>
        /// The blue component of this color (0-1)
        /// </summary>
        public float b;
        /// <summary>
        /// The alpha value of this color (0-1)
        /// </summary>
        public float a;
        /// <summary>
        /// Creates a color from RGBa.
        /// </summary>
        /// <param name="r">The red component between 0-1.</param>
        /// <param name="g">The green component between 0-1.</param>
        /// <param name="b">The blue component between 0-1.</param>
        /// <param name="a">The alpha value between 0-1.</param>
        public Color(float r, float g, float b, float a = 1) {
            this.r=r;
            this.g=g;
            this.b=b;
            this.a=a;
        }
        /// <summary>
        /// Creates a color from RGBa.
        /// </summary>
        /// <param name="r">The red component between 0-255.</param>
        /// <param name="g">The green component between 0-255.</param>
        /// <param name="b">The blue component between 0-255.</param>
        /// <param name="a">The alpha value between 0-1.</param>
        public Color(byte r, byte g, byte b, float a = 1) {
            this.r=r/255;
            this.g=g/255;
            this.b=b/255;
            this.a=a;
        }
        /// <summary>
        /// Returns a byte array from the RGBA components.
        /// </summary>
        /// <returns>A byte array from the RGBA components.</returns>
        public byte[] ToBytes() {
            return [(byte)(r*255), (byte)(g*255), (byte)(b*255), (byte)(a*255)];
        }
        /// <summary>
        /// Returns a byte array from the RGB components.
        /// </summary>
        /// <returns>A byte array from the RGB components.</returns>
        public byte[] ToBytesRGB() {
            return [(byte)(r*255), (byte)(g*255), (byte)(b*255), (byte)(a*255)];
        }

        /// <inheritdoc/>
        public bool Equals(Color? other) {
            if (other == null) { return false; }
            return (r==other.r)&&(g==other.g)&&(b==other.b)&&(a==other.a);
        }
        ///
        public static bool operator ==(Color? a, Color? b) {
            if (a==null) { return false; }
            return a.Equals(b);
        }
        ///
        public static bool operator !=(Color? a, Color? b) {
            if (a==null) { return false; }
            return a.Equals(b);
        }
        /// <inheritdoc/>
        public override bool Equals(object? obj) {
            if (obj==null) { return false; }
            if (obj.GetType()!=typeof(Color)) { return false; }
            return Equals((Color)obj);
        }
        /// <inheritdoc/>
        public override int GetHashCode() { return ((byte)(r*255)) + ((byte)(g*255))*1_000 + ((byte)(b*255))*1_000_000+((int)(a*255))*1_000_000_000; }
    }
}
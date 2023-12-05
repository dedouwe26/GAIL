#pragma once

#include <string>

using string = std::string;

namespace GAIL
{
    // A 4x4 matrix.
    struct Matrix {
        double m00, m10, m20, m30;
        double m01, m11, m21, m31;
        double m02, m12, m22, m32;
        double m03, m13, m23, m33;
        static Matrix Identity;

        public:
            Matrix operator*(Matrix right);
    };
    
    // A structure with 2 components (used for 2D space).
    struct Vector2
    {
        double x;
        double y;
        Vector2 operator+(double b) {return {this->x+b,this->y+b};};
        Vector2 operator-(double b) {return {this->x-b,this->y-b};};
        Vector2 operator*(Vector2 b) {return {this->x*b.x,this->y*b.y};};
    };

    // A structure with 3 components (used for 3D space).
    struct Vector3
    {
        double x;
        double y;
        double z;
        // Creates a 4x4 scale matrix from a vector 3.
        Matrix ToScale();
        // Creates a 4x4 translation matrix from a vector 3.
        Matrix ToTranslation();
        Vector3 operator+(double b) {return {this->x+b,this->y+b,this->z+b};};
        Vector3 operator-(double b) {return {this->x-b,this->y-b,this->z-b};};
        Vector3 operator*(Vector3 b) {return {this->x*b.x,this->y*b.y,this->z*b.z};};
    };
    
    // A structure with 4 components.
    struct Vector4
    {
        double x;
        double y;
        double z;
        double w;
        Vector4 operator+(double b) {return {this->x+b,this->y+b,this->z+b,this->w+b};};
        Vector4 operator-(double b) {return {this->x-b,this->y-b,this->z-b,this->w-b};};
        Vector4 operator*(Vector4 b) {return {this->x*b.x,this->y*b.y,this->z*b.z,this->w*b.w};};
    };

    // A RGBA color structure (normalized).
    struct Color {
        double r;
        double g;
        double b;
        double a;
        // For values between 0-255 (except the alpha channel).
        static Color FromRGBA(unsigned char r, unsigned char g, unsigned char b, double a) {
            return {r/255., g/255., b/255., a};
        }
    };

    // A texture rendered with the GPU.
    struct Texture
    {
        int width;
        int height;
        Color colors[];
        // Returns the color on that coordinate.
        Color GetColor(Vector2 coord);
        // Creates a texture from a png file.
        static Texture FromPNG(string path);
        // Creates a texture form a jpg file
        static Texture FromJPEG(string path);
    };
    
    
    // A 3D Rotation quaternion
    struct Quaternion 
    {
        double x;
        double y;
        double z;
        double w;
        static Quaternion Identity;
        // Creates a 4x4 rotation matrix from a quaternion.
        Matrix ToRotationMatrix();
    };

    // Contains rotation, scale and translation for 3D / 2D objects.
    struct Transform
    {
        Vector3 translation;
        Quaternion rotation;
        Vector3 scale;
        // Creates a view matrix.
        Matrix ToViewMatrix();
    };
} // namespace GAIL
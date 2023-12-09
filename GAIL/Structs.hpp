#pragma once

// PI
#define PI 3.14159265358979323846

#include <string>

using string = std::string;

namespace GAIL
{
    // Turns degrees into radians.
    double ToRadians(double degree) {return degree * (PI/180);};

    // A 4x4 matrix.
    struct Matrix {
        double m00, m10, m20, m30;
        double m01, m11, m21, m31;
        double m02, m12, m22, m32;
        double m03, m13, m23, m33;
        static Matrix Identity;

        static Matrix FromView();
        // Creates a Matrix Projection from perspective, FoV in radians: like 90 degrees, aspectRatio of screen:  4/3 or 1000/6000, 
        // nearClippingPlane from camera: the closest that a object can be, farClippingPlane from : 
        static Matrix fromPerspective(double FoV, double aspectRatio, double nearClippingPlane = .1, double farClippingPlane = 100.);
        static Matrix fromOrthographic();

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
        bool operator==(Vector2 b) {return this->x==b.x && this->y==b.y;}
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
        bool operator==(Vector3 b) {return this->x==b.x && this->y==b.y && this->z==b.z;}
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
        bool operator==(Vector4 b) {return this->x==b.x && this->y==b.y && this->z==b.z && this->w==b.w;}
    };

    // A RGBA color structure (normalized).
    struct Color {
        double r; // 0-1
        double g; // 0-1
        double b; // 0-1
        double a; // 0-1
        // For values between 0-255 (except the alpha channel: 0-1).
        static Color FromRGBA(unsigned char r, unsigned char g, unsigned char b, double a) {
            return {r/255., g/255., b/255., a};
        }
        bool operator==(Color b) {return this->r==b.r && this->g==b.g && this->b==b.b && this->a==b.a;}
    };

    // A texture rendered with the GPU.
    struct Texture
    {
        int width = 0;
        int height = 0;
        // Format: each row has the length of the width. Height is the amount of rows.
        // 
        // row0 + row1 + row2 + row[height]
        Color *colors[];

        // Returns the color on that coordinate.
        Color* GetColor(Vector2 coord) {return colors[int (coord.x+this->width*coord.y)];};
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

        Vector4 operator+(double b) {return {this->x+b,this->y+b,this->z+b,this->w+b};};
        Vector4 operator-(double b) {return {this->x-b,this->y-b,this->z-b,this->w-b};};
        Vector4 operator*(Vector4 b) {return {this->x*b.x,this->y*b.y,this->z*b.z,this->w*b.w};};
    };

    // Contains rotation, scale and translation for 3D / 2D objects.
    struct Transform
    {
        Vector3 *translation;
        Quaternion *rotation;
        Vector3 *scale;
        // Creates a model matrix.
        Matrix ToModelMatrix();
    };
} // namespace GAIL
#pragma once

#ifndef PI
// PI (𝝅)
#define PI 3.14159265358979323846
#endif

#include "GAIL.hpp"

using string = std::string;

namespace GAIL
{
    // Turns degrees into radians.
    double ToRadians(double degree);

    // A structure with 2 components (used for 2D space).
    struct Vector2
    {
        double x;
        double y;
        Vector2 operator+(double b);
        Vector2 operator-(double b);
        Vector2 operator*(Vector2 b);
        bool operator==(Vector2 b);
    };

    struct Matrix;

    // A structure with 3 components (used for 3D space).
    struct Vector3
    {
        double x;
        double y;
        double z;
        // Creates a 4x4 scale matrix from a vector 3.
        Matrix ToScaleMatrix();
        // Creates a 4x4 translation matrix from a vector 3.
        Matrix ToTranslationMatrix();
        Vector3 operator+(double b);
        Vector3 operator-(double b);
        Vector3 operator*(Vector3 b);
        bool operator==(Vector3 b);
    };
    
    // A structure with 4 components.
    struct Vector4
    {
        double x;
        double y;
        double z;
        double w;
        Vector4 operator+(double b);
        Vector4 operator-(double b);
        Vector4 operator*(Vector4 b);
        bool operator==(Vector4 b);
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

        Quaternion operator+(double b);
        Quaternion operator-(double b);
        Quaternion operator*(Quaternion b);
    };

    // A 4x4 matrix.
    struct Matrix {
        double m00, m10, m20, m30;
        double m01, m11, m21, m31;
        double m02, m12, m22, m32;
        double m03, m13, m23, m33;
        static Matrix Identity;

        // Creates a view matrix that is looking at target. With camera as position of the cam.
        // And up as the world position up.
        static Matrix FromLookAt(Vector3 camera, Vector3 target, Vector3 up = Vector3{0, 1, 0});
        // Creates a view matrix that is at position and rotation.
        static Matrix FromView(Vector3 position, Quaternion rotation);
        // Creates a perspective projection matrix, 
        //FoV in radians: like 90 degrees, 
        // aspectRatio of screen:  4/3 or 1000/6000, 
        // near (near clipping plane) from camera: the closest that a object can be, 
        // far (far clipping plane) from camera : furthest a object can be.
        static Matrix FromPerspective(double FoV, double aspectRatio, double near = .1, double far = 100.);
        // Creates a orthographic projection matrix, left: left distance to the camera, 
        // right: right distance to the camera,
        // bottom: bottom distance to the camera,
        // top: top distance to the camera,
        // near (near clipping plane) from camera: the closest that a object can be, 
        // far (far clipping plane) from camera : furthest a object can be.
        static Matrix FromOrthographic(double left, double right, double bottom, double top, double near, double far);

        Matrix operator*(Matrix right);
    };

    // A RGBA color structure (normalized).
    struct Color {
        double r; // 0-1
        double g; // 0-1
        double b; // 0-1
        double a; // 0-1
        // For values between 0-255 (except the alpha channel: 0-1).
        static Color FromRGBA(unsigned char r, unsigned char g, unsigned char b, double a);
        bool operator==(Color b);
    };

    // A texture rendered with the GPU.
    struct Texture
    {
        int width = 0;
        int height = 0;
        // Format: each row has the length of the width. Height is the amount of rows.
        //
        // row0 + row1 + row2 + row[height]
        std::vector<Color> colors;

        // Returns the color on that coordinate.
        Color* GetColor(Vector2 coord);
        // Creates a texture from a png file.
        static Texture FromPNG(string path);
        // Creates a texture form a jpg file
        static Texture FromJPEG(string path);
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
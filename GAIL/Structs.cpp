#include "GAIL.hpp"
#include <cmath>

namespace GAIL
{
    Vector2 Vector2::operator+(double b) {return {this->x+b,this->y+b};};
    Vector2 Vector2::operator-(double b) {return {this->x-b,this->y-b};};
    Vector2 Vector2::operator*(Vector2 b) {return {this->x*b.x,this->y*b.y};};
    bool Vector2::operator==(Vector2 b) {return this->x==b.x && this->y==b.y;};
    
    Vector3 Vector3::operator+(double b) {return {this->x+b,this->y+b,this->z+b};};
    Vector3 Vector3::operator-(double b) {return {this->x-b,this->y-b,this->z-b};};
    Vector3 Vector3::operator*(Vector3 b) {return {this->x*b.x,this->y*b.y,this->z*b.z};};
    bool Vector3::operator==(Vector3 b) {return this->x==b.x && this->y==b.y && this->z==b.z;};

    Vector4 Vector4::operator+(double b) {return {this->x+b,this->y+b,this->z+b,this->w+b};};
    Vector4 Vector4::operator-(double b) {return {this->x-b,this->y-b,this->z-b,this->w-b};};
    Vector4 Vector4::operator*(Vector4 b) {return {this->x*b.x,this->y*b.y,this->z*b.z,this->w*b.w};};
    bool Vector4::operator==(Vector4 b) {return this->x==b.x && this->y==b.y && this->z==b.z && this->w==b.w;};

    Quaternion Quaternion::operator+(double b) {return {this->x+b,this->y+b,this->z+b,this->w+b};};
    Quaternion Quaternion::operator-(double b) {return {this->x-b,this->y-b,this->z-b,this->w-b};};
    Quaternion Quaternion::operator*(Quaternion b) {return {this->x*b.x,this->y*b.y,this->z*b.z,this->w*b.w};};

    Color Color::FromRGBA(unsigned char r, unsigned char g, unsigned char b, double a) {
        return {r/255., g/255., b/255., a};
    };
    bool Color::operator==(Color b) {return this->r==b.r && this->g==b.g && this->b==b.b && this->a==b.a;};

    Color* Texture::GetColor(Vector2 coord) {return &this->colors[int (coord.x+this->width*coord.y)];};

    double ToRadians(double degree) {return degree * (PI/180);};

    Matrix Matrix::Identity = {1, 0, 0, 0,
                               0, 1, 0, 0,
                               0, 0, 1, 0,
                               0, 0, 0, 1};

    Matrix Vector3::ToScaleMatrix() {
        Matrix result = Matrix::Identity;
        result.m00 = this->x;
        result.m11 = this->y;
        result.m22 = this->z;
        return result;
    }

    Matrix Vector3::ToTranslationMatrix() {
        Matrix result = Matrix::Identity;
        result.m03 = this->x;
        result.m13 = this->y;
        result.m23 = this->z;
        return result;
    }

    Quaternion Quaternion::Identity = {0, 0, 0, 1};

    Matrix Quaternion::ToRotationMatrix() {
        Matrix result = Matrix::Identity;
        double xx = this->x * this->x;
        double yy = this->y * this->y;
        double zz = this->z * this->z;
        double xy = this->x * this->y;
        double wz = this->z * this->w;
        double xz = this->z * this->x;
        double wy = this->y * this->w;
        double yz = this->y * this->z;
        double wx = this->x * this->w;

        result.m00 = (1 - (2 * (yy + zz)));
        result.m10 = (2 * (xy + wz));
        result.m20 = (2 * (xz - wy));

        result.m01 = (2 * (xy - wz));
        result.m11 = (1 - (2 * (zz + xx)));
        result.m21 = (2 * (yz + wx));

        result.m02 = (2 * (xz + wy));
        result.m12 = (2 * (yz - wx));
        result.m22 = (1 - (2 * (yy + xx)));
        return result;
    }

    Matrix Transform::ToModelMatrix() {
        return Matrix::Identity * (*this->rotation).ToRotationMatrix() * (*this->scale).ToScaleMatrix() * (*this->translation).ToTranslationMatrix();
    }
    Matrix Matrix::operator*(Matrix right)
    {
        return {
            this->m00*right.m00, this->m10*right.m10, this->m20*right.m20, this->m30*right.m30,
            this->m01*right.m01, this->m11*right.m11, this->m21*right.m21, this->m31*right.m31,
            this->m02*right.m02, this->m12*right.m12, this->m22*right.m22, this->m32*right.m32,
            this->m03*right.m03, this->m13*right.m13, this->m23*right.m23, this->m33*right.m33
        };
    }

    Matrix Matrix::FromLookAt(Vector3 camera, Vector3 target, Vector3 up) {
        // TODO: Return lookat matrix
    };
    Matrix Matrix::FromView(Vector3 position, Quaternion rotation) {
        // TODO: Return view matrix
    };
    Matrix Matrix::FromPerspective(double FoV, double aspectRatio, double near, double far) {
        double yScale = (1. / (tan((FoV/2))));
        double xScale = (yScale/aspectRatio);

        Matrix result = Matrix::Identity;

        result.m00 = xScale;
        result.m10 = result.m20 = result.m30 = 0;

        result.m11 = yScale;
        result.m01 = result.m21 = result.m31 = 0;

        result.m02 = result.m12 = 0;
        double negFarRange = far==-1. ? -1. : (far/(near-far));
        result.m22 = negFarRange;
        result.m32 = -1;

        result.m03 = result.m13 = result.m33 = 0;
        result.m23 = near*negFarRange;

        return result;
    };
    Matrix Matrix::FromOrthographic(double left, double right, double bottom, double top, double near = .1, double far = 100.) {
        Matrix result = Matrix::Identity;
        result.m00 = 2./(right-left);
        result.m11 = 2./(top-bottom);
        result.m22 = (1. / (near-far));
        result.m03 = (left+right)/(left-right);
        result.m13 = (top+bottom)/(bottom-top);
        result.m23 = near/(near-far);
        return result;
};

    Texture Texture::FromPNG(string path) {
        // TODO: Load texture from PNG file at path
    };
    Texture Texture::FromJPEG(string path) {
        // TODO: Load texture from JPEG file at path
    };
} // namespace GAIL
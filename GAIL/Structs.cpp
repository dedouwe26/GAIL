#include "GAIL.hpp"

namespace GAIL
{
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
} // namespace GAIL
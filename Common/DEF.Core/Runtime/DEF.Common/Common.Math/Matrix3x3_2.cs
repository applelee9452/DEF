using System;
using System.Globalization;

namespace DEF.Math2
{
    [Serializable]
    public struct Matrix3x3_2 : IEquatable<Matrix3x3_2>
    {
        public static readonly Matrix3x3_2 identity = new Matrix3x3_2(1, 0, 0, 0, 1, 0, 0, 0, 1);
        public float m00;
        public float m01;
        public float m02;
       
        public float m10;
        public float m11;
        public float m12;
       
        public float m20;
        public float m21;
        public float m22;
       
       

        public bool isIdentity
        {
            get
            {
                return this.m00 == 1f && this.m11 == 1f && this.m22 == 1f  && // Check diagonal element first for early out.
                this.m12 == 0.0f &&  this.m21 == 0.0f ;
            }
        }

        public Vector2 up
        {
            get
            {
                Vector2 vector3;
                vector3.x = this.m01;
                vector3.y = this.m11;
                return vector3;
            }
            set
            {
                this.m01 = value.x;
                this.m11 = value.y;
            }
        }

        public Vector2 down
        {
            get
            {
                Vector2 vector3;
                vector3.x = -this.m01;
                vector3.y = -this.m11;
                return vector3;
            }
            set
            {
                this.m01 = -value.x;
                this.m11 = -value.y;
            }
        }

        public Vector2 right
        {
            get
            {
                Vector2 vector3;
                vector3.x = this.m00;
                vector3.y = this.m10;
                return vector3;
            }
            set
            {
                this.m00 = value.x;
                this.m10 = value.y;
            }
        }

        public Vector2 left
        {
            get
            {
                Vector2 vector3;
                vector3.x = -this.m00;
                vector3.y = -this.m10;
                return vector3;
            }
            set
            {
                this.m00 = -value.x;
                this.m10 = -value.y;
            }
        }

        public Vector2 forward
        {
            get
            {
                Vector2 vector3;
                vector3.x = -this.m02;
                vector3.y = -this.m12;
                return vector3;
            }
            set
            {
                this.m02 = -value.x;
                this.m12 = -value.y;
            }
        }

        public Vector2 back
        {
            get
            {
                Vector2 vector3;
                vector3.x = this.m02;
                vector3.y = this.m12;
                return vector3;
            }
            set
            {
                this.m02 = value.x;
                this.m12 = value.y;
            }
        }

        public unsafe float this[int row, int col]
        {
            get
            {
                fixed (float* numPtr = &this.m00)
                    return numPtr[row * 4 + col];
            }
            set
            {
                fixed (float* numPtr = &this.m00)
                    numPtr[row * 4 + col] = value;
            }
        }

        public unsafe float this[int index]
        {
            get
            {
                fixed (float* numPtr = &this.m00)
                    return numPtr[index];
            }
            set
            {
                fixed (float* numPtr = &this.m00)
                    numPtr[index] = value;
            }
        }

        public Vector4 GetRow(int index)
        {
            Vector4 vector4;
            vector4.x = this[index, 0];
            vector4.y = this[index, 1];
            vector4.z = this[index, 2];
            vector4.w = this[index, 3];
            return vector4;
        }

        public void SetRow(int index, Vector4 value)
        {
            this[index, 0] = value.x;
            this[index, 1] = value.y;
            this[index, 2] = value.z;
            this[index, 3] = value.w;
        }

        public Vector4 GetColumn(int index)
        {
            Vector4 vector4;
            vector4.x = this[0, index];
            vector4.y = this[1, index];
            vector4.z = this[2, index];
            vector4.w = this[3, index];
            return vector4;
        }

        public void SetColumn(int index, Vector4 value)
        {
            this[0, index] = value.x;
            this[1, index] = value.y;
            this[2, index] = value.z;
            this[3, index] = value.w;
        }

        public Matrix3x3_2(
            float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;
            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;
            this.m20 = m20;
            this.m21 = m21;
            this.m22 = m22;
        }

        public static Matrix3x3_2 CreateTranslation(Vector2 position)
        {
            Matrix3x3_2 matrix44;
            matrix44.m00 = 1f;
            matrix44.m01 = 0.0f;
            matrix44.m02 = position.x;
            matrix44.m10 = 0.0f;
            matrix44.m11 = 1f;
            matrix44.m12 = position.y;
            matrix44.m20 = 0.0f;
            matrix44.m21 = 0.0f;
            matrix44.m22 = 1f;
            return matrix44;
        }

        public Matrix3x3_2 inverse
        {
            get
            {
                return Matrix3x3_2.Invert(this);
            }
        }

        public static void CreateTranslation(ref Vector2 position, out Matrix3x3_2 matrix)
        {
            matrix.m00 = 1f;
            matrix.m01 = 0.0f;
            matrix.m02 =  position.x;
            matrix.m10 = 0.0f;
            matrix.m11 = 1f;
            matrix.m12 = position.y;
            matrix.m20 = 0.0f;
            matrix.m21 = 0.0f;
            matrix.m22 = 1f;
        }

        public static Matrix3x3_2 CreateForward(Vector2 forward)
        {
            Matrix3x3_2 matrix44;
            matrix44.m00 = forward.y;
            matrix44.m01 = forward.x;
            matrix44.m02 = 0.0f;
            matrix44.m10 = -forward.x;
            matrix44.m11 = forward.y;
            matrix44.m12 = 0.0f;
            matrix44.m20 = 0.0f;
            matrix44.m21 = 0.0f;
            matrix44.m22 = 1f;
            return matrix44;
        }
        public static Matrix3x3_2 CreateScale(Vector2 scales)
        {
            Matrix3x3_2 matrix44;
            matrix44.m00 = scales.x;
            matrix44.m01 = 0.0f;
            matrix44.m02 = 0.0f;
            matrix44.m10 = 0.0f;
            matrix44.m11 = scales.y;
            matrix44.m12 = 0.0f;
            matrix44.m20 = 0.0f;
            matrix44.m21 = 0.0f;
            matrix44.m22 = 1f;
            return matrix44;
        }

        public static Matrix3x3_2 TRS(Vector2 pos, Vector2 forward, Vector2 s)
        {
            Matrix3x3_2 m1 = CreateTranslation(pos);
            Matrix3x3_2 m2 = CreateForward(forward);
            Matrix3x3_2 m3 = CreateScale(s);
            return m1 * m2 * m3;
        }
        public static Matrix3x3_2 TR(Vector2 pos, Vector2 forward)
        {
            Matrix3x3_2 m1 = CreateTranslation(pos);
            Matrix3x3_2 m2 = CreateForward(forward);
            return m1 * m2;
        }
        public static Matrix3x3_2 Scale(Vector2 scales)
        {
            Matrix3x3_2 m1;
            CreateScale(ref scales, out m1);
            return m1;
        }

        public static void CreateScale(ref Vector2 scales, out Matrix3x3_2 matrix)
        {
            matrix.m00 = scales.x;
            matrix.m01 = 0.0f;
            matrix.m02 = 0.0f;
            matrix.m10 = 0.0f;
            matrix.m11 = scales.y;
            matrix.m12 = 0.0f;
            matrix.m20 = 0.0f;
            matrix.m21 = 0.0f;
            matrix.m22 = 1f;           
        }

        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return
                    string.Format((IFormatProvider)currentCulture, "{0}, {1}, {2}, {3}; ",
                                  (object)this.m00.ToString((IFormatProvider)currentCulture),
                                  (object)this.m01.ToString((IFormatProvider)currentCulture),
                                  (object)this.m02.ToString((IFormatProvider)currentCulture)) +
                    string.Format((IFormatProvider)currentCulture, "{0}, {1}, {2}, {3}; ",
                                  (object)this.m10.ToString((IFormatProvider)currentCulture),
                                  (object)this.m11.ToString((IFormatProvider)currentCulture),
                                  (object)this.m12.ToString((IFormatProvider)currentCulture)) +
                    string.Format((IFormatProvider)currentCulture, "{0}, {1}, {2}, {3}; ",
                                  (object)this.m20.ToString((IFormatProvider)currentCulture),
                                  (object)this.m21.ToString((IFormatProvider)currentCulture),
                                  (object)this.m22.ToString((IFormatProvider)currentCulture));
        }

        public bool Equals(Matrix3x3_2 other)
        {
            if ((double)this.m00 == (double)other.m00 && (double)this.m11 == (double)other.m11 &&
                ((double)this.m22 == (double)other.m22) &&
                ((double)this.m01 == (double)other.m01 && (double)this.m02 == (double)other.m02 &&
                    ((double)this.m10 == (double)other.m10)) &&
                ((double)this.m12 == (double)other.m12 &&
                    ((double)this.m20 == (double)other.m20 && (double)this.m21 == (double)other.m21) ))
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is Matrix3x3_2)
                flag = this.Equals((Matrix3x3_2)obj);
            return flag;
        }

        public override int GetHashCode()
        {
            return this.m00.GetHashCode() + this.m01.GetHashCode() + this.m02.GetHashCode() +  this.m10.GetHashCode() +
                    this.m11.GetHashCode() + this.m12.GetHashCode() + this.m20.GetHashCode() + this.m21.GetHashCode() +
                    this.m22.GetHashCode() ;
        }

        public static Matrix3x3_2 Transpose(Matrix3x3_2 matrix)
        {
            Matrix3x3_2 matrix44;
            matrix44.m00 = matrix.m00;
            matrix44.m01 = matrix.m10;
            matrix44.m02 = matrix.m20;
            matrix44.m10 = matrix.m01;
            matrix44.m11 = matrix.m11;
            matrix44.m12 = matrix.m21;
            matrix44.m20 = matrix.m02;
            matrix44.m21 = matrix.m12;
            matrix44.m22 = matrix.m22;
            return matrix44;
        }

        public static void Transpose(ref Matrix3x3_2 matrix, out Matrix3x3_2 result)
        {
            result.m00 = matrix.m00;
            result.m01 = matrix.m10;
            result.m02 = matrix.m20;
            result.m10 = matrix.m01;
            result.m11 = matrix.m11;
            result.m12 = matrix.m21;
            result.m20 = matrix.m02;
            result.m21 = matrix.m12;
            result.m22 = matrix.m22;
        }



        public static Matrix3x3_2 Invert(Matrix3x3_2 matrix)
        {
            float m00 = matrix.m00;
            float m10 = matrix.m10;
            float m20 = matrix.m20;

            float m01 = matrix.m01;
            float m11 = matrix.m11;
            float m21 = matrix.m21;
            float m02 = matrix.m02;
            float m12 = matrix.m12;
            float m22 = matrix.m22;

            Matrix3x3_2 matrix44;
           
            matrix44.m00 = (m11*m22-m12*m21);
            matrix44.m10 = -(m10*m22-m12*m20);
            matrix44.m20 = (m10*m21-m11*m20);

            matrix44.m01 = -(m01*m22-m02*m21);
            matrix44.m11 = (m00*m22-m02*m20);
            matrix44.m21 = -(m00*m21-m01*m20);

            matrix44.m02 = (m01*m12-m02*m11);
            matrix44.m12 = -(m00*m12-m02*m10);
            matrix44.m22 = (m00*m11-m01*m10);

            return matrix44;
        }

        public static void Invert(ref Matrix3x3_2 matrix, out Matrix3x3_2 result)
        {
            float m00 = matrix.m00;
            float m10 = matrix.m10;
            float m20 = matrix.m20;
            float m01 = matrix.m01;
            float m11 = matrix.m11;
            float m21 = matrix.m21;
            float m02 = matrix.m02;
            float m12 = matrix.m12;
            float m22 = matrix.m22;

            result.m00 = m11*m22-m12*m21;
            result.m01 = -(m10*m22-m12*m20);
            result.m02 = m10*m21-m11*m20;

            result.m10 = -(m01*m22-m02*m21);
            result.m11 = m00*m22-m02*m20;
            result.m12 = -(m00*m21-m01*m20);

            result.m20 = m01*m12-m02*m11;
            result.m21 = -(m00*m12-m02*m10);
            result.m22 = m00*m11-m01*m10;
        }

        public static Matrix3x3_2 Add(Matrix3x3_2 matrix1, Matrix3x3_2 matrix2)
        {
            Matrix3x3_2 matrix44;
            matrix44.m00 = matrix1.m00 + matrix2.m00;
            matrix44.m01 = matrix1.m01 + matrix2.m01;
            matrix44.m02 = matrix1.m02 + matrix2.m02;          
            matrix44.m10 = matrix1.m10 + matrix2.m10;
            matrix44.m11 = matrix1.m11 + matrix2.m11;
            matrix44.m12 = matrix1.m12 + matrix2.m12;           
            matrix44.m20 = matrix1.m20 + matrix2.m20;
            matrix44.m21 = matrix1.m21 + matrix2.m21;
            matrix44.m22 = matrix1.m22 + matrix2.m22;           
            return matrix44;
        }

        public static void Add(ref Matrix3x3_2 matrix1, ref Matrix3x3_2 matrix2, out Matrix3x3_2 result)
        {
            result.m00 = matrix1.m00 + matrix2.m00;
            result.m01 = matrix1.m01 + matrix2.m01;
            result.m02 = matrix1.m02 + matrix2.m02;
            result.m10 = matrix1.m10 + matrix2.m10;
            result.m11 = matrix1.m11 + matrix2.m11;
            result.m12 = matrix1.m12 + matrix2.m12;
            result.m20 = matrix1.m20 + matrix2.m20;
            result.m21 = matrix1.m21 + matrix2.m21;
            result.m22 = matrix1.m22 + matrix2.m22;
        }

        public static Matrix3x3_2 Sub(Matrix3x3_2 matrix1, Matrix3x3_2 matrix2)
        {
            Matrix3x3_2 matrix44;
            matrix44.m00 = matrix1.m00 - matrix2.m00;
            matrix44.m01 = matrix1.m01 - matrix2.m01;
            matrix44.m02 = matrix1.m02 - matrix2.m02;
            matrix44.m10 = matrix1.m10 - matrix2.m10;
            matrix44.m11 = matrix1.m11 - matrix2.m11;
            matrix44.m12 = matrix1.m12 - matrix2.m12;
            matrix44.m20 = matrix1.m20 - matrix2.m20;
            matrix44.m21 = matrix1.m21 - matrix2.m21;
            matrix44.m22 = matrix1.m22 - matrix2.m22;
            return matrix44;
        }

        public static void Sub(ref Matrix3x3_2 matrix1, ref Matrix3x3_2 matrix2, out Matrix3x3_2 result)
        {
            result.m00 = matrix1.m00 - matrix2.m00;
            result.m01 = matrix1.m01 - matrix2.m01;
            result.m02 = matrix1.m02 - matrix2.m02;
            result.m10 = matrix1.m10 - matrix2.m10;
            result.m11 = matrix1.m11 - matrix2.m11;
            result.m12 = matrix1.m12 - matrix2.m12;
            result.m20 = matrix1.m20 - matrix2.m20;
            result.m21 = matrix1.m21 - matrix2.m21;
            result.m22 = matrix1.m22 - matrix2.m22;
        }

        public static Matrix3x3_2 Multiply(Matrix3x3_2 matrix1, Matrix3x3_2 matrix2)
        {
            Matrix3x3_2 matrix44;
            matrix44.m00 = (float)((double)matrix1.m00 * (double)matrix2.m00 + (double)matrix1.m01 * (double)matrix2.m10 +
                (double)matrix1.m02 * (double)matrix2.m20);
            matrix44.m01 = (float)((double)matrix1.m00 * (double)matrix2.m01 + (double)matrix1.m01 * (double)matrix2.m11 +
                (double)matrix1.m02 * (double)matrix2.m21);
            matrix44.m02 = (float)((double)matrix1.m00 * (double)matrix2.m02 + (double)matrix1.m01 * (double)matrix2.m12 +
                (double)matrix1.m02 * (double)matrix2.m22);
            matrix44.m10 = (float)((double)matrix1.m10 * (double)matrix2.m00 + (double)matrix1.m11 * (double)matrix2.m10 +
                (double)matrix1.m12 * (double)matrix2.m20);
            matrix44.m11 = (float)((double)matrix1.m10 * (double)matrix2.m01 + (double)matrix1.m11 * (double)matrix2.m11 +
                (double)matrix1.m12 * (double)matrix2.m21);
            matrix44.m12 = (float)((double)matrix1.m10 * (double)matrix2.m02 + (double)matrix1.m11 * (double)matrix2.m12 +
                (double)matrix1.m12 * (double)matrix2.m22);
            matrix44.m20 = (float)((double)matrix1.m20 * (double)matrix2.m00 + (double)matrix1.m21 * (double)matrix2.m10 +
                (double)matrix1.m22 * (double)matrix2.m20);
            matrix44.m21 = (float)((double)matrix1.m20 * (double)matrix2.m01 + (double)matrix1.m21 * (double)matrix2.m11 +
                (double)matrix1.m22 * (double)matrix2.m21);
            matrix44.m22 = (float)((double)matrix1.m20 * (double)matrix2.m02 + (double)matrix1.m21 * (double)matrix2.m12 +
                (double)matrix1.m22 * (double)matrix2.m22);
            return matrix44;
        }

        public static void Multiply(ref Matrix3x3_2 matrix1, ref Matrix3x3_2 matrix2, out Matrix3x3_2 result)
        {
            float num1 = (float)((double)matrix1.m00 * (double)matrix2.m00 + (double)matrix1.m01 * (double)matrix2.m10 +
                (double)matrix1.m02 * (double)matrix2.m20 );
            float num2 = (float)((double)matrix1.m00 * (double)matrix2.m01 + (double)matrix1.m01 * (double)matrix2.m11 +
                (double)matrix1.m02 * (double)matrix2.m21 );
            float num3 = (float)((double)matrix1.m00 * (double)matrix2.m02 + (double)matrix1.m01 * (double)matrix2.m12 +
                (double)matrix1.m02 * (double)matrix2.m22 );          
            float num5 = (float)((double)matrix1.m10 * (double)matrix2.m00 + (double)matrix1.m11 * (double)matrix2.m10 +
                (double)matrix1.m12 * (double)matrix2.m20 );
            float num6 = (float)((double)matrix1.m10 * (double)matrix2.m01 + (double)matrix1.m11 * (double)matrix2.m11 +
                (double)matrix1.m12 * (double)matrix2.m21 );
            float num7 = (float)((double)matrix1.m10 * (double)matrix2.m02 + (double)matrix1.m11 * (double)matrix2.m12 +
                (double)matrix1.m12 * (double)matrix2.m22 );
           
            float num9 = (float)((double)matrix1.m20 * (double)matrix2.m00 + (double)matrix1.m21 * (double)matrix2.m10 +
                (double)matrix1.m22 * (double)matrix2.m20 );
            float num10 = (float)((double)matrix1.m20 * (double)matrix2.m01 + (double)matrix1.m21 * (double)matrix2.m11 +
                (double)matrix1.m22 * (double)matrix2.m21 );
            float num11 = (float)((double)matrix1.m20 * (double)matrix2.m02 + (double)matrix1.m21 * (double)matrix2.m12 +
                (double)matrix1.m22 * (double)matrix2.m22 );
           
          
            result.m00 = num1;
            result.m01 = num2;
            result.m02 = num3;
            result.m10 = num5;
            result.m11 = num6;
            result.m12 = num7;
            result.m20 = num9;
            result.m21 = num10;
            result.m22 = num11;
        }

        public Vector2 TransformVector2(Vector2 vector)
        {
            float num1 = (float)((double)vector.x * (double)m00 + (double)vector.y * (double)m01);
            float num2 = (float)((double)vector.x * (double)m10 + (double)vector.y * (double)m11);           
            Vector2 vector4;
            vector4.x = num1;
            vector4.y = num2;           
            return vector4;
        }

        public static void TransformVector2(ref Matrix3x3_2 matrix, ref Vector2 vector, out Vector2 result)
        {
            float num1 = (float)((double)vector.x * (double)matrix.m00 + (double)vector.y * (double)matrix.m01);
            float num2 = (float)((double)vector.x * (double)matrix.m10 + (double)vector.y * (double)matrix.m11);           
            result.x = num1;
            result.y = num2;            
        }

        public Vector2 TransformPosition(Vector2 position)
        {
            float num1 = (float)((double)position.x * (double)m00 + (double)position.y * (double)m01) + m02;
            float num2 = (float)((double)position.x * (double)m10 + (double)position.y * (double)m11) + m12;
           
            Vector2 vector3;
            vector3.x = num1;
            vector3.y = num2;
            return vector3;
        }
        public Vector2 InverseTransformPosition(Vector2 position)
        {
            float num1 = (float)((double)position.x * (double)m00 + (double)position.y * (double)m10) + m20;
            float num2 = (float)((double)position.x * (double)m01 + (double)position.y * (double)m11) + m21;

            Vector2 vector3;
            vector3.x = num1;
            vector3.y = num2;
            return vector3;
        }

        public static void TransformPosition(ref Matrix3x3_2 matrix, ref Vector2 position, out Vector2 result)
        {
            float num1 = (float)((double)position.x * (double)matrix.m00 + (double)position.y * (double)matrix.m01 ) + matrix.m02;
            float num2 = (float)((double)position.x * (double)matrix.m10 + (double)position.y * (double)matrix.m11 ) + matrix.m12;
            result.x = num1;
            result.y = num2;
        }



        public Vector2 TransformDirection(Vector2 direction)
        {
            float num1 = (float)((double)direction.x * (double)m00 + (double)direction.y * (double)m01);
            float num2 = (float)((double)direction.x * (double)m10 + (double)direction.y * (double)m11);
          
            Vector2 vector3;
            vector3.x = num1;
            vector3.y = num2;
            return vector3;
        }

        public static void TransformDirection(ref Matrix3x3_2 matrix, ref Vector3 direction, out Vector3 result)
        {
            float num1 = (float)((double)direction.x * (double)matrix.m00 + (double)direction.y * (double)matrix.m01 +
                (double)direction.z * (double)matrix.m02);
            float num2 = (float)((double)direction.x * (double)matrix.m10 + (double)direction.y * (double)matrix.m11 +
                (double)direction.z * (double)matrix.m12);
            float num3 = (float)((double)direction.x * (double)matrix.m20 + (double)direction.y * (double)matrix.m21 +
                (double)direction.z * (double)matrix.m22);
            result.x = num1;
            result.y = num2;
            result.z = num3;
        }

        public static Matrix3x3_2 operator -(Matrix3x3_2 matrix1)
        {
            Matrix3x3_2 matrix44;
            matrix44.m00 = -matrix1.m00;
            matrix44.m01 = -matrix1.m01;
            matrix44.m02 = -matrix1.m02;
            matrix44.m10 = -matrix1.m10;
            matrix44.m11 = -matrix1.m11;
            matrix44.m12 = -matrix1.m12;
            matrix44.m20 = -matrix1.m20;
            matrix44.m21 = -matrix1.m21;
            matrix44.m22 = -matrix1.m22;
            return matrix44;
        }

        public static bool operator ==(Matrix3x3_2 matrix1, Matrix3x3_2 matrix2)
        {
            if ((double)matrix1.m00 == (double)matrix2.m00 && (double)matrix1.m11 == (double)matrix2.m11 &&
                ((double)matrix1.m22 == (double)matrix2.m22) &&
                ((double)matrix1.m01 == (double)matrix2.m01 && (double)matrix1.m02 == (double)matrix2.m02 &&
                    ((double)matrix1.m10 == (double)matrix2.m10)) &&
                ((double)matrix1.m12 == (double)matrix2.m12 &&
                    ((double)matrix1.m20 == (double)matrix2.m20 && (double)matrix1.m21 == (double)matrix2.m21)))
                return true;
            return false;
        }

        public static bool operator !=(Matrix3x3_2 matrix1, Matrix3x3_2 matrix2)
        {
            if ((double)matrix1.m00 == (double)matrix2.m00 && (double)matrix1.m01 == (double)matrix2.m01 &&
                (double)matrix1.m02 == (double)matrix2.m02 && 
                (double)matrix1.m10 == (double)matrix2.m10 && (double)matrix1.m11 == (double)matrix2.m11 &&
                    (double)matrix1.m12 == (double)matrix2.m12 &&
                (double)matrix1.m20 == (double)matrix2.m20 && (double)matrix1.m21 == (double)matrix2.m21 &&
                    (double)matrix1.m22 == (double)matrix2.m22 )
                return false;
            return true;
        }

        public static Matrix3x3_2 operator +(Matrix3x3_2 matrix1, Matrix3x3_2 matrix2)
        {
            Matrix3x3_2 matrix44;
            matrix44.m00 = matrix1.m00 + matrix2.m00;
            matrix44.m01 = matrix1.m01 + matrix2.m01;
            matrix44.m02 = matrix1.m02 + matrix2.m02;
            matrix44.m10 = matrix1.m10 + matrix2.m10;
            matrix44.m11 = matrix1.m11 + matrix2.m11;
            matrix44.m12 = matrix1.m12 + matrix2.m12;
            matrix44.m20 = matrix1.m20 + matrix2.m20;
            matrix44.m21 = matrix1.m21 + matrix2.m21;
            matrix44.m22 = matrix1.m22 + matrix2.m22;
            return matrix44;
        }

        public static Matrix3x3_2 operator -(Matrix3x3_2 matrix1, Matrix3x3_2 matrix2)
        {
            Matrix3x3_2 matrix44;
            matrix44.m00 = matrix1.m00 - matrix2.m00;
            matrix44.m01 = matrix1.m01 - matrix2.m01;
            matrix44.m02 = matrix1.m02 - matrix2.m02;
            matrix44.m10 = matrix1.m10 - matrix2.m10;
            matrix44.m11 = matrix1.m11 - matrix2.m11;
            matrix44.m12 = matrix1.m12 - matrix2.m12;
            matrix44.m20 = matrix1.m20 - matrix2.m20;
            matrix44.m21 = matrix1.m21 - matrix2.m21;
            matrix44.m22 = matrix1.m22 - matrix2.m22;
            return matrix44;
        }

        public static Matrix3x3_2 operator *(Matrix3x3_2 matrix1, Matrix3x3_2 matrix2)
        {
            Matrix3x3_2 matrix44;
            matrix44.m00 = (float)((double)matrix1.m00 * (double)matrix2.m00 + (double)matrix1.m01 * (double)matrix2.m10 +
                (double)matrix1.m02 * (double)matrix2.m20 );
            matrix44.m01 = (float)((double)matrix1.m00 * (double)matrix2.m01 + (double)matrix1.m01 * (double)matrix2.m11 +
                (double)matrix1.m02 * (double)matrix2.m21 );
            matrix44.m02 = (float)((double)matrix1.m00 * (double)matrix2.m02 + (double)matrix1.m01 * (double)matrix2.m12 +
                (double)matrix1.m02 * (double)matrix2.m22 );
          
            matrix44.m10 = (float)((double)matrix1.m10 * (double)matrix2.m00 + (double)matrix1.m11 * (double)matrix2.m10 +
                (double)matrix1.m12 * (double)matrix2.m20 );
            matrix44.m11 = (float)((double)matrix1.m10 * (double)matrix2.m01 + (double)matrix1.m11 * (double)matrix2.m11 +
                (double)matrix1.m12 * (double)matrix2.m21 );
            matrix44.m12 = (float)((double)matrix1.m10 * (double)matrix2.m02 + (double)matrix1.m11 * (double)matrix2.m12 +
                (double)matrix1.m12 * (double)matrix2.m22 );
           
            matrix44.m20 = (float)((double)matrix1.m20 * (double)matrix2.m00 + (double)matrix1.m21 * (double)matrix2.m10 +
                (double)matrix1.m22 * (double)matrix2.m20);
            matrix44.m21 = (float)((double)matrix1.m20 * (double)matrix2.m01 + (double)matrix1.m21 * (double)matrix2.m11 +
                (double)matrix1.m22 * (double)matrix2.m21 );
            matrix44.m22 = (float)((double)matrix1.m20 * (double)matrix2.m02 + (double)matrix1.m21 * (double)matrix2.m12 +
                (double)matrix1.m22 * (double)matrix2.m22 );           
           
            return matrix44;
        }
    }
}
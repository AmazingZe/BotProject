namespace GameAI.Pathfinding.Core
{
    using UnityEngine;

    public class GraphTransform
    {
        public static readonly GraphTransform identityTransform = new GraphTransform(Matrix4x4.identity);
        public readonly bool onlyTranslational;
        public readonly bool identity;

        readonly bool isXY;
        readonly bool isXZ;

        readonly Matrix4x4 matrix;
        readonly Matrix4x4 inverseMatrix;
        readonly Vector3 up;
        readonly Vector3 translation;
        readonly Quaternion rotation;
        readonly Quaternion inverseRotation;

        public GraphTransform(Matrix4x4 matrix)
        {
            this.matrix = matrix;
            inverseMatrix = matrix.inverse;
            identity = matrix.isIdentity;
            onlyTranslational = MatrixIsTranslational(matrix);
            up = matrix.MultiplyVector(Vector3.up).normalized;
            translation = matrix.MultiplyPoint3x4(Vector3.zero);
            
            rotation = Quaternion.LookRotation(TransformVector(Vector3.forward), TransformVector(Vector3.up));
            inverseRotation = Quaternion.Inverse(rotation);

            isXY = rotation == Quaternion.Euler(-90, 0, 0);
            isXZ = rotation == Quaternion.Euler(0, 0, 0);
        }

        private static bool MatrixIsTranslational(Matrix4x4 matrix)
        {
            return matrix.GetColumn(0) == new Vector4(1, 0, 0, 0) && 
                   matrix.GetColumn(1) == new Vector4(0, 1, 0, 0) && 
                   matrix.GetColumn(2) == new Vector4(0, 0, 1, 0) && 
                   matrix.m33 == 1;
        }
        public Vector3 Transform(Vector3 point)
        {
            if (onlyTranslational) return point + translation;
            return matrix.MultiplyPoint3x4(point);
        }
        public Vector3 TransformVector(Vector3 point)
        {
            if (onlyTranslational) return point;
            return matrix.MultiplyVector(point);
        }

        public Vector3 InverseTransform(Vector3 point)
        {
            if (onlyTranslational) return point - translation;
            return inverseMatrix.MultiplyPoint3x4(point);
        }
    }
}
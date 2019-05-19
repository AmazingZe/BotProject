namespace GameAI.Pathfinding.Utils
{
    using UnityEngine;

    public class GraphTransform
    {
        #region Properties
        private readonly Matrix4x4 m_Matrix;
        private readonly Matrix4x4 m_InverseMatrix;

        private readonly Quaternion m_Rotation;
        private readonly Quaternion m_InverseRoation;

        private readonly Vector3 m_Up;
        private readonly Vector3 m_Translation;

        private readonly bool m_OnlyTranslational;
        private readonly bool m_Identity;
        private readonly bool m_IsXY;
        private readonly bool m_IsXZ;

        public Vector3 GraphUp
        {
            get { return m_Up; }
        }
        #endregion

        private static bool MatrixIsTranslationOnly(Matrix4x4 matrix)
        {
            return matrix.GetColumn(0) == new Vector4(1, 0, 0, 0) &&
                   matrix.GetColumn(1) == new Vector4(0, 1, 0, 0) &&
                   matrix.GetColumn(2) == new Vector4(0, 0, 1, 0) &&
                   matrix.m33 == 1;
        }

        public static GraphTransform operator * (Matrix4x4 matrix, GraphTransform transform)
        {
            return new GraphTransform(matrix * transform.m_Matrix);
        }
        public static GraphTransform operator * (GraphTransform transform, Matrix4x4 matrix)
        {
            return new GraphTransform(transform.m_Matrix * matrix);
        }

        public GraphTransform(Matrix4x4 matrix)
        {
            m_Matrix = matrix;
            m_InverseMatrix = m_Matrix.inverse;
            m_Rotation = Quaternion.LookRotation(TransformVector(Vector3.forward), TransformVector(Vector3.up));
            m_InverseRoation = Quaternion.Inverse(m_Rotation);
            m_Up = m_Matrix.MultiplyVector(Vector3.up);
            m_Translation = m_Matrix.MultiplyPoint3x4(Vector3.zero);

            m_OnlyTranslational = MatrixIsTranslationOnly(m_Matrix);
            m_Identity = m_Matrix.isIdentity;
            m_IsXY = m_Rotation == Quaternion.Euler(-90, 0, 0);
            m_IsXZ = m_Rotation == Quaternion.Euler(0, 0, 0);
        }

        #region Public-API
        // Graph -> World
        public Vector3 TransformVector(Vector3 vector)
        {
            if (m_OnlyTranslational) return vector;
            return m_Matrix.MultiplyVector(vector);
        }
        public Vector3 TransformPoint(Vector3 point)
        {
            if (m_OnlyTranslational) return point + m_Translation;
            return m_Matrix.MultiplyPoint3x4(point);
        }
        // World -> Graph
        public Vector3 InverseTransformPoint(Vector3 point)
        {
            if (m_OnlyTranslational) return point - m_Translation;
            return m_InverseMatrix.MultiplyPoint3x4(point);
        }
        public Vector3 InverseTransformVector(Vector3 vector)
        {
            if (m_OnlyTranslational) return vector;
            return m_InverseMatrix.MultiplyPoint3x4(vector);
        }
        
        public Vector2 ToGraphPlane(Vector3 point)
        {
            if (m_IsXY) return new Vector2(point.x, point.y);
            if (!m_IsXZ) return point = m_InverseRoation * point;
            return new Vector2(point.x, point.z);
        }
        public Vector2 ToGraphPlane(Vector3 point, out float evaluation)
        {
            if(!m_IsXZ) point = m_InverseRoation * point;
            evaluation = point.y;
            return new Vector2(point.x, point.z);
        }
        public Vector3 ToWorld(Vector2 point, float evaluation)
        {
            return m_Rotation * new Vector3(point.x, evaluation, point.y);
        }
        #endregion
    }
} 
namespace GameUtils
{
    using UnityEngine;

    using System.Collections.Generic;

    public static class Bezier
    {
        public static void Generate(List<Vector3> inputList, List<Vector3> retList, int sampleNum)
        {
            retList.Clear();

            int len = inputList.Count;
            retList.Add(inputList[0]);

            for (int i = 1; i <= sampleNum; i++)
            {
                float index = i / (float)sampleNum;
                retList.Add(CalculateBezierPoint(inputList, index));
            }

            retList.Add(inputList[len - 1]);
        }

        private static Vector3 CalculateBezierPoint(List<Vector3> anchors, float t)
        {
            int level = anchors.Count - 1;

            Vector3 retMe = Vector3.zero;

            float u = 1 - t;

            for (int i = 0; i <= level; i++) 
                retMe += MathUtils.C(level,i) * Mathf.Pow(t, i) * Mathf.Pow(u, level - i) * anchors[i]; 

            return retMe;
        }
    }
}
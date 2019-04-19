using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtils;

public class GameEntry : MonoBehaviour
{
    public List<Vector3> anchors = new List<Vector3>();
    public int sampleNum;

    private List<Vector3> curve = new List<Vector3>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Bezier.Generate(anchors, curve, sampleNum);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < anchors.Count - 1; i++) 
            Gizmos.DrawLine(anchors[i], anchors[i + 1]);

        Gizmos.color = Color.white;
        for (int i = 0; i < curve.Count - 1; i++) 
            Gizmos.DrawLine(curve[i], curve[i + 1]); 
    }
}

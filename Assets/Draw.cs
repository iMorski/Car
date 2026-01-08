using UnityEngine;
using System.Collections.Generic;

public class Draw : MonoBehaviour
{
    public Color Color = new Color(1, 1, 1, 1);
    public float Distance = 0.1f;
    public int Count = 1000;

    private List<Vector3> PositionGroup = new List<Vector3>();
    private Vector3 PPosition;

    void Awake()
    {
        PositionGroup.Add(transform.position);
        PPosition = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, PPosition) > Distance)
        {
            if (PositionGroup.Count >= Count) PositionGroup.RemoveAt(0);
            
            PositionGroup.Add(transform.position);
            PPosition = transform.position;
        }
        
        for (int i = 0; i < PositionGroup.Count - 1; i++)
            Debug.DrawLine(PositionGroup[i], PositionGroup[i + 1], Color);
        
        if (PositionGroup.Count > 0)
            Debug.DrawLine(PositionGroup[PositionGroup.Count - 1], transform.position, Color);
    }
}
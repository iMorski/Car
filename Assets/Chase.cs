using UnityEngine;

public class Chase : MonoBehaviour
{
    public Transform Anchor;
    public float SmoothTime;

    private Vector3 Position;

    void Awake()
    {
        Position = transform.position;
    }

    private Vector3 Velocity;

    void LateUpdate()
    {
        if (!Anchor) return;
        
        Vector3 OnAnchorPosition = Position + Anchor.position;
        
        transform.position = Vector3.SmoothDamp(transform.position,
            OnAnchorPosition, ref Velocity, SmoothTime);;
    }
}
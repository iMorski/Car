using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Transform Anchor;
    public float SmoothTime;

    Vector3 Rotation;

    void Awake()
    {
        Rotation = transform.eulerAngles;
    }

    Vector3 Velocity;

    void LateUpdate()
    {
        if (!Anchor) return;
        
        Vector3 OnAnchorRotation = Rotation + Anchor.eulerAngles;
        
        float X = Mathf.SmoothDampAngle(transform.eulerAngles.x, OnAnchorRotation.x, ref Velocity.x, SmoothTime);
        float Y = Mathf.SmoothDampAngle(transform.eulerAngles.y, OnAnchorRotation.y, ref Velocity.y, SmoothTime);
        float Z = Mathf.SmoothDampAngle(transform.eulerAngles.z, OnAnchorRotation.z, ref Velocity.z, SmoothTime);
        
        transform.rotation = Quaternion.Euler(X, Y, Z);
    }
}

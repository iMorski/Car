using UnityEngine;

public class Chase : MonoBehaviour
{
    public Transform Anchor;
    public float SmoothTime;

    Vector3 Position;

    void Awake(){ Position = transform.position; }

    Vector3 Velocity;

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position,
            Anchor.position + Position, ref Velocity, SmoothTime);;
    }
}
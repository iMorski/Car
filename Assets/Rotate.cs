using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Transform Anchor;
    public float SmoothTime;

    Vector3 Rotation;

    void Awake(){ Rotation = transform.eulerAngles; }

    Car Car;

    private void Start(){ Car = Anchor.GetComponent<Car>(); }
    
    Vector3 Velocity;

    void LateUpdate()
    {
        Vector3 From = transform.eulerAngles;
        Vector3 To = Quaternion.LookRotation(Car.Velocity,
            new Vector3(0, 1, 0)).eulerAngles + Rotation;
        
        float X = Mathf.SmoothDampAngle(From.x, To.x, ref Velocity.x, SmoothTime);
        float Y = Mathf.SmoothDampAngle(From.y, To.y, ref Velocity.y, SmoothTime);
        float Z = Mathf.SmoothDampAngle(From.z, To.z, ref Velocity.z, SmoothTime);
        
        transform.rotation = Quaternion.Euler(X, Y, Z);
    }
}

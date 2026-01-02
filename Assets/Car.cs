using UnityEngine;
using UnityEngine.InputSystem;

public class Car : MonoBehaviour
{
    public InputActionReference Vertical;
    public InputActionReference Horizontal;
    public Wheel[] WheelGroup;
    public float WheelRadius;
    public float Speed = 20;
    public float ReSpeed = 10;
    public float Acceleration = 10;
    public float Deceleration = 20;
    public float Drag = 1;
    public float Angle = 30;
    public float AngleSpeed = 180;
    public float Grip = 1;
    
    Vector3 PPosition;
    Vector3 CPosition;

    Quaternion PRotation;
    Quaternion CRotation;

    float Width;
    float Length;

    void Awake()
    {
        CPosition = transform.position;
        CRotation = transform.rotation;
        
        Width = Vector3.Distance(WheelGroup[0].Position(), WheelGroup[1].Position());
        Length = Vector3.Distance(WheelGroup[0].Position(), WheelGroup[2].Position());
        
        Debug.Log(Width + " | " + Length);
    }
    
    void OnEnable(){ Vertical.action.Enable(); Horizontal.action.Enable(); }
    void OnDisable(){ Vertical.action.Disable(); Horizontal.action.Disable(); }
    
    Vector3 Velocity;
    
    float CSpeed;
    float CAngle;
    
    void FixedUpdate()
    {
        PPosition = CPosition;
        PRotation = CRotation;
        
        float Vertical = this.Vertical.action.ReadValue<float>();
        float Horizontal = this.Horizontal.action.ReadValue<float>();

        float Speed = this.Speed;
        float Acceleration = this.Acceleration;

        switch (Vertical)
        {
            case < 0 when CSpeed > 0: Acceleration = Deceleration; break;
            case < 0: Speed = ReSpeed; break;
            case < 1: Acceleration = Drag; break;
        }

        CSpeed = API.SmoothStep(CSpeed, Speed * Vertical, Acceleration * API.FDTime());
        CAngle = API.SmoothStep(CAngle, Angle * Horizontal, AngleSpeed * API.FDTime());

        Velocity = API.SmoothStepPosition(Velocity, transform.forward,
            Grip * API.FDTime());
        
        // Debug.Log("Grip: " + Vector3.Distance(transform.forward, Velocity));
        
        CPosition = CPosition + CSpeed * API.FDTime() * Velocity;
        CRotation = CRotation * Quaternion.Euler(0, CSpeed / Length * Mathf.Tan(
            CAngle * Mathf.Deg2Rad) * Mathf.Rad2Deg * API.FDTime(), 0);
    }
    
    float SAngle;

    void Update()
    {
        float Alpha = (API.Time() - API.FTime()) / API.FDTime();

        transform.position = Vector3.Lerp(PPosition, CPosition, Alpha);
        transform.rotation = Quaternion.Slerp(PRotation, CRotation, Alpha);
        
        float LAngle = 0;
        float RAngle = 0;

        if (Mathf.Abs(CAngle) > 0)
        {
            float CotangentCenter = 1.0f / Mathf.Tan(CAngle * Mathf.Deg2Rad);
            float GeometryFactor = Width / (2 * Length);
            
            LAngle = Mathf.Atan(1.0f / (CotangentCenter + GeometryFactor)) * Mathf.Rad2Deg;
            RAngle = Mathf.Atan(1.0f / (CotangentCenter - GeometryFactor)) * Mathf.Rad2Deg;
        }
        
        SAngle = SAngle + CSpeed / WheelRadius * Mathf.Rad2Deg * API.DTime();
        SAngle %= 360;
        
        WheelGroup[0].transform.localRotation = Quaternion.Euler(0f, LAngle, 0f) * Quaternion.Euler(SAngle, 0f, 0f);
        WheelGroup[1].transform.localRotation = Quaternion.Euler(0f, RAngle, 0f) * Quaternion.Euler(SAngle, 0f, 0f);
        WheelGroup[2].Rotate(Quaternion.Euler(SAngle, 0, 0));
        WheelGroup[3].Rotate(Quaternion.Euler(SAngle, 0, 0));
    }
}

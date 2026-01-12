using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Car : MonoBehaviour
{
    public InputActionReference Vertical;
    public InputActionReference Horizontal;
    public Wheel[] WheelGroup;
    public float WheelRadius;
    public AnimationCurve AccelerationCurve;
    public float Speed = 10;
    public float ReSpeed = 5;
    public float Acceleration = 5;
    public float Deceleration = 20;
    public float Drag = 1;
    public float Angle = 30;
    public float AngleSpeed = 135;
    public float ZSpeed;
    public float ZReSpeed;
    public float GripSpeed = 1;
    
    [HideInInspector] public Vector3 Velocity;

    Transform Camera;
    
    Vector3 PPosition;
    Vector3 CPosition;
    
    Quaternion PRotation;
    Quaternion CRotation;

    float Width;
    float Length;
    float Z;
    
    Vector3[] WheelRelativePositionGroup;
    
    void Awake()
    {
        Camera = UnityEngine.Camera.main.transform;
        
        CPosition = transform.position;
        CRotation = transform.rotation;
        
        Velocity = transform.forward;
        
        Width = Vector3.Distance(WheelGroup[0].transform.position, WheelGroup[1].transform.position);
        Length = Vector3.Distance(WheelGroup[0].transform.position, WheelGroup[2].transform.position);
        
        Z = -Length / 2;
        
        WheelRelativePositionGroup = new Vector3[WheelGroup.Length];
        for(int i = 0; i < WheelGroup.Length; i++)
        {
            WheelRelativePositionGroup[i] = WheelGroup[i].transform.position - transform.position;
        }
    }
    
    void OnEnable(){ Vertical.action.Enable(); Horizontal.action.Enable(); }
    void OnDisable(){ Vertical.action.Disable(); Horizontal.action.Disable(); }
    
    float CSpeed;
    float CAngle;
    
    void FixedUpdate()
    {
        /* [1] */
        PPosition = CPosition;
        PRotation = CRotation;
        
        /* [2] */
        float Vertical = this.Vertical.action.ReadValue<float>();
        float Horizontal = this.Horizontal.action.ReadValue<float>();
        
        /* [3] */
        float Speed = this.Speed;
        float Acceleration = this.Acceleration * AccelerationCurve.Evaluate(CSpeed / Speed);
        
        switch (Vertical)
        {
            case < 0 when CSpeed > 0: Acceleration = Deceleration; break;
            case < 0: Speed = ReSpeed; break;
            case < 1: Acceleration = Drag; break;
        }

        /* [4] */
        CSpeed = Mathf.MoveTowards(CSpeed, Speed * Vertical, Acceleration * FDTime());
        CAngle = Mathf.MoveTowardsAngle(CAngle, Angle * Horizontal, AngleSpeed * FDTime());
        
        /* [5] */
        Quaternion Rotation = Quaternion.Euler(0, CSpeed / Length * Mathf.Tan(
            CAngle * Mathf.Deg2Rad) * Mathf.Rad2Deg * FDTime(), 0);
        
        CRotation = CRotation * Rotation;
        
        /* [6] */
        Z = Horizontal != 0 ?
            Mathf.MoveTowards(Z, Length / 2, ZSpeed * FDTime()) :
            Mathf.MoveTowards(Z, -Length / 2, ZReSpeed * FDTime());
        
        Vector3 Position = CPosition + CRotation * new Vector3(0, 0, Z);
        CPosition = Position + Rotation * (CPosition - Position);
        
        Draw(Position + new Vector3(0, WheelRadius, 0), .25f,
            new Color(0, 0, 1));
        
        /* [7] */
        Velocity = Vector3.RotateTowards(Velocity, CRotation * new Vector3(
            0, 0, 1), GripSpeed * Mathf.PI / 180 * FDTime(), 0);
        
        CPosition = CPosition + Velocity * (CSpeed * FDTime());
        
        Debug.DrawRay(CPosition, Velocity * 5, new Color(0, 1, 0));
        Debug.DrawRay(CPosition, CRotation * new Vector3(0, 0, 1) * 5, new Color(1, 0, 0));
        
        /* [8] */
        Vector3[] HitGroup = new Vector3[WheelGroup.Length + 1];

        Vector3 From = CPosition + new Vector3(0, WheelRadius, 0);
        Vector3 To = CRotation * new Vector3(0, -1, 0);
        
        float Range = WheelRadius + .05f;
        
        Vector3 Hit = this.Hit(From, To, Range);
        if (Hit != new Vector3()) HitGroup[WheelGroup.Length] = Hit;
        
        for (int i = 0; i < WheelGroup.Length; i++)
        {
            From = CPosition + CRotation * WheelRelativePositionGroup[i];
            
            Hit = this.Hit(From, To, Range);
            if (Hit != new Vector3()) HitGroup[i] = Hit;
            else HitGroup[i] = From + To * Range;
        }

        CPosition = (HitGroup[0] + HitGroup[1] + HitGroup[2] + HitGroup[3]) / 4;

        Vector3 F = (HitGroup[0] + HitGroup[1]) / 2;
        Vector3 B = (HitGroup[2] + HitGroup[3]) / 2;
        Vector3 L = (HitGroup[0] + HitGroup[2]) / 2;
        Vector3 R = (HitGroup[1] + HitGroup[3]) / 2;
            
        Vector3 Up = Vector3.Normalize(Vector3.Cross(
            Vector3.Normalize(F - B), Vector3.Normalize(R - L)));
        
        CRotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.ProjectOnPlane(
            CRotation * new Vector3(0, 0, 1), Up)), Up);
    }

    float SAngle;

    void Update()
    {
        /* [1] */
        float Alpha = (Time() - FTime()) / FDTime();

        transform.position = Vector3.Lerp(PPosition, CPosition, Alpha);
        transform.rotation = Quaternion.Slerp(PRotation, CRotation, Alpha);
        
        /* [2] */
        float LAngle = 0;
        float RAngle = 0;

        if (Mathf.Abs(CAngle) > 0)
        {
            float CotangentCenter = 1.0f / Mathf.Tan(CAngle * Mathf.Deg2Rad);
            float GeometryFactor = Width / (2 * Length);
            
            LAngle = Mathf.Atan(1.0f / (CotangentCenter + GeometryFactor)) * Mathf.Rad2Deg;
            RAngle = Mathf.Atan(1.0f / (CotangentCenter - GeometryFactor)) * Mathf.Rad2Deg;
        }
        
        /* [3] */
        SAngle = SAngle + CSpeed / WheelRadius * Mathf.Rad2Deg * DTime();
        SAngle %= 360;
        
        /* [4] */
        WheelGroup[0].transform.localRotation = Quaternion.Euler(0f, LAngle, 0f) * Quaternion.Euler(SAngle, 0f, 0f);
        WheelGroup[1].transform.localRotation = Quaternion.Euler(0f, RAngle, 0f) * Quaternion.Euler(SAngle, 0f, 0f);
        WheelGroup[2].transform.localRotation = Quaternion.Euler(SAngle, 0, 0);
        WheelGroup[3].transform.localRotation = Quaternion.Euler(SAngle, 0, 0);
    }
    
    Vector3 Hit(Vector3 From, Vector3 To, float Range)
    {
        RaycastHit Hit;
        
        if (Physics.Raycast(From, To, out Hit, Range))
        {
            Draw(From, To, Range, new Color(1, 1, 1));
            Draw(Hit.point, .125f, new Color(1, 1, 1));
        }

        return Hit.point;
    }
    
    void Draw(Vector3 Point, float Length, Color Color)
    {
        Vector3 L = Vector3.Normalize(-Camera.right + Camera.up);
        Vector3 R = Vector3.Normalize(Camera.right + Camera.up);

        Debug.DrawRay(Point - L * Length / 2, L * Length, Color);
        Debug.DrawRay(Point - R * Length / 2, R * Length, Color);
    }

    void Draw(Vector3 From, Vector3 To, float Range, Color Color)
        { Debug.DrawRay(From, To * Range, Color); }
    
    float Time(){ return UnityEngine.Time.time; }
    float DTime(){ return UnityEngine.Time.deltaTime; }
    float FTime(){ return UnityEngine.Time.fixedTime; }
    float FDTime(){ return UnityEngine.Time.fixedDeltaTime; }
}

using UnityEngine;

public class API
{
    public static float SmoothStep(float From, float To, float Step)
    {
        float Range = Mathf.Abs(From - To);
        if (Range < Mathf.Epsilon) return To;
        
        return Mathf.Lerp(From, To, Step / Range);
    }

    public static Vector3 SmoothStepPosition(Vector3 From, Vector3 To, float Step)
    {
        float Range = Vector3.Distance(From, To);
        if (Range < Mathf.Epsilon) return To;

        return Vector3.Lerp(From, To, Step / Range);
    }
    
    public static Quaternion SmoothStepRotation(Quaternion From, Quaternion To, float Step)
    {
        float Angle = Quaternion.Angle(From, To);
        if (Angle < Mathf.Epsilon) return To;

        return Quaternion.Slerp(From, To, Step / Angle);
    }

    public static float Time()
    {
        return UnityEngine.Time.time;
    }
    
    public static float DTime()
    {
        return UnityEngine.Time.deltaTime;
    }
    
    public static float FTime()
    {
        return UnityEngine.Time.fixedTime;
    }
    
    public static float FDTime()
    {
        return UnityEngine.Time.fixedDeltaTime;
    }
}

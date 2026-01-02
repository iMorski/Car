using UnityEngine;

public class Wheel : MonoBehaviour
{
    public void Rotate(Quaternion Quaternion)
    {
        transform.localRotation = Quaternion;
    }
    
    public Vector3 Position()
    {
        return transform.position;
    }
}

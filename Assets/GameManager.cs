using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = Mathf.Max(60,
            (int)Screen.currentResolution.refreshRateRatio.value);
    }
}

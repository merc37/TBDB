using UnityEngine;

public class SetVsync : MonoBehaviour
{
    [SerializeField]
    private int vSyncCount = 0;
    [SerializeField]
    private int targetFrameRate = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = vSyncCount;
        Application.targetFrameRate = targetFrameRate;
    }
}

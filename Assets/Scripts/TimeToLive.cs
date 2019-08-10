using UnityEngine;

public class TimeToLive : MonoBehaviour
{
    [SerializeField]
    private float timeToLive = 10;

    void Update()
    {
        if (timeToLive <= 0) Destroy(gameObject);
        else timeToLive -= Time.deltaTime;
    }
}

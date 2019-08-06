using UnityEngine;

public class TimeToLive : MonoBehaviour
{
    [SerializeField]
    private float TTL;

    void Update()
    {
        if (TTL <= 0) Destroy(gameObject);
        else TTL -= Time.deltaTime;
    }
}

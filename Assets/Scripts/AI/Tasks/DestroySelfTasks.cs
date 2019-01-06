using Panda;
using UnityEngine;

public class DestroySelfTasks : MonoBehaviour
{
    [Task]
    bool DestroySelf()
    {
        Destroy(gameObject);
        return true;
    }
}

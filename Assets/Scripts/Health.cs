using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using Events;

public class Health : MonoBehaviour
{
    [SerializeField]
    private short maxHealth;
    public short MaxAmount { get { return maxHealth; } set { maxHealth = value; } }

    private GameObjectEventManager eventManager;

    void Awake()
    {
        eventManager = GetComponent<GameObjectEventManager>();
        eventManager.StartListening(HealthEvents.OnDecreaseHealth, new UnityAction<ParamsObject>(OnDecreaseHealth));
        eventManager.StartListening(HealthEvents.OnIncreaseHealth, new UnityAction<ParamsObject>(OnIncreaseHealth));
        CurrentAmount = MaxAmount;
    }

    void Start()
    {
        eventManager.TriggerEvent(HealthEvents.OnUpdateMaxHealth, new ParamsObject(MaxAmount));
        eventManager.TriggerEvent(HealthEvents.OnUpdateCurrentHealth, new ParamsObject(CurrentAmount));
    }

    private short health;
    private short CurrentAmount
    {
        get { return health; }
        set
        {
            if (value < 0)
            {
                health = 0;
            }
            if (value > maxHealth)
            {
                health = maxHealth;
            }

            health = value;

            eventManager.TriggerEvent(HealthEvents.OnUpdateCurrentHealth, new ParamsObject(value));
        }
    }

    private void OnDecreaseHealth(ParamsObject paramsObj)
    {
        CurrentAmount -= paramsObj.Short;
    }

    private void OnIncreaseHealth(ParamsObject paramsObj)
    {
        CurrentAmount += paramsObj.Short;
    }
}

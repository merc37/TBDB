using EventManagers;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {

    private GameObjectEventManager eventManager;
    [SerializeField]
	private int maxHealth;
	public int MaxAmount {get{return maxHealth;} set{maxHealth = value;}}

    void Awake() {
        eventManager = GetComponent<GameObjectEventManager>();
        CurrentAmount = MaxAmount;
        eventManager.StartListening("DecreaseHealth", new UnityAction<ParamsObject>(DecreaseHealth));
    }

    private int health;
	private int CurrentAmount {
		get{return health;}
		set{
			if(value < 0) {
				health = 0;
			}
			if(value > maxHealth) {
				health = maxHealth;
			}

            ParamsObject paramsObject = new ParamsObject(value);
            paramsObject.Float = (float)MaxAmount;
            eventManager.TriggerEvent("HealthPoints", paramsObject);

            health = value;
		}
	}

    private void DecreaseHealth(ParamsObject paramsObj) {
        CurrentAmount -= paramsObj.Int;
    }

    private void IncreaseHealth(ParamsObject paramsObj) {
        CurrentAmount += paramsObj.Int;
    }

    private void SetMaxHealth() {
        CurrentAmount = MaxAmount;
    }
}

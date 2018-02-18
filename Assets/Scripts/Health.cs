using EventManagers;
using UnityEngine;

public class Health : MonoBehaviour {

    private GameObjectEventManager eventManager;
    [SerializeField]
	private int maxHealth;
	public int MaxAmount {get{return maxHealth;} set{maxHealth = value;}}

	private int health;
	public int CurrentAmount {
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
            eventManager.TriggerEvent("UpdateHealth", paramsObject);

            health = value;
		}
	}

    public void Decrease(int amount) {
        CurrentAmount -= amount;
    }

    public void Increase(int amount) {
        CurrentAmount += amount;
    }

    public void SetMax() {
        CurrentAmount = MaxAmount;
    }

	// Use this for initialization
	void Start () {
        eventManager = GetComponent<GameObjectEventManager>();
        CurrentAmount = MaxAmount;
	}
}

using UnityEngine;
using Panda;
using EventManagers;
using UnityEngine.Events;

public class EnemyAttackTasks : MonoBehaviour {

    [SerializeField]
    private float attackRange = 7;

    private GameObjectEventManager eventManager;

    private Vector2 targetVector;

    private new Rigidbody2D rigidbody;

    void Awake() {
        eventManager = GetComponent<GameObjectEventManager>();
        eventManager.StartListening("SetAttackTarget", new UnityAction<ParamsObject>(SetAttackTarget));
        rigidbody = GetComponent<Rigidbody2D>();
    }

    [Task]
    bool Shoot() {
        eventManager.TriggerEvent("OnShoot");
        return true;
    }

    [Task]
    bool CheckAttackRange() {
        float distanceToTarget = Vector2.Distance(rigidbody.position, targetVector);
        return distanceToTarget <= attackRange;
    }

    [Task]
    bool CheckAttackTimer() {
        return true;
    }

    private void SetAttackTarget(ParamsObject paramsObj) {
        targetVector = paramsObj.Vector2;
    }
}

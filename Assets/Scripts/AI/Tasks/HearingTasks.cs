using UnityEngine;
using UnityEngine.Events;
using EventManagers;

namespace Enemy
{
    public class HearingTasks : MonoBehaviour
    {
        [SerializeField]
        private int hearingLevelThreshold = 3;
        [SerializeField]
        private int hearingDistance = 20;
        [SerializeField]
        private int hearingObstructionThreshold = 2;

        private int playerNoiseLevel = -1;

        private Vector2 playerNoiseLocation;

        private GameObjectEventManager eventManager;
        private new Rigidbody2D rigidbody;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            eventManager.StartListening("OnPlayerMakeNoise", new UnityAction<ParamsObject>(OnPlayerMakeNoise));
        }

        void Update()
        {
            if(playerNoiseLevel > -1)
            {
                if(playerNoiseLevel >= hearingLevelThreshold)
                {
                    Vector2 direction = rigidbody.position - playerNoiseLocation;
                    RaycastHit2D[] walls = Physics2D.RaycastAll(rigidbody.position, playerNoiseLocation);
                    if(walls.Length <= hearingObstructionThreshold)
                    {
                        eventManager.TriggerEvent("OnSetPlayerLastKnownPosition", new ParamsObject(playerNoiseLocation));
                    }
                }
                playerNoiseLevel = -1;
            }
        }

        private void OnPlayerMakeNoise(ParamsObject paramsObj)
        {
            playerNoiseLevel = paramsObj.Int;
            playerNoiseLocation = paramsObj.Vector2;
        }
    }
}

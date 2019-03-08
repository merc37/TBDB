using UnityEngine;
using UnityEngine.Events;
using EventManagers;
using Panda;
using Events;

namespace Enemy
{
    public class HearingTasks : MonoBehaviour
    {
        [SerializeField]
        private int hearingObstructionThreshold = 2;
        [SerializeField]
        private LayerMask hearingBlockMask;

        private bool noiseHeard = false;
        private bool frameNoiseHeard = false;

        private Vector2 noiseLocation;

        private int previousFrameCount;

        private GameObjectEventManager eventManager;
        private new Rigidbody2D rigidbody;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            eventManager.StartListening(PlayerRadiusEvents.OnPlayerMakeNoise, new UnityAction<ParamsObject>(OnPlayerMakeNoise));
        }

        [Task]
        private bool IsNoiseHeard()
        {
            if(previousFrameCount == Time.frameCount)
            {
                return frameNoiseHeard;
            }

            if(noiseHeard)
            {
                noiseHeard = false;
                frameNoiseHeard = true;
                previousFrameCount = Time.frameCount;
                return true;
            }

            frameNoiseHeard = false;
            previousFrameCount = Time.frameCount;
            return false;
        }

        [Task]
        private bool IsNoiseBlockedByWalls()
        {
            RaycastHit2D[] walls = Physics2D.LinecastAll(rigidbody.position, noiseLocation, hearingBlockMask);
            if(walls.Length <= hearingObstructionThreshold)
            {
                return false;
            }
            return true;
        }

        [Task]
        private bool SetPlayerLastKnownPositionToNoiseLocation()
        {
            eventManager.TriggerEvent(EnemyEvents.OnSetPlayerLastKnownLocation, new ParamsObject(noiseLocation));
            return true;
        }

        private void OnPlayerMakeNoise(ParamsObject paramsObj)
        {
            noiseHeard = true;
            noiseLocation = paramsObj.Vector2;
        }
    }
}

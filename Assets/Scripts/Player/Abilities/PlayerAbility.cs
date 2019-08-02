using UnityEngine;
using Events;
using EventManagers;

namespace Player
{
    public abstract class PlayerAbility : MonoBehaviour
    {
        [SerializeField]
        protected float cooldownTime = 1;
        [SerializeField]
        protected Sprite abilityIcon;

        protected Timer cooldownTimer;

        protected GameObjectEventManager eventManager;

        private bool abilityActive;
        private bool cooldownActive;
        private bool abilityReady;

        protected virtual void Awake()
        {
            eventManager = GetComponentInParent<GameObjectEventManager>();
            cooldownTimer = new Timer(cooldownTime);
            abilityActive = false;
            cooldownActive = false;
            abilityReady = true;
        }

        public bool AbilityStart()
        {
            if (abilityReady)
            {
                if (StartAbility())
                {
                    abilityActive = true;
                    abilityReady = false;
                    return true;
                }
            }
            return false;
        }

        protected abstract bool StartAbility();

        protected virtual void AbilityEnd()
        {
            abilityActive = false;
            cooldownActive = true;
        }

        protected virtual void Update()
        {
            if (IsAbilityOnCooldown())
            {
                if (cooldownTimer.Tick())
                {
                    cooldownActive = false;
                    abilityReady = true;
                }
            }
        }

        public Sprite AbilityIcon()
        {
            return abilityIcon;
        }

        public bool IsAbilityActive()
        {
            return abilityActive;
        }

        public bool IsAbilityReady()
        {
            return abilityReady;
        }

        public bool IsAbilityOnCooldown()
        {
            return cooldownActive;
        }

        public Timer GetCooldownTimer()
        {
            return cooldownTimer;
        }
    }
}

using EventManagers;
using UnityEngine;
using UnityEngine.Events;

namespace Player {
    public class PlayerGun : Gun {

        private bool shootLocked = false;
        private bool hasStarted = false;

        private Rigidbody2D lastFired;
        public Rigidbody2D LastFired
        {
            get {
                return lastFired;
            }
        }

        protected override void OnShoot(ParamsObject paramsObj) {
            if(!hasStarted) {
                eventManager.StartListening("UnlockShoot", new UnityAction<ParamsObject>(UnlockShoot));
                hasStarted = true;
            }

            if(!shootLocked) {
                base.OnShoot(paramsObj);
                if(!automatic) {
                    shootLocked = true;
                }
            }
        }

        protected override Rigidbody2D FireProjectile() {
            lastFired = base.FireProjectile();
            return lastFired;
        }

        private void UnlockShoot(ParamsObject paramsObj) {
            shootLocked = false;
        }
    }
}


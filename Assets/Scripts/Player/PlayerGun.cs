using EventManagers;
using UnityEngine;
using UnityEngine.Events;

namespace Player {
    public class PlayerGun : Gun {

        private bool shootLocked = false;
        private bool hasStarted = false;

        private Rigidbody2D _lastFired;
        public Rigidbody2D LastFired
        {
            get {
                return _lastFired;
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
            _lastFired = base.FireProjectile();
            return LastFired;
        }

        private void UnlockShoot(ParamsObject paramsObj) {
            shootLocked = false;
        }
    }
}


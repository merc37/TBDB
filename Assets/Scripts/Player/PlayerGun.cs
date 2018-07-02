using EventManagers;
using UnityEngine.Events;

namespace Player {
    public class PlayerGun : Gun {

        private bool shootLocked = false;
        private bool hasStarted = false;

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

        private void UnlockShoot(ParamsObject paramsObj) {
            shootLocked = false;
        }
    }
}


namespace Events
{
    public class PlayerEvents
    {
        public static readonly string OnUpdateCurrentSlowMotionTime = "OnUpdateCurrentSlowMotionTime";
        public static readonly string OnUpdateMaxSlowMotionTime = "OnUpdateMaxSlowMotionTime";

        public static readonly string OnSendMovementSpeed = "OnSendMovementSpeed";

        public static readonly string OnDisableMovement = "OnDisableMovement";
        public static readonly string OnEnableMovement = "OnEnableMovement";

        public static readonly string OnUnlockShoot = "OnUnlockShoot";

        public static readonly string OnReturnGun = "OnReturnGun";

        public static readonly string OnUpdateAbility1 = "OnUpdateAbility1";
        public static readonly string OnUpdateAbility2 = "OnUpdateAbility2";
        public static readonly string OnUpdateAbility3 = "OnUpdateAbility3";

        public static readonly string OnSetAbility = "OnSetAbility";

        public static readonly string OnInventoryToggle = "OnInventoryToggle";
    }

    public class PlayerRadiusEvents
    {
        public static readonly string OnPlayerMakeNoise = "OnPlayerMakeNoise";
    }

    public class PlayerGlobalEvents
    {
        public static readonly string OnPlayerUpdateMaxHealth = "OnPlayerUpdateMaxHealth";
        public static readonly string OnPlayerUpdateCurrentHealth = "OnPlayerUpdateCurrentHealth";

        public static readonly string OnPlayerUpdateCurrentSlowMotionTime = "OnPlayerUpdateCurrentSlowMotionTime";
        public static readonly string OnPlayerUpdateMaxSlowMotionTime = "OnPlayerUpdateMaxSlowMotionTime";

        public static readonly string OnPlayerUpdateCurrentAmmo = "OnPlayerUpdateCurrentAmmo";
        public static readonly string OnPlayerUpdateMaxAmmo = "OnPlayerUpdateMaxAmmo";

        public static readonly string OnPlayerSendTransform = "OnPlayerSendTransform";
        public static readonly string OnPlayerSendRigidbody = "OnPlayerSendRigidbody";

        public static readonly string OnPlayerUpdateAbility1 = "OnPlayerUpdateAbility1";
        public static readonly string OnPlayerUpdateAbility2 = "OnPlayerUpdateAbility2";
        public static readonly string OnPlayerUpdateAbility3 = "OnPlayerUpdateAbility3";

        public static readonly string OnPlayerInventoryToggle = "OnPlayerInventoryToggle";
    }
}

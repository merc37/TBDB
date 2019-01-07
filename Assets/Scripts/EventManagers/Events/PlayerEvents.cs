namespace Events
{
    public class PlayerEvents
    {
        public static readonly string OnUpdateCurrentSlowMotionTime = "OnUpdateCurrentSlowMotionTime";
        public static readonly string OnUpdateMaxSlowMotionTime = "OnUpdateMaxSlowMotionTime";

        public static readonly string OnSendMovementSpeed = "OnSendMovementSpeed";

        public static readonly string OnUnlockShoot = "OnUnlockShoot";
        public static readonly string OnRollStart = "OnRollStart";
        public static readonly string OnRollCooldownEnd = "OnRollCooldownEnd";
        public static readonly string OnRollCooldownStart = "OnRollCooldownStart";
        public static readonly string OnRollEnd = "OnRollEnd";
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
    }
}

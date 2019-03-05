namespace Events
{
    public class EnemyEvents
    {
        public static readonly string OnPlayerSendRigidbody = "OnPlayerSendRigidbody";
        public static readonly string OnMapSendTransform = "OnMapSendTransform";

        public static readonly string OnSendMovementSpeed = "OnSendMovementSpeed";
        public static readonly string OnSendSightAngle = "OnSendSightAngle";
        public static readonly string OnSendSightDistance = "OnSendSightDistance";
        public static readonly string OnSendSightBlockMask = "OnSendSightBlockMask";

        public static readonly string OnSetMovementTarget = "OnSetMovementTarget";
        public static readonly string OnSetPlayerLastKnownLocation = "OnSetPlayerLastKnownLocation";
        public static readonly string OnSetPlayerLastKnownHeading = "OnSetPlayerLastKnownHeading";
        public static readonly string OnRoll = "OnRoll";
        public static readonly string OnRollStart = "OnRollStart";
        public static readonly string OnRollCooldownEnd = "OnRollCooldownEnd";
        public static readonly string OnRollCooldownStart = "OnRollCooldownStart";
        public static readonly string OnRollEnd = "OnRollEnd";
    }
}

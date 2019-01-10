namespace Events
{
    public class GunEvents
    {
        public static readonly string OnUpdateCurrentAmmo = "OnUpdateCurrentAmmo";
        public static readonly string OnUpdateMaxAmmo = "OnUpdateMaxAmmo";
        public static readonly string OnUpdateGunProjectileSpeed = "OnUpdateGunProjectileSpeed";
        public static readonly string OnUpdateGunDamage = "OnUpdateGunDamage";
        public static readonly string OnUpdateGunTransform = "OnUpdateGunTransform";
        public static readonly string OnUpdateGunFireType = "OnUpdateGunFireType";

        public static readonly string OnReload = "OnReload";
        public static readonly string OnReloadStart = "OnReloadStart";
        public static readonly string OnReloadEnd = "OnReloadEnd";
        public static readonly string OnShoot = "OnShoot";
        public static readonly string OnUnlockFire = "OnUnlockFire";
        public static readonly string OnLockFire = "OnLockFire";
    }
}

namespace JokersPlayground.Items
{
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;

    public class Awp : CustomWeapon
    {
        public override uint Id { get; set; } = 119;
        public override string Name { get; set; } = "AWP";
        public override string Description { get; set; } = "Insta-kills anything.";
        public override SpawnProperties SpawnProperties { get; set; }
        public override Modifiers Modifiers { get; set; }
        public override float Damage { get; set; } = float.MaxValue;
        public override ItemType Type { get; set; } = ItemType.GunE11SR;
    }
}
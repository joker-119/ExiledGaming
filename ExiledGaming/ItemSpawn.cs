namespace ExiledGaming
{
    using Exiled.API.Enums;

    public struct ItemSpawn
    {
        public ItemType Type { get; private set; }
        public int Chance { get; private set; }

        public ItemSpawn(ItemType type, int chance)
        {
            Type = type;
            Chance = chance;
        }

        public void Deconstruct(out ItemType itemType, out int i)
        {
            itemType = Type;
            i = Chance;
        }
    }
}
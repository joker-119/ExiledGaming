using System;
using System.Collections.Generic;
using System.ComponentModel;
using ExiledGaming.Items;

namespace ExiledGaming.Configs
{
    using Exiled.API.Enums;
    using ExiledGaming.Items;

    public class Items
    {
        [Description("X-Ray Healing Item configs")]
        public List<XrayHealing> XrayHealingItems { get; private set; } = new List<XrayHealing>
        {
            new XrayHealing { Type = ItemType.Adrenaline },
        };

        [Description("SCP-035 items.")]
        public List<Scp035> Scp035S { get; set; } = new List<Scp035>
        {
            new Scp035()
        };
    }
}
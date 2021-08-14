using System;
using System.Collections.Generic;
using System.ComponentModel;
using JokersPlayground.Items;

namespace JokersPlayground.Configs
{
    using JokersPlayground.Items;

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
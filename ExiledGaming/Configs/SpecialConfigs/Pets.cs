using System.Collections.Generic;
using System.ComponentModel;

namespace ExiledGaming.Configs
{
    public class Pets
    {
        [Description("The list of badge names/text to allow to have pets.")]
        public List<string> BadgeNames { get; set; } = new List<string>
        {
            "owner"
        };
        
        [Description("The list of UserID's to allow to have pets.")]
        public List<string> UserIds { get; set; } = new List<string>();

        [Description("The list of valid Roles for pets.")]
        public List<RoleType> ValidRoles { get; set; } = new List<RoleType>()
        {
            RoleType.Scp049, RoleType.Scp106, RoleType.Scp173, RoleType.Scp0492, RoleType.Scp93953, RoleType.Scp93989
        };
        
        [Description("Whether or not pets are killable.")]
        public bool GodmodePets { get; set; }
    }
}
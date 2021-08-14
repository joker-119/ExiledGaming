using System.ComponentModel;
using System.IO;
using Exiled.API.Features;

namespace JokersPlayground.Configs
{
    public class SpecialConfigs
    {
        [Description("The file path to the sound file to play when D-Class win. Leave empty to disable.")]
        public string DclassVictorySound { get; set; }= Path.Combine(Path.Combine(Paths.Plugins, "JokersPlayground"), "crabrave.f32le");
        
        [Description("Custom Item Configs")]
        public Items ItemConfigs { get; set; } = new Items();
        
        [Description("Pet related Configs")]
        public Pets PetConfigs { get; set; } = new Pets();
    }
}
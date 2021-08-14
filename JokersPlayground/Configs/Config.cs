using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;

namespace JokersPlayground.Configs
{
    public class Config : IConfig
    {
        public SpecialConfigs SpecialConfigs;
        
        [Description("Whether or not this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not debug messages should be shown.")]
        public bool Debug { get; set; } = true;

        [Description("The folder path to where special config files will be stored.")]
        public string SpecialConfigFolder { get; set; } = Path.Combine(Paths.Configs, "JokersPlayground");

        [Description("The file name for special configs.")]
        public string SpecialConfigFile { get; set; } = "global.yml";
        
        [Description("How long a discharge from an activated generator will last. Set to -1 to disable discharges.")]
        public float GeneratorDischargeDuration { get; set; } = 15f;
        
        [Description("How far infront of the generator it will damage players.")]
        public float GeneratorDischargeDistance { get; set; } = 5f;
        
        [Description("The max angle from straight infront of the center of the generator panel that someone can stand without getting hit.")]
        public float GeneratorDischargeAngle { get; set; } = 30f;
        
        [Description("The status effect to apply to a player when hit by a discharge.")]
        public EffectType GeneratorDischageEffect { get; set; } = EffectType.Burned;
        
        [Description("The duration of the status effect applied when hit by a discharge. Set to -1 to disable. Set to 0 for permanent (lasts until they change roles).")]
        public float GeneratorDischargeEffectDuration { get; set; } = 10f;
        
        [Description("How long units should be spawn protected from grenades. Set to 0 to disable.")]
        public float SpawnProtection { get; set; } = 30f;
        
        [Description("How long after the nuke detonates before players start to take damage from periodically. Set to -1 to disable.")]
        public float SurfaceTensionDelay { get; set; } = 60f;
        
        [Description("How much damage is dealt every 1s after the surface_tension_delay expires.")]
        public float SurfaceTensionDamage { get; set; } = 0.20f;

        [Description("The percentage chance Chaos Insurgency will replace Facility Guards when a round starts. Set to 0 to disable.")]
        public float CiOnStartChance { get; set; } = 20f;

        [Description("How many rounds someone is allowed to spawn as the same role, in a row.")]
        public int SpawnLuckProtectionLimit { get; set; } = 3;

        [Description("Whether or not CASSIE will announce the containment breach when the round starts.")]
        public bool EnableCassieAnnouncements { get; set; } = true;

        [Description("How many rounds must occur in between CASSIE announcements about the containment breach. Set to 0 to have it occur every round.")]
        public int CassieAnnouncementFrequency { get; set; } = 10;

        [Description("Whether or not SCPs will see other fellow SCP types in the 'UnitName' section of their screen.")]
        public bool ShowScpListAsUnits { get; set; } = true;

        public void LoadItemConfigs()
        {
            if (!Directory.Exists(SpecialConfigFolder))
                Directory.CreateDirectory(SpecialConfigFolder);

            string filePath = Path.Combine(SpecialConfigFolder, SpecialConfigFile);
            Log.Info($"{filePath}");
            if (!File.Exists(filePath))
            {
                SpecialConfigs = new SpecialConfigs();
                File.WriteAllText(filePath, Loader.Serializer.Serialize(SpecialConfigs));
            }
            else
            {
                SpecialConfigs = Loader.Deserializer.Deserialize<SpecialConfigs>(File.ReadAllText(filePath));
                File.WriteAllText(filePath, Loader.Serializer.Serialize(SpecialConfigs));
            }
        }
    }
}
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace UsefulZapGun.Compatibility.CodeRebirth
{
    internal class CRConfig //cool name i know
    {
        internal static ConfigEntry<bool> enableACUZap;
        internal static ConfigEntry<bool> enableBearTrapZap;
        internal static ConfigEntry<bool> enableFlashZap;
        internal static ConfigEntry<bool> enableFanZap;
        internal static ConfigEntry<bool> enableLaserZap;
        internal static ConfigEntry<bool> enableMicrowaveZap;
        internal static ConfigEntry<bool> enableTeslaZap;

        internal static void RebirthConfigSetup(ConfigFile cfg)
        {
            enableACUZap = cfg.Bind("Hazards", "Enable ACUnit zap", true, "Decrease radius by 1/15 every second");
            enableBearTrapZap = cfg.Bind("Hazards", "Enable BearTrap zap", true, "...have you ever tried to charge a conductive item?");
            enableFlashZap = cfg.Bind("Hazards", "Enable FlashTurret zap", true, "Increases cooldown to a maximum of 1 minute");
            enableFanZap = cfg.Bind("Hazards", "Enable IndustrialFan zap", true, "Turns off the fan during zap");
            enableLaserZap = cfg.Bind("Hazards", "Enable LazerTurret zap", true, "Beware of the laser turret battery explosion");
            enableMicrowaveZap = cfg.Bind("Hazards", "Enable Microwave zap", true, "Toggle microwave");
            enableTeslaZap = cfg.Bind("Hazards", "Enable Tesla zap", true, "Turns off tesla at half the maximum charge of the zap gun... but beware of the zap gun explosion");
        }
    }
}

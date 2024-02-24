using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using GameNetcodeStuff;
using BepInEx.Logging;
using BepInEx.Configuration;
using Unity.Netcode;
using System.Linq;
using Amrv.ConfigurableCompany;
using Amrv.ConfigurableCompany.content.model;
using Amrv.ConfigurableCompany.content.model.events;
using Amrv.ConfigurableCompany.content.model.data;
using System;
using UnityEngine.Experimental.Rendering;

namespace LethalCompanyTemplate
{
    [BepInPlugin(modGUID, modName, modVersion)]

    internal class Whitelist : BaseUnityPlugin
    {
        private const string modGUID = "Eyan17.whitelist";
        private const string modName = "Whitelist";
        private const string modVersion = "1.0.1";

        public static List<string> whitelist;
        public static List<string> disconnectedPlayers;

        public ConfigEntry<bool> isWhitelistOn;
        private ConfigEntry<string> allowedUsernames;

        private static ConfigurationCategory whitelistCategory = LethalConfiguration.CreateCategory().SetID("eyan17_whitelist").SetName("Whitelist").Build();

        Configuration whitelistInput = LethalConfiguration.CreateConfig().SetID("eyan17_whitelist_input").SetName("Allowed Players").SetTooltip("A list of usernames seperated by a semicolon. All spaces before and after the name are trimed.").SetCategory(whitelistCategory).SetType(ConfigurationTypes.String).SetValue("Player").Build();
        Configuration whitelistToggle = LethalConfiguration.CreateConfig().SetID("eyan17_whitelist_toggle").SetName("Whitelist Toggle").SetTooltip("Toggles whether the whitelist is on.").SetCategory(whitelistCategory).SetType(ConfigurationTypes.Boolean).SetValue(true).Build();
        
        internal static ManualLogSource mls;

        private void Awake()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource(modName);
            mls.LogInfo("The whitelist has been initiated.");

            isWhitelistOn = Config.Bind("General", "isWhitelistOn", true, "Toggles whether the whitelist is on.");
            allowedUsernames = Config.Bind("Whitelist", "allowedUsenames", "Player", "A list of usernames seperated by a semicolon. All spaces before and after the name are trimed.");

            mls.LogInfo("Whitelisted players:");

            whitelistToggle.TrySet(isWhitelistOn.Value, ChangeReason.READ_FROM_FILE);
            whitelistInput.TrySet(allowedUsernames.Value, ChangeReason.READ_FROM_FILE);

            Events.ConfigurationChanged.Listeners += OnConfigUIChanged;
            isWhitelistOn.SettingChanged += OnConfigFileChanged;
            allowedUsernames.SettingChanged += OnConfigFileChanged;

            var a = splitString(allowedUsernames.Value);
            whitelist = a;

            for (int i = 0; i < whitelist.Count; i++)
            {
                mls.LogInfo(whitelist[i]);
            }

            if (isWhitelistOn.Value == true)
            {
                Harmony.CreateAndPatchAll(typeof(HarmonyUpdatePatch));
                mls.LogInfo("Update has been patched.");
            }
        }

        private void OnConfigUIChanged(object sender, EventArgs e)
        {
            isWhitelistOn.Value = whitelistToggle.Get<bool>();
            allowedUsernames.Value = whitelistInput.Get<string>();
            var a = splitString(allowedUsernames.Value);
            whitelist = a;
            mls.LogInfo("Saved settings to config from UI.");
        }

        private void OnConfigFileChanged(object sender, EventArgs args)
        {
            whitelistToggle.TrySet(isWhitelistOn.Value, ChangeReason.READ_FROM_FILE);
            whitelistInput.TrySet(allowedUsernames.Value, ChangeReason.READ_FROM_FILE);
            var a = splitString(allowedUsernames.Value);
            whitelist = a;
            mls.LogInfo("Read settings from config.");
        }

        static List<string> splitString(string input)
        {
            List<string> result = new List<string>();
            string subresult = "";
            bool escape = false;

            foreach (char c in input)
            {
                if (c == '\\' && !escape) escape = true;
                else if (c == ';' && !escape)
                {
                    result.Add(subresult.Trim());
                    subresult = "";
                }
                else if (c == '\\' && escape)
                {
                    subresult += c;
                    escape = false;
                }
                else
                {
                    subresult += c;
                    escape = false;
                }
            }
            result.Add(subresult.Trim());
            return result;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class HarmonyUpdatePatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(ref string ___playerUsername, ref ulong ___actualClientId)
        {
            if (___actualClientId == 0) return;

            if (!Whitelist.whitelist.Contains<string>(___playerUsername))
            {
                NetworkManager.Singleton.DisconnectClient(___actualClientId, "Not on whitelist.");


                if (Whitelist.disconnectedPlayers.Contains(___playerUsername)) return;

                Whitelist.disconnectedPlayers.Add(___playerUsername);
                Whitelist.mls.LogInfo("Disconnected " + ___playerUsername + "; " + "Client id: " + ___actualClientId);
            }
        }
    }
}

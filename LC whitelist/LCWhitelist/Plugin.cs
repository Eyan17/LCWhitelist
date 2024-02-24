using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using GameNetcodeStuff;
using BepInEx.Logging;
using BepInEx.Configuration;
using Unity.Netcode;
using System.Linq;
using System;
using Amrv.ConfigurableCompany.content.model;
using Amrv.ConfigurableCompany;
using Amrv.ConfigurableCompany.content.model.data;

namespace LethalCompanyTemplate
{
    [BepInPlugin(modGUID, modName, modVersion)]

    internal class Whitelist : BaseUnityPlugin
    {
        private const string modGUID = "Eyan17.whitelist";
        private const string modName = "Whitelist";
        private const string modVersion = "1.1.0";

        internal static Whitelist instance = null;
        internal static ManualLogSource mls;
        bool disableUpdateFromGUI = false;

        public static List<string> loadedWhitelist;

        internal ConfigEntry<bool> configWhitelistToggle;
        internal ConfigEntry<string> configWhitelist;

        private static ConfigurationCategory whitelistCategory = LethalConfiguration.CreateCategory().SetID("eyan17_whitelist").SetName("Whitelist").Build();
        Configuration GUIWhitelist = LethalConfiguration.CreateConfig().SetID("eyan17_whitelist_input").SetName("Allowed Players").SetTooltip("A list of usernames seperated by a semicolon. All spaces before and after the name are trimed.").SetCategory(whitelistCategory).SetType(ConfigurationTypes.String).SetValue("Player").Build();
        Configuration GUIWhitelistToggle = LethalConfiguration.CreateConfig().SetID("eyan17_whitelist_toggle").SetName("Whitelist Toggle").SetTooltip("Toggles whether the whitelist is on.").SetCategory(whitelistCategory).SetType(ConfigurationTypes.Boolean).SetValue(true).Build();
        
        private void Awake()
        {
            instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource(modName);
            mls.LogInfo("The whitelist has been initiated.");

            configWhitelistToggle = Config.Bind("General", "isWhitelistOn", true, "Toggles whether the whitelist is on.");
            configWhitelist = Config.Bind("Whitelist", "allowedUsenames", "Player", "A list of usernames seperated by a semicolon. All spaces before and after the name are trimed.");
            
            ReadConfigFile();

            Events.ConfigurationChanged.Listeners += OnConfigUIChanged;

            Harmony.CreateAndPatchAll(typeof(PlayerControllerPatch));
            Harmony.CreateAndPatchAll(typeof(GameNetworkManagerPatch));
            mls.LogInfo("Patches were applied.");
        }

        internal void ReadConfigFile()
        {
            Config.Reload();

            mls.LogInfo("Reloaded data from config file.");

            loadedWhitelist = SplitString(configWhitelist.Value);

            mls.LogMessage("Whitelisted players:");
            for (int i = 0; i < loadedWhitelist.Count; i++)
            {
                mls.LogMessage(loadedWhitelist[i]);
            }

            disableUpdateFromGUI = true;
            GUIWhitelistToggle.TrySet(configWhitelistToggle.Value, ChangeReason.READ_FROM_FILE);
            GUIWhitelist.TrySet(configWhitelist.Value, ChangeReason.READ_FROM_FILE);
            disableUpdateFromGUI = false;
        }

        private void OnConfigUIChanged(object sender, EventArgs e)
        {
            if(disableUpdateFromGUI) return;

            configWhitelistToggle.Value = GUIWhitelistToggle.Get<bool>();
            configWhitelist.Value = GUIWhitelist.Get<string>();

            var a = SplitString(configWhitelist.Value);
            loadedWhitelist = a;

            mls.LogInfo("Saved settings to config from UI.");
        }

        static List<string> SplitString(string input)
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
    internal class PlayerControllerPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(ref string ___playerUsername, ref ulong ___actualClientId, ref bool ___isPlayerControlled)
        {
            if (___actualClientId == 0 || !___isPlayerControlled || !Whitelist.instance.configWhitelistToggle.Value) return;

            if (!Whitelist.loadedWhitelist.Contains<string>(___playerUsername))
            {
                NetworkManager.Singleton.DisconnectClient(___actualClientId, "Not on whitelist.");
                Whitelist.mls.LogInfo("Disconnected " + ___playerUsername + "; " + "Client id: " + ___actualClientId);
            }
        }
    }

    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {
        [HarmonyPatch("ConnectionApproval")]
        [HarmonyPostfix]
        static void ConnectionApprovalPatch()
        {
            Whitelist.instance.ReadConfigFile();
        }
    }
}

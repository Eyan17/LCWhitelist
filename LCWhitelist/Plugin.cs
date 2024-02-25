using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using GameNetcodeStuff;
using BepInEx.Logging;
using BepInEx.Configuration;
using Unity.Netcode;
using System.Linq;

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

        public static List<string> loadedWhitelist;

        internal ConfigEntry<bool> configWhitelistToggle;
        internal ConfigEntry<string> configWhitelist;

        private void Awake()
        {
            instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource(modName);
            mls.LogInfo("The whitelist has been initiated.");

            configWhitelistToggle = Config.Bind("General", "isWhitelistOn", true, "Toggles whether the whitelist is on.");
            configWhitelist = Config.Bind("Whitelist", "allowedUsenames", "Player", "A list of usernames seperated by a semicolon. All spaces before and after the name are trimed.");
            ReadConfigFile();

            Harmony.CreateAndPatchAll(typeof(PlayerControllerPatch));
            Harmony.CreateAndPatchAll(typeof(GameNetworkManagerPatch));
            mls.LogInfo("Patches were applied.");
        }

        internal void ReadConfigFile()
        {
            mls.LogMessage("Reading config file.");
            Config.Reload();

            loadedWhitelist = splitString(configWhitelist.Value);
            mls.LogMessage("Whitelisted players:");
            for (int i = 0; i < loadedWhitelist.Count; i++)
            {
                mls.LogMessage(loadedWhitelist[i]);
            }
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
        static void ConnectionApprovalPatch(NetworkManager.ConnectionApprovalResponse __0){
            Whitelist.instance.ReadConfigFile();
        }
    }
}

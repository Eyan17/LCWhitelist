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
        private const string modVersion = "1.0.1";

        public static List<string> whitelist;

        public ConfigEntry<bool> isWhitelistOn;
        private ConfigEntry<string> allowedUsernames;

        internal static ManualLogSource mls;

        private void Awake()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource(modName);
            mls.LogInfo("The whitelist has been initiated.");

            isWhitelistOn = Config.Bind("General", "isWhitelistOn", true, "Toggles whether the whitelist is on.");
            allowedUsernames = Config.Bind("Whitelist", "allowedUsenames", "Player", "A list of usernames seperated by a semicolon. All spaces before and after the name are trimed.");

            var a = splitString(allowedUsernames.Value);

            whitelist = a;

            mls.LogInfo("Whitelisted players:");

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
                Whitelist.mls.LogInfo("Checking whitelist...");
                NetworkManager.Singleton.DisconnectClient(___actualClientId, "Not on whitelist.");
                Whitelist.mls.LogInfo("Disconnected " + ___playerUsername + "; " + "Client id: " + ___actualClientId);
            }
        }
    }
}




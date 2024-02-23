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
        private const string modGUID = "EyanAndSmajloSlovakian.whitelist";
        private const string modName = "Whitelist";
        private const string modVersion = "1.0.1";

        public static List<string> whitelist;

        public ConfigEntry<bool> isWhitelistOn;

        private ConfigEntry<string> configWhitelist1;
        private ConfigEntry<string> configWhitelist2;
        private ConfigEntry<string> configWhitelist3;
        private ConfigEntry<string> configWhitelist4;
        private ConfigEntry<string> configWhitelist5;
        private ConfigEntry<string> configWhitelist6;
        private ConfigEntry<string> configWhitelist7;
        private ConfigEntry<string> configWhitelist8;
        private ConfigEntry<string> configWhitelist9;
        private ConfigEntry<string> configWhitelist10;
        private ConfigEntry<string> configWhitelist11;
        private ConfigEntry<string> configWhitelist12;
        private ConfigEntry<string> configWhitelist13;
        private ConfigEntry<string> configWhitelist14;
        private ConfigEntry<string> configWhitelist15;
        private ConfigEntry<string> configWhitelist16;
        private ConfigEntry<string> configWhitelist17;
        private ConfigEntry<string> configWhitelist18;
        private ConfigEntry<string> configWhitelist19;
        private ConfigEntry<string> configWhitelist20;
        private ConfigEntry<string> configWhitelist21;
        private ConfigEntry<string> configWhitelist22;
        private ConfigEntry<string> configWhitelist23;
        private ConfigEntry<string> configWhitelist24;
        private ConfigEntry<string> configWhitelist25;
        private ConfigEntry<string> configWhitelist26;
        private ConfigEntry<string> configWhitelist27;
        private ConfigEntry<string> configWhitelist28;
        private ConfigEntry<string> configWhitelist29;
        private ConfigEntry<string> configWhitelist30;
        private ConfigEntry<string> configWhitelist31;
        private ConfigEntry<string> configWhitelist32;



        //      public static List<string> whitelist = new List<string>() { "Adamecnotreal", "SmajloSlovakian", "Eyan17", "VAO_svk", "Kretossk", "Togy", "Eyan"};
        internal static ManualLogSource mls;

        private void Awake()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource(modName);
            mls.LogInfo("The whitelist has been initiated.");

            isWhitelistOn = Config.Bind("General", "isWhitelistOn", true, "Toggles whether the whitelist is on.");

            configWhitelist1 = Config.Bind("Whitelist", "Username1", "", "Player #1 that is allowed to join. Takes in a steam username.");
            configWhitelist2 = Config.Bind("Whitelist", "Username2", "", "Player #2 that is allowed to join. Takes in a steam username.");
            configWhitelist3 = Config.Bind("Whitelist", "Username3", "", "Player #3 that is allowed to join. Takes in a steam username.");
            configWhitelist4 = Config.Bind("Whitelist", "Username4", "", "Player #4 that is allowed to join. Takes in a steam username.");
            configWhitelist5 = Config.Bind("Whitelist", "Username5", "", "Player #5 that is allowed to join. Takes in a steam username.");
            configWhitelist6 = Config.Bind("Whitelist", "Username6", "", "Player #6 that is allowed to join. Takes in a steam username.");
            configWhitelist7 = Config.Bind("Whitelist", "Username7", "", "Player #7 that is allowed to join. Takes in a steam username.");
            configWhitelist8 = Config.Bind("Whitelist", "Username8", "", "Player #8 that is allowed to join. Takes in a steam username.");
            configWhitelist9 = Config.Bind("Whitelist", "Username9", "", "Player #9 that is allowed to join. Takes in a steam username.");
            configWhitelist10 = Config.Bind("Whitelist", "Username10", "", "Player #10 that is allowed to join. Takes in a steam username.");
            configWhitelist11 = Config.Bind("Whitelist", "Username11", "", "Player #11 that is allowed to join. Takes in a steam username.");
            configWhitelist12 = Config.Bind("Whitelist", "Username12", "", "Player #12 that is allowed to join. Takes in a steam username.");
            configWhitelist13 = Config.Bind("Whitelist", "Username13", "", "Player #13 that is allowed to join. Takes in a steam username.");
            configWhitelist14 = Config.Bind("Whitelist", "Username14", "", "Player #14 that is allowed to join. Takes in a steam username.");
            configWhitelist15 = Config.Bind("Whitelist", "Username15", "", "Player #15 that is allowed to join. Takes in a steam username.");
            configWhitelist16 = Config.Bind("Whitelist", "Username16", "", "Player #16 that is allowed to join. Takes in a steam username.");
            configWhitelist17 = Config.Bind("Whitelist", "Username17", "", "Player #17 that is allowed to join. Takes in a steam username.");
            configWhitelist18 = Config.Bind("Whitelist", "Username18", "", "Player #18 that is allowed to join. Takes in a steam username.");
            configWhitelist19 = Config.Bind("Whitelist", "Username19", "", "Player #19 that is allowed to join. Takes in a steam username.");
            configWhitelist20 = Config.Bind("Whitelist", "Username20", "", "Player #20 that is allowed to join. Takes in a steam username.");
            configWhitelist21 = Config.Bind("Whitelist", "Username21", "", "Player #21 that is allowed to join. Takes in a steam username.");
            configWhitelist22 = Config.Bind("Whitelist", "Username22", "", "Player #22 that is allowed to join. Takes in a steam username.");
            configWhitelist23 = Config.Bind("Whitelist", "Username23", "", "Player #23 that is allowed to join. Takes in a steam username.");
            configWhitelist24 = Config.Bind("Whitelist", "Username24", "", "Player #24 that is allowed to join. Takes in a steam username.");
            configWhitelist25 = Config.Bind("Whitelist", "Username25", "", "Player #25 that is allowed to join. Takes in a steam username.");
            configWhitelist26 = Config.Bind("Whitelist", "Username26", "", "Player #26 that is allowed to join. Takes in a steam username.");
            configWhitelist27 = Config.Bind("Whitelist", "Username27", "", "Player #27 that is allowed to join. Takes in a steam username.");
            configWhitelist28 = Config.Bind("Whitelist", "Username28", "", "Player #28 that is allowed to join. Takes in a steam username.");
            configWhitelist29 = Config.Bind("Whitelist", "Username29", "", "Player #29 that is allowed to join. Takes in a steam username.");
            configWhitelist30 = Config.Bind("Whitelist", "Username30", "", "Player #30 that is allowed to join. Takes in a steam username.");
            configWhitelist31 = Config.Bind("Whitelist", "Username31", "", "Player #31 that is allowed to join. Takes in a steam username.");
            configWhitelist32 = Config.Bind("Whitelist", "Username32", "", "Player #32 that is allowed to join. Takes in a steam username.");

            var a = new List<string> { configWhitelist1.Value, configWhitelist2.Value, configWhitelist3.Value, configWhitelist4.Value, configWhitelist5.Value, configWhitelist6.Value, configWhitelist7.Value, configWhitelist8.Value, configWhitelist9.Value, configWhitelist10.Value, configWhitelist11.Value, configWhitelist12.Value, configWhitelist13.Value, configWhitelist14.Value, configWhitelist15.Value, configWhitelist16.Value, configWhitelist17.Value, configWhitelist18.Value, configWhitelist19.Value, configWhitelist20.Value, configWhitelist21.Value, configWhitelist22.Value, configWhitelist23.Value, configWhitelist24.Value, configWhitelist25.Value, configWhitelist26.Value, configWhitelist27.Value, configWhitelist28.Value, configWhitelist29.Value, configWhitelist30.Value, configWhitelist31.Value, configWhitelist32.Value };

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
    }

    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class HarmonyUpdatePatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(ref string ___playerUsername, ref ulong ___actualClientId)
        {
            if (___actualClientId == 0)
            {
                Whitelist.mls.LogInfo("Skiping ovet client, because it is the host.");
                return;
            }

            if (!Whitelist.whitelist.Contains<string>(___playerUsername))
            {
                Whitelist.mls.LogInfo("Checking whitelist...");
                NetworkManager.Singleton.DisconnectClient(___actualClientId, "Not on whitelist.");
                Whitelist.mls.LogInfo("Disconnected " + ___playerUsername + "; " + "Client id: " + ___actualClientId);
            }
        }
    }
}




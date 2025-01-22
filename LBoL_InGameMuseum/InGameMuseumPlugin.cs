using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LBoL.Presentation;
using LBoL.Presentation.I10N;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LBoL_InGameMuseum
{
    [BepInProcess("LBoL.exe")]
    [BepInPlugin(mod_guid, mod_name, mod_version)]
    public class InGameMuseumPlugin : BaseUnityPlugin
    {
        private const string mod_guid = "lbol.ingamemusem";
        private const string mod_name = "LBoL InGameMuseum";
        private const string mod_version = "0.1.0";

        private static readonly Harmony harmony = new Harmony(mod_guid);

        internal static ManualLogSource log;

        public static ConfigEntry<KeyboardShortcut> ShowMuseumKey;

        private void Awake()
        {
            log = Logger;
            log.LogInfo("InGameMuseum plugin awake");

            ShowMuseumKey = Config.Bind("Keys", "ShowMuseumKey",
                new KeyboardShortcut(KeyCode.P),
                new ConfigDescription("Shows Museum"));

            harmony.PatchAll();
        }

        private void OnDestroy()
        {
            if (harmony != null)
                harmony.UnpatchSelf();
        }

        private static void ShowMuseumPanel()
        {
            try
            {
                if (UiManager.GetPanel<MuseumPanel>().IsVisible)
                {
                    log.LogDebug("MuseumPanel toggle off");
                    UiManager.Hide<MuseumPanel>();
                }
                else
                {
                    log.LogDebug("MuseumPanel toggle on");
                    UiManager.Show<MuseumPanel>();
                }

            }
            catch (InvalidOperationException e)
            {
                log.LogError(e);
            }
        }

        private void Update()
        {
            if (UiManager.Instance != null &&
                ShowMuseumKey.Value.IsDown())
            {
                ShowMuseumPanel();
            }
        }

        [HarmonyPatch(typeof(GameMaster), nameof(GameMaster.UnloadMainMenuUi))]
        public static class GameMaster_UnloadMainMenuUi_Patch
        {
            public static void Prefix()
            {
                // Patch to prevent MuseumPanel from being unloaded (thus inaccessible) when a new game run starts.
                log.LogDebug("MuseumPanel removed before unload due to game start");
                GameMaster.MainMenuUiList.Remove(typeof(MuseumPanel));
            }

            public static void Postfix()
            {
                // Patch to move the MuseumPanel above most render layers in the game to prevent
                // unintentional player interaction with some UI elements while the museum is open.
                log.LogDebug("MuseumPanel parentage moved to TopmostLayer");
                var museum_obj = GameObject.Find("UICamera/Canvas/Root/NormalLayer/MuseumPanel");
                var topmost_layer_obj = GameObject.Find("UICamera/Canvas/Root/TopmostLayer");
                museum_obj.transform.parent = topmost_layer_obj.transform;
            }
        }

        [HarmonyPatch(typeof(GameMaster), nameof(GameMaster.UnloadGameRunUi))]
        public static class GameMaster_UnloadGameRunUi_Patch
        {
            public static void Prefix()
            {
                // Patch to return the MuseumPanel to its original layer when a game ends in one form or another.
                // Mostly done to maybe fix a weird freeze issue where the game couldnt find the MuseumPanel after a game run.
                // Also adds the MuseumPanel back into the list of panels to derender or else the game will try to recreate a MuseumPanel when entering the main menu and complain.
                log.LogDebug("MuseumPanel parentage returned to NormalLayer");
                var museum_obj = GameObject.Find("UICamera/Canvas/Root/TopmostLayer/MuseumPanel");
                var normal_layer_obj = GameObject.Find("UICamera/Canvas/Root/NormalLayer");
                museum_obj.transform.parent = normal_layer_obj.transform;

                log.LogDebug("MuseumPanel added back to unload due to exiting to main menu");
                GameMaster.GameRunUiList.Add(typeof(MuseumPanel));
            }
        }

        [HarmonyPatch(typeof(SystemBoard), nameof(SystemBoard.Awake))]
        public static class SystemBoard_Awake_Patch
        {

            public class ShowMuseumListener : MonoBehaviour, IPointerClickHandler
            {

                public UnityEvent rightClick;

                public void OnPointerClick(PointerEventData eventData)
                {
                    if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        log.LogDebug("Deck right clicked");
                        ShowMuseumPanel();
                    }
                        
                }
            }

            public static void Postfix(SystemBoard __instance)
            {
                // Patch to add functionality to clicking the deck button.
                // Right click will open the museum.
                log.LogDebug("Patching SystemBoard Deck button");
                __instance.deckButton.gameObject.AddComponent<ShowMuseumListener>();
            }
        }
    }
}

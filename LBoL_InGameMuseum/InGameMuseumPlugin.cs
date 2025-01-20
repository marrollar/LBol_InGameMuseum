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

namespace LBoL_InGameMuseum
{
    [BepInProcess("LBoL.exe")]
    [BepInPlugin(mod_guid, mod_name, mod_version)]
    public class InGameMuseumPlugin : BaseUnityPlugin
    {
        private const string mod_guid = "lbol.ingamemusem";
        private const string mod_name = "LBoL InGameMuseum";
        private const string mod_version = "1.0.0";

        private static readonly Harmony harmony = new Harmony(mod_guid);

        internal static ManualLogSource log;

        public static ConfigEntry<KeyboardShortcut> ShowMuseumKey;

        private static InGameMuseumPlugin instance;
        /*        private static MuseumPanel museum_panel;
        */

        private static MuseumPanel museum_panel;

        private void Awake()
        {
            log = Logger;
            log.LogInfo("InGameMuseum plugin awake");

            instance = this;

            /*Instantiate(museum_panel);*/

            /*museum_panel = GameObject.Find("MuseumPanel");
            log.LogInfo(museum_panel);*/

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

        private void Update()
        {
            if (UiManager.Instance != null &&
                ShowMuseumKey.Value.IsDown())
            {
                log.LogInfo("Keybind pressed");

                var museum_obj = GameObject.Find("UICamera/Canvas/Root/NormalLayer/MuseumPanel");
                var top_layer_obj = GameObject.Find("UICamera/Canvas/Root/TopmostLayer");

                if (museum_obj != null)
                {
                    log.LogInfo("Museum Obj found");
                    museum_obj.transform.parent = top_layer_obj.transform;
                } else
                {
                    log.LogInfo("Museum Obj not found");
                }
                
                /*log.LogInfo(MainManuPanel_Patch.museum_panel);
                log.LogInfo(UiManager.Instance._panelTable.ContainsKey(typeof(MuseumPanel)));*/

                try
                {
                    if (UiManager.GetPanel<MuseumPanel>().IsVisible)
                    {
                        UiManager.Hide<MuseumPanel>();
                    }
                    else
                    {
                        /*UiManager.GetPanel<MuseumPanel>().Layer = PanelLayer.Top;*/
                        UiManager.Show<MuseumPanel>();
                    }

                }
                catch (InvalidOperationException e)
                {
                    log.LogError(e);
                }

                /*foreach (KeyValuePair<Type, UiPanelBase> entry in UiManager.Instance._panelTable)
                {
                    log.LogInfo($"{entry.Key}, {entry.Value}");
                }*/


            }
        }

        /*[HarmonyPatch(typeof(GameMaster), nameof(GameMaster.CoLoadGameRunUi))]
        public static class MainManuPanel_Patch
        {

            public static async void Prefix()
            {
                *//*log.LogInfo("Prefix patch");
                foreach (KeyValuePair<Type, UiPanelBase> entry in UiManager.Instance._panelTable)
                {
                    log.LogInfo($"{entry.Key}, {entry.Value}");
                }*//*
                
            }
            public static async void Postfix()
            {
                if (!UiManager.Instance._panelTable.ContainsKey(typeof(MuseumPanel)))
                {
                    GameMaster.GameRunUiList.Add(typeof(MuseumPanel));
                    await UiManager.LoadPanelAsync<MuseumPanel>("GameRun", false);

                }

                foreach (Type t in GameMaster.GameRunUiList)
                {
                    log.LogInfo(t);
                }

            }
        }*/

        [HarmonyPatch(typeof(GameMaster), nameof(GameMaster.UnloadMainMenuUi))]
        public static class GameMaster_UnloadMainMenuUi_Patch
        {
            public static void Prefix()
            {
                GameMaster.MainMenuUiList.Remove(typeof(MuseumPanel));
            }
        }

        [HarmonyPatch(typeof(GameMaster), nameof(GameMaster.UnloadGameRunUi))]
        public static class GameMaster_UnloadGameRunUi_Patch
        {
            public static void Prefix()
            {
                GameMaster.GameRunUiList.Add(typeof(MuseumPanel));
            }
        }

        [HarmonyPatch(typeof(MuseumPanel), nameof(MuseumPanel.Awake))]
        public static class MuseumPanel_Awake_Patch
        {
            public static void Postfix(MuseumPanel __instance)
            {
                museum_panel = __instance;
            }
        }

    }
}

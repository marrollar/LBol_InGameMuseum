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
    [BepInDependency(AddWatermark.API.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class InGameMuseumPlugin : BaseUnityPlugin
    {
        private const string mod_guid = "marrollar.lbol.ingamemuseum";
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
        private void Update()
        {
            if (UiManager.Instance != null &&
                ShowMuseumKey.Value.IsDown())
            {
                MuseumPanelPatches.ShowMuseumPanel();
            }
        }

    }
}

using HarmonyLib;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LBoL.Presentation.UI;

namespace LBoL_InGameMuseum
{
    [HarmonyPatch(typeof(GameMaster))]
    internal class MuseumPanelPatches
    {
        internal static void ShowMuseumPanel()
        {
            try
            {
                if (UiManager.GetPanel<MuseumPanel>().IsVisible)
                {
                    InGameMuseumPlugin.log.LogDebug("MuseumPanel toggle off");
                    UiManager.Hide<MuseumPanel>();
                }
                else
                {
                    InGameMuseumPlugin.log.LogDebug("MuseumPanel toggle on");
                    UiManager.Show<MuseumPanel>();
                }

            }
            catch (InvalidOperationException e)
            {
                InGameMuseumPlugin.log.LogError(e);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameMaster.CoSetupGameRun))]
        private static void Patch_RetainMuseumPanel()
        {
            // Patch to prevent MuseumPanel from being unloaded (thus inaccessible) when a new game run starts.
            if (GameMaster.MainMenuUiList.Contains(typeof(MuseumPanel)))
            {
                InGameMuseumPlugin.log.LogDebug("MuseumPanel removed before unload due to game start");
                GameMaster.MainMenuUiList.Remove(typeof(MuseumPanel));

                // Also move MuseumPanel to be above most UI layers in the game.
                InGameMuseumPlugin.log.LogDebug("MuseumPanel parentage moved to TopmostLayer");
                var museum_obj = GameObject.Find("UICamera/Canvas/Root/NormalLayer/MuseumPanel");
                var topmost_layer_obj = GameObject.Find("UICamera/Canvas/Root/TopmostLayer");
                museum_obj.transform.SetParent(topmost_layer_obj.transform, false) ;
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameMaster.CoLeaveGameRun))]
        private static void Patch_RemoveMuseumPanel()
        {
            // Patch to unload MuseumPanel before main menu is hit.
            // Main menu will attempt to reload a MuseumPanel, which it will complain about if there is already one that exists.
            try
            {
                InGameMuseumPlugin.log.LogDebug("MuseumPanel unloaded successfully");
                UiManager.UnloadPanel<MuseumPanel>();
            }
            catch (InvalidOperationException e)
            {
                InGameMuseumPlugin.log.LogDebug("MuseumPanel not found during unload attempt");
            }
        }
    }
}

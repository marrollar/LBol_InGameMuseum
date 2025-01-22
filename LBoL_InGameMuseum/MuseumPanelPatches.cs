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
        [HarmonyPatch(nameof(GameMaster.UnloadMainMenuUi))]
        private static void Patch_RetainMuseumPanel()
        {
            // Patch to prevent MuseumPanel from being unloaded (thus inaccessible) when a new game run starts.
            InGameMuseumPlugin.log.LogDebug("MuseumPanel removed before unload due to game start");
            GameMaster.MainMenuUiList.Remove(typeof(MuseumPanel));
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameMaster.UnloadMainMenuUi))]
        private static void Patch_FixMuseumPanelLayer()
        {
            // Patch to move the MuseumPanel above most render layers in the game to prevent
            // unintentional player interaction with some UI elements while the museum is open.
            InGameMuseumPlugin.log.LogDebug("MuseumPanel parentage moved to TopmostLayer");
            var museum_obj = GameObject.Find("UICamera/Canvas/Root/NormalLayer/MuseumPanel");
            var topmost_layer_obj = GameObject.Find("UICamera/Canvas/Root/TopmostLayer");
            museum_obj.transform.parent = topmost_layer_obj.transform;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameMaster.UnloadGameRunUi))]
        private static void Patch_UnloadMuseumPanel()
        {
            // Patch to return the MuseumPanel to its original layer when a game ends in one form or another.
            // Mostly done to maybe fix a weird freeze issue where the game couldnt find the MuseumPanel after a game run.
            // Also adds the MuseumPanel back into the list of panels to derender or else the game will try to recreate a MuseumPanel when entering the main menu and complain.
            InGameMuseumPlugin.log.LogDebug("MuseumPanel parentage returned to NormalLayer");
            var museum_obj = GameObject.Find("UICamera/Canvas/Root/TopmostLayer/MuseumPanel");
            var normal_layer_obj = GameObject.Find("UICamera/Canvas/Root/NormalLayer");
            museum_obj.transform.parent = normal_layer_obj.transform;

            InGameMuseumPlugin.log.LogDebug("MuseumPanel added back to unload due to exiting to main menu");
            GameMaster.GameRunUiList.Add(typeof(MuseumPanel));
        }
    }
}

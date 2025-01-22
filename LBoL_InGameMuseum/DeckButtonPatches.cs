using HarmonyLib;
using LBoL.Presentation.UI.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

namespace LBoL_InGameMuseum
{
    [HarmonyPatch(typeof(SystemBoard))]
    internal class DeckButtonPatches
    {
        private class ShowMuseumListener : MonoBehaviour, IPointerClickHandler
        {
            public void OnPointerClick(PointerEventData eventData)
            {
                if (eventData.button == PointerEventData.InputButton.Right)
                {
                    InGameMuseumPlugin.log.LogDebug("Deck right clicked");
                    MuseumPanelPatches.ShowMuseumPanel();
                }

            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SystemBoard.Awake))]
        public static void Patch_AddRightClickDeck(SystemBoard __instance)
        {
            // Patch to add functionality to clicking the deck button.
            // Right click will open the museum.
            InGameMuseumPlugin.log.LogDebug("Patching SystemBoard Deck button");
            __instance.deckButton.gameObject.AddComponent<ShowMuseumListener>();
        }
    }
}

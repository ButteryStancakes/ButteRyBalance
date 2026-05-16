using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ButteRyBalance.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    static class TerminalPatches
    {
        internal static float fakeValueMultiplier = 0.4f;

        [HarmonyPatch(nameof(Terminal.TextPostProcess))]
        [HarmonyPrefix]
        [HarmonyBefore(Plugin.GUID_BUTTERY_FIXES, Plugin.GUID_LETHAL_FIXES)]
        [HarmonyPriority(Priority.HigherThanNormal)]
        static void Terminal_Pre_TextPostProcess(ref float __state)
        {
            __state = RoundManager.Instance.scrapValueMultiplier;
            RoundManager.Instance.scrapValueMultiplier = fakeValueMultiplier; // for display purposes
        }

        [HarmonyPatch(nameof(Terminal.TextPostProcess))]
        [HarmonyPostfix]
        static void Terminal_Post_TextPostProcess(float __state)
        {
            RoundManager.Instance.scrapValueMultiplier = __state;
        }

        [HarmonyPatch(nameof(Terminal.SetItemSales))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Terminal_Trans_SetItemSales(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            MethodInfo count = AccessTools.DeclaredPropertyGetter(typeof(List<int>), nameof(List<int>.Count)),
                       next = AccessTools.Method(typeof(System.Random), nameof(System.Random.Next), [typeof(int), typeof(int)]),
                       removeAt = AccessTools.Method(typeof(List<int>), nameof(List<int>.RemoveAt));
            bool patchIndex = false, patchRemove = false;
            for (int i = 4; i < codes.Count - 1; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt)
                {
                    if (!patchIndex && codes[i].operand as MethodInfo == next && codes[i - 1].opcode == OpCodes.Callvirt && codes[i - 1].operand as MethodInfo == count)
                    {
                        codes.Insert(i + 1, new(OpCodes.Callvirt, AccessTools.DeclaredPropertyGetter(typeof(List<int>), "Item")));
                        codes.Insert(i - 4, new(codes[i - 2].opcode, codes[i - 2].operand));
                        i++;
                        patchIndex = true;
                    }
                    else if (!patchRemove && codes[i].operand as MethodInfo == removeAt)
                    {
                        codes[i].operand = AccessTools.Method(typeof(List<int>), nameof(List<int>.Remove));
                        codes.Insert(i + 1, new(OpCodes.Pop));
                        i++;
                        patchRemove = true;
                    }
                }

                if (patchIndex && patchRemove)
                {
                    Plugin.Logger.LogDebug("Transpiler (Terminal): Fix discount calculations");
                    return codes;
                }
            }

            Plugin.Logger.LogWarning("Terminal transpiler failed");
            return instructions;
        }

        [HarmonyPatch(nameof(Terminal.RunTerminalEvents))]
        [HarmonyPostfix]
        static void Terminal_Post_RunTerminalEvents(Terminal __instance, TerminalNode node)
        {
            if (node.terminalEvent == "cheat_ResetCredits" && GameNetworkManager.Instance.localPlayerController.IsServer)
            {
                string username = GameNetworkManager.Instance.localPlayerController.playerUsername;
                if (username == "Zeekerss" || username == "Blueray" || username == "Puffo")
                    return; // don't run twice

                __instance.useCreditsCooldown = true;
                __instance.groupCredits = 2500;
                __instance.SyncGroupCreditsServerRpc(__instance.groupCredits, __instance.numberOfItemsInDropship);
            }
        }
    }
}

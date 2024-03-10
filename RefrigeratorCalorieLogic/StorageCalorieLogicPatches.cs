using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using HarmonyLib;

namespace RazorspinMods_ONI
{
    public class StorageCalorieLogicPatches
    {
        private static void AddComponent(GameObject go)
        {
            go.AddOrGet<StorageCalorieLogic>();
        }

        [HarmonyPatch(typeof(RefrigeratorConfig))]
        [HarmonyPatch(nameof(RefrigeratorConfig.DoPostConfigureComplete))]
        public static class RefrigeratorConfig_DoPostConfigureComplete_Patch
        {
            private static void Postfix(GameObject go)
            {
                AddComponent(go);
            }
        }

        [HarmonyPatch(typeof(RefrigeratorConfig))]
        [HarmonyPatch(nameof(RefrigeratorConfig.CreateBuildingDef))]
        public static class RefrigeratorConfig_CreateBuildingDef_Patch
        {
            private static void Postfix(ref BuildingDef __result)
            {
                __result.LogicOutputPorts = new List<LogicPorts.Port>()
                {
                    LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), (string) StorageCalorieLogic.UISTRINGS.LOGIC_PORT, (string) StorageCalorieLogic.UISTRINGS.LOGIC_PORT_ACTIVE, (string) StorageCalorieLogic.UISTRINGS.LOGIC_PORT_INACTIVE)
                };
            }
        }

        [HarmonyPatch(typeof(Refrigerator))]
        [HarmonyPatch("UpdateLogicCircuit")]
        public static class Refrigerator_UpdateLogicCircuit_Patch
        {
            private static bool Prefix()
            {
                //Disable existing Refrigerator logic functionality
                return false;
            }
        }
    }
}

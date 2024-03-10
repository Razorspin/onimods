using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using STRINGS;

namespace RazorspinMods_ONI
{
    [HarmonyPatch(typeof(Database.ChoreTypes))]
    [HarmonyPatch("Add")]
    public class ChoreGroupPatch
    {
        private static Dictionary<string, string[]> __CHOREGROUPPATCH = new Dictionary<string, string[]> {
            { DUPLICANTS.CHORES.FOODFETCH.NAME, new string[1] { "Storage" }}, //Store Food
            { DUPLICANTS.CHORES.TRANSPORT.NAME, new string[2] { "Storage", "Basekeeping" }}, //Sweep
            { DUPLICANTS.CHORES.EQUIPMENTFETCH.NAME, new string[1] { "Storage" }}, //Store equipment
            { DUPLICANTS.CHORES.EMPTYSTORAGE.NAME, new string[2] { "Storage", "Basekeeping" }} //Empty Storage
        };

        public static void Prefix(string id,
          ref string[] chore_groups,
          string urge,
          string[] interrupt_exclusion,
          string name,
          string status_message,
          string tooltip,
          bool skip_implicit_priority_change,
          int explicit_priority = -1,
          string report_name = null)
        {
            if (__CHOREGROUPPATCH.ContainsKey(name))
            {
                chore_groups = __CHOREGROUPPATCH[name];
            }
        }
    }
}

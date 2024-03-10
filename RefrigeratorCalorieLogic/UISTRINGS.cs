using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorspinMods_ONI
{
    public class UISTRINGS
    {
        public static LocString TITLE = (LocString)"Pressure";
        public static LocString MEASUREMENT = (LocString)"Stored KCalories";
        public static LocString TOOLTIP_THRESHOLD_ABOVE = (LocString)("Will send a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if storage has " + UI.PRE_KEYWORD + "calories" + UI.PST_KEYWORD + " above <b>{0}</b>");
        public static LocString TOOLTIP_THRESHOLD_BELOW = (LocString)("Will send a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if storage has " + UI.PRE_KEYWORD + "calories" + UI.PST_KEYWORD + " below <b>{0}</b>");
        public static LocString LOGIC_PORT = (LocString)"Calorie sensor parameters";
        public static LocString LOGIC_PORT_ACTIVE = (LocString)("Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when storage has calories that meet the specified threshold requirement. ");
        public static LocString LOGIC_PORT_INACTIVE = (LocString)("Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby));
    }
}

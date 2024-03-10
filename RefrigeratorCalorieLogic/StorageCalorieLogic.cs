using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using KSerialization;
using STRINGS;
using UnityEngine;

namespace RazorspinMods_ONI
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class StorageCalorieLogic: KMonoBehaviour, IThresholdSwitch
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

        [MyCmpGet]
        private Storage storage;

        [MyCmpGet]
        private LogicPorts ports;

        [Serialize]
        private float threshold;
        [Serialize]
        private bool activateAboveThreshold;

        private FilteredStorage filteredStorage;

        private static readonly EventSystem.IntraObjectHandler<StorageCalorieLogic> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<StorageCalorieLogic>((System.Action<StorageCalorieLogic, object>)((component, data) => component.OnCopySettings(data)));
        private static readonly EventSystem.IntraObjectHandler<StorageCalorieLogic> UpdateLogicCircuitCBDelegate = new EventSystem.IntraObjectHandler<StorageCalorieLogic>((System.Action<StorageCalorieLogic, object>)((component, data) => component.UpdateLogicCircuitCB(data)));

        public float TotalKCalories
        {
            get
            {
                float calorieCount = 0;
                foreach (GameObject item in storage.items)
                {
                    Edible edible = item.GetComponent<Edible>();
                    if (edible != null)
                    {
                        calorieCount += edible.Calories;
                    }
                }

                return calorieCount / 1000;
            }
        }
        /*
         * KMonoBehaviour
         */

        protected override void OnSpawn()
        {
            base.OnSpawn();

            Component componentInstance = this.storage.GetComponent<Refrigerator>();
            Type componentType = componentInstance.GetType();

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo field = componentType.GetField("filteredStorage", bindingFlags);

            this.filteredStorage = (FilteredStorage)field?.GetValue(componentInstance);

            this.UpdateLogicCircuit();
            this.Subscribe<StorageCalorieLogic>(-905833192, StorageCalorieLogic.OnCopySettingsDelegate);
            this.Subscribe<StorageCalorieLogic>(-1697596308, StorageCalorieLogic.UpdateLogicCircuitCBDelegate);
            this.Subscribe<StorageCalorieLogic>(-592767678, StorageCalorieLogic.UpdateLogicCircuitCBDelegate);
        }

        /*
         * IThreshold implementation
         */
        public float Threshold 
        { 
            get 
            { 
                return this.threshold; 
            } 
            set 
            {
                if( this.threshold != value)
                {
                    this.threshold = value;
                    this.UpdateLogicCircuit();
                }
            } 
        }

        public bool ActivateAboveThreshold 
        { 
            get 
            { 
                return this.activateAboveThreshold; 
            } 
            set 
            {
                if(this.activateAboveThreshold != value)
                {
                    this.activateAboveThreshold = value;
                    this.UpdateLogicCircuit();
                }
            } 
        }

        public float CurrentValue { get { return this.TotalKCalories; } }

        public float RangeMin { get { return 0f; } }

        public float RangeMax { get { return this.storage.capacityKg * 4000; } }

        public LocString Title { get { return UISTRINGS.TITLE; } }

        public LocString ThresholdValueName { get { return UISTRINGS.MEASUREMENT; } }

        public string AboveToolTip { get { return UISTRINGS.TOOLTIP_THRESHOLD_ABOVE; } }

        public string BelowToolTip { get { return UISTRINGS.TOOLTIP_THRESHOLD_BELOW; } }

        public ThresholdScreenLayoutType LayoutType { get { return ThresholdScreenLayoutType.SliderBar; } }

        public int IncrementScale { get { return 500; } }

        public NonLinearSlider.Range[] GetRanges { get { return NonLinearSlider.GetDefaultRange(this.RangeMax); } }

        public string Format(float value, bool units)
        {
            return GameUtil.GetFormattedInt((float)((int)value), GameUtil.TimeSlice.None);
        }

        public float GetRangeMaxInputField() => this.RangeMax;

        public float GetRangeMinInputField() => this.RangeMin;

        public float ProcessedInputValue(float input) => input;

        public float ProcessedSliderValue(float input)
        {
            input = ((int)(input / this.IncrementScale)) * this.IncrementScale;
            return input;
        }

        public LocString ThresholdValueUnits() => UI.UNITSUFFIXES.CALORIES.KILOCALORIE;

        /*
         * Event handler
         */
        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
                return;
            StorageCalorieLogic component = gameObject.GetComponent<StorageCalorieLogic>();
            if ((UnityEngine.Object)component == (UnityEngine.Object)null)
                return;
            this.threshold = component.threshold;
            this.activateAboveThreshold = component.activateAboveThreshold; 
        }

        private void UpdateLogicCircuitCB(object data) => this.UpdateLogicCircuit();

        private void UpdateLogicCircuit()
        {
            bool thresholdCheck = this.CurrentValue / this.threshold > 1.0f;
            bool signalValue = this.activateAboveThreshold ? thresholdCheck : !thresholdCheck;

            this.ports.SendSignal(FilteredStorage.FULL_PORT_ID, signalValue ? 1 : 0);
            this.filteredStorage.SetLogicMeter(signalValue);
        }
    }
}

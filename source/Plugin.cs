﻿using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LethalCompanyInputUtils.Api;
using LethalCompanyInputUtils.BindingPathEnums;
using UnityEngine.InputSystem;

namespace BetterVehicleControls
{
    [BepInPlugin(modGUID, "BetterVehicleControls", modVersion)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    internal class PluginLoader : BaseUnityPlugin
    {
        internal const string modGUID = "Dev1A3.BetterVehicleControls";
        internal const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static bool initialized;

        public static PluginLoader Instance { get; private set; }

        internal static VehicleControls VehicleControlsInstance;

        internal static ManualLogSource logSource;

        private void Awake()
        {
            if (initialized)
            {
                return;
            }
            initialized = true;
            Instance = this;
            logSource = Logger;
            VehicleControlsInstance = new VehicleControls();

            FixesConfig.InitConfig();

            Assembly patches = Assembly.GetExecutingAssembly();
            harmony.PatchAll(patches);
        }

        public void BindConfig<T>(ref ConfigEntry<T> config, string section, string key, T defaultValue, string description = "")
        {
            config = Config.Bind<T>(section, key, defaultValue, description);
        }
    }
    internal class FixesConfig
    {
        internal static ConfigEntry<bool> AutoSwitchDriveReverse;
        internal static ConfigEntry<bool> AutoSwitchFromParked;
        internal static ConfigEntry<bool> AutoSwitchToParked;
        internal static ConfigEntry<bool> RecenterWheel;
        internal static ConfigEntry<float> ChanceToStartIgnition;

        internal static void InitConfig()
        {
            PluginLoader.Instance.BindConfig(ref AutoSwitchDriveReverse, "Settings", "Automatic Gearbox", false, "Should the gear automatically switch between drive & reverse when pressing the forward/backwards buttons?");;
            PluginLoader.Instance.BindConfig(ref AutoSwitchFromParked, "Settings", "Automatic Handbrake Release", false, "Should the gear automatically switch to drive/reverse from parked?");
            PluginLoader.Instance.BindConfig(ref AutoSwitchToParked, "Settings", "Automatic Handbrake Pull", false, "Should the gear automatically switch to parked when the key is taken from the ignition?");
            PluginLoader.Instance.BindConfig(ref RecenterWheel, "Settings", "Automatically Center Wheel", true, "Should the wheel be automatically re-centered?");
            AcceptableValueRange<float> ignitionChance = new AcceptableValueRange<float>(0f, 101f);
            ChanceToStartIgnition = PluginLoader.Instance.Config.Bind("Settings", "Ignition Chance", 0f, new ConfigDescription("What should the success chance for the ignition be? If set to 0 this will increase the chance each time the ignition is used. (Vanilla: 0)", ignitionChance));
        }
    }

    internal class VehicleControls : LcInputActions
    {
        [InputAction(KeyboardControl.None, Name = "Drive Forward", GamepadPath = "<Gamepad>/leftStick/up")]
        public InputAction MoveForwardsKey { get; set; }

        [InputAction(KeyboardControl.None, Name = "Drive Backward", GamepadPath = "<Gamepad>/leftStick/down")]
        public InputAction MoveBackwardsKey { get; set; }

        [InputAction(KeyboardControl.W, Name = "Gas Pedal", GamepadControl = GamepadControl.RightTrigger)]
        public InputAction GasPedalKey { get; set; }

        [InputAction(KeyboardControl.S, Name = "Brake", GamepadControl = GamepadControl.LeftTrigger)]
        public InputAction BrakePedalKey { get; set; }

        [InputAction(KeyboardControl.Space, Name = "Boost", GamepadControl = GamepadControl.ButtonNorth)]
        public InputAction TurboKey { get; set; }

        [InputAction(MouseControl.ScrollUp, Name = "Shift Gear Forward", GamepadControl = GamepadControl.LeftShoulder)]
        public InputAction GearShiftForwardKey { get; set; }

        [InputAction(MouseControl.ScrollDown, Name = "Shift Gear Backward", GamepadControl = GamepadControl.RightShoulder)]
        public InputAction GearShiftBackwardKey { get; set; }
    }
}
using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using Facepunch.Steamworks;
using BepInEx.Configuration;
using System.Reflection;
using System;

namespace CommandPotential
{

    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI))]
    public class CommandPotential : BaseUnityPlugin
	{

        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "DarkKronicle";
        public const string PluginName = "CommandPotential";
        public const string PluginVersion = "1.1.0";

        public static AssetBundle AssetBundle;

        public static WeightedSelection<RoR2.PickupIndex> Tier1 = null;
        public static WeightedSelection<RoR2.PickupIndex> Tier2 = null;
        public static WeightedSelection<RoR2.PickupIndex> Tier3 = null;
        public static WeightedSelection<RoR2.PickupIndex> Void1 = null;
        public static WeightedSelection<RoR2.PickupIndex> Void2 = null;
        public static WeightedSelection<RoR2.PickupIndex> Void3 = null;
        public static WeightedSelection<RoR2.PickupIndex> Boss = null;
        public static WeightedSelection<RoR2.PickupIndex> Lunar = null;
        public static WeightedSelection<RoR2.PickupIndex> Equipment = null;
        public static WeightedSelection<RoR2.PickupIndex> LunarEquipment = null;
        public static GameObject Prefab = null;


        public static ConfigEntry<bool> OverrideCommand;


        public void Awake()
        {
            Log.Init(Logger);
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommandPotential.commandpotentialbundle"))
            {
                AssetBundle = AssetBundle.LoadFromStream(stream);
                Log.LogInfo(AssetBundle);
                foreach (string s in AssetBundle.GetAllAssetNames()) {
                    Log.LogInfo(s);
                }
            }
            InitConfig();

            On.RoR2.Artifacts.CommandArtifactManager.OnDropletHitGroundServer += CommandOveride;
            On.RoR2.Run.Start += (orig, self) => {
                orig(self);
                SetupSelections();
            };

            new InfluenceArtifact();

            Log.LogInfo(nameof(Awake) + " done.");
        }

        public void InitConfig()
        {
            OverrideCommand = Config.Bind(
                "Settings",
                "OverrideCommand",
                false,
                "DOES NOT WORK AT THE MOMENT. If enabled it will override the command artifact instead of creating a new one and make this work server-side."
            );
        }

        public static void SetupSelections() 
        {
            Tier1 = CreateSelection(RoR2.Run.instance.availableTier1DropList);
            Tier2 = CreateSelection(RoR2.Run.instance.availableTier2DropList);
            Tier3 = CreateSelection(RoR2.Run.instance.availableTier3DropList);
            Void1 = CreateSelection(RoR2.Run.instance.availableVoidTier1DropList);
            Void2 = CreateSelection(RoR2.Run.instance.availableVoidTier2DropList);
            Void3 = CreateSelection(RoR2.Run.instance.availableVoidTier3DropList);
            Boss = CreateSelection(RoR2.Run.instance.availableBossDropList);
            Lunar = CreateSelection(RoR2.Run.instance.availableLunarItemDropList);
            Equipment = CreateSelection(RoR2.Run.instance.availableEquipmentDropList);
            LunarEquipment = CreateSelection(RoR2.Run.instance.availableLunarEquipmentDropList);
            Prefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/OptionPickup");
        }

        public static void CommandOveride(
            On.RoR2.Artifacts.CommandArtifactManager.orig_OnDropletHitGroundServer orig, 
            ref RoR2.GenericPickupController.CreatePickupInfo pickupInfo, 
            ref bool shouldSpawn
        ) 
        {
            if (!OverrideCommand.Value || !InfluenceArtifact.InfluenceDroplet(ref pickupInfo, ref shouldSpawn))
            {
                orig(ref pickupInfo, ref shouldSpawn);
                return;
            }

        }

        public static WeightedSelection<RoR2.PickupIndex> CreateSelection(List<PickupIndex> arr)
        {
            WeightedSelection<RoR2.PickupIndex> weight = new WeightedSelection<RoR2.PickupIndex>(8);
            foreach (RoR2.PickupIndex pickup in arr)
            {
                weight.AddChoice(pickup, 1);
            }
            return weight;
        }

    }
}

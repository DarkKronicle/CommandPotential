using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;

namespace CommandPotential
{
    public class Storage
    {

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
        public static ConfigEntry<bool> EnabledInBazaar;
        public static ConfigEntry<bool> SpawnMultiShops;

        public static ConfigEntry<int> Tier1Options;
        public static ConfigEntry<int> Tier2Options;
        public static ConfigEntry<int> Tier3Options;
        public static ConfigEntry<int> Void1Options;
        public static ConfigEntry<int> Void2Options;
        public static ConfigEntry<int> Void3Options;
        public static ConfigEntry<int> BossOptions;
        public static ConfigEntry<int> LunarOptions;
        public static ConfigEntry<int> EquipmentOptions;
        public static ConfigEntry<int> LunarEquipmentOptions;
        

        public static void SetupSelections() 
        {
            Run run = RoR2.Run.instance;

            Tier1 = CreateSelection(run.availableTier1DropList);
            Tier2 = CreateSelection(run.availableTier2DropList);
            Tier3 = CreateSelection(run.availableTier3DropList);
            Void1 = CreateSelection(run.availableVoidTier1DropList);
            Void2 = CreateSelection(run.availableVoidTier2DropList);
            Void3 = CreateSelection(run.availableVoidTier3DropList);
            Boss = CreateSelection(run.availableBossDropList);
            Lunar = CreateSelection(run.availableLunarItemDropList);
            Equipment = CreateSelection(run.availableEquipmentDropList);
            LunarEquipment = CreateSelection(run.availableLunarEquipmentDropList);
            Prefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/OptionPickup");
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
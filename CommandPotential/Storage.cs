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
        public static GameObject Garbage = null;


        // Based off of
        // https://github.com/FunkFrog/ShareSuite/blob/master/ShareSuite/ItemSharingHooks.cs#L19-L23
        // GPL-v3
        public static readonly List<CostTypeIndex> PrinterObjects = new List<CostTypeIndex>
        {
            CostTypeIndex.WhiteItem, CostTypeIndex.GreenItem, CostTypeIndex.RedItem, CostTypeIndex.BossItem, CostTypeIndex.LunarItemOrEquipment
        };


        public static ConfigEntry<bool> OverrideCommand;
        public static ConfigEntry<bool> EnabledInBazaar;
        public static ConfigEntry<bool> SpawnMultiShops;
        public static ConfigEntry<bool> PrintersOnlyDropOne;

        public static ConfigEntry<string> Tier1OptionsConfig;
        public static ConfigEntry<string> Tier2OptionsConfig;
        public static ConfigEntry<string> Tier3OptionsConfig;
        public static ConfigEntry<string> Void1OptionsConfig;
        public static ConfigEntry<string> Void2OptionsConfig;
        public static ConfigEntry<string> Void3OptionsConfig;
        public static ConfigEntry<string> BossOptionsConfig;
        public static ConfigEntry<string> LunarOptionsConfig;
        public static ConfigEntry<string> EquipmentOptionsConfig;
        public static ConfigEntry<string> LunarEquipmentOptionsConfig;

        public static WeightedSelection<int> Tier1Options;
        public static WeightedSelection<int> Tier2Options;
        public static WeightedSelection<int> Tier3Options;
        public static WeightedSelection<int> Void1Options;
        public static WeightedSelection<int> Void2Options;
        public static WeightedSelection<int> Void3Options;
        public static WeightedSelection<int> BossOptions;
        public static WeightedSelection<int> LunarOptions;
        public static WeightedSelection<int> EquipmentOptions;
        public static WeightedSelection<int> LunarEquipmentOptions;
        

        public static void InitConfig()
        {
            SetupOptions();
        }

        public static void SetupOptions()
        {
            Tier1Options = SelectionFromString(Tier1OptionsConfig.Value);
            Tier2Options = SelectionFromString(Tier2OptionsConfig.Value);
            Tier3Options = SelectionFromString(Tier3OptionsConfig.Value);
            Void1Options = SelectionFromString(Void1OptionsConfig.Value);
            Void2Options = SelectionFromString(Void2OptionsConfig.Value);
            Void3Options = SelectionFromString(Void3OptionsConfig.Value);
            BossOptions = SelectionFromString(BossOptionsConfig.Value);
            LunarOptions = SelectionFromString(LunarOptionsConfig.Value);
            EquipmentOptions = SelectionFromString(EquipmentOptionsConfig.Value);
            LunarEquipmentOptions = SelectionFromString(LunarEquipmentOptionsConfig.Value);
        }

        public static WeightedSelection<int> SelectionFromString(string input)
        {
            string[] values = input.Split(',');
            WeightedSelection<int> sel = new WeightedSelection<int>(8);
            foreach (string s in values)
            {
                string[] options = s.Split('|');
                if (options.Length > 1)
                {
                    try 
                    {
                        int num1 = int.Parse(options[0]);
                        float num2 = float.Parse(options[1]);
                        sel.AddChoice(num1, num2);
                    } catch (FormatException) {
                        Log.LogError("Invalid Config! Configuration option " + s + " in " + input + " is not a valid weight!");
                        continue;
                    }
                } 
                else
                {
                    try 
                    {
                        sel.AddChoice(int.Parse(options[0]), 1);
                    } catch (FormatException) {
                        Log.LogError("Invalid Config! Configuration option " + s + " in " + input + " is not a valid weight!");
                        continue;
                    }
                }
            }
            if (sel.Count == 0)
            {
                sel.AddChoice(3, 1);
            }
            return sel;
        }

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
            Garbage = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGolem");
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
using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

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
        public const string PluginVersion = "1.0.0";

        public static WeightedSelection<RoR2.PickupIndex> Tier1 = null;
        public static WeightedSelection<RoR2.PickupIndex> Tier2 = null;
        public static WeightedSelection<RoR2.PickupIndex> Tier3 = null;
        public static WeightedSelection<RoR2.PickupIndex> Void1 = null;
        public static WeightedSelection<RoR2.PickupIndex> Void2 = null;
        public static WeightedSelection<RoR2.PickupIndex> Void3 = null;
        public static WeightedSelection<RoR2.PickupIndex> Boss = null;
        public static WeightedSelection<RoR2.PickupIndex> Lunar = null;
        public static WeightedSelection<RoR2.PickupIndex> Equipment = null;
        public static GameObject Prefab = null;


        public void Awake()
        {
            Log.Init(Logger);

            On.RoR2.Artifacts.CommandArtifactManager.OnDropletHitGroundServer += CommandOveride;

            Log.LogInfo(nameof(Awake) + " done.");
        }

        public static void SetupSelections() {
            Tier1 = CreateSelection(RoR2.Run.instance.availableTier1DropList);
            Tier2 = CreateSelection(RoR2.Run.instance.availableTier2DropList);
            Tier3 = CreateSelection(RoR2.Run.instance.availableTier3DropList);
            Void1 = CreateSelection(RoR2.Run.instance.availableVoidTier1DropList);
            Void2 = CreateSelection(RoR2.Run.instance.availableVoidTier2DropList);
            Void3 = CreateSelection(RoR2.Run.instance.availableVoidTier3DropList);
            Boss = CreateSelection(RoR2.Run.instance.availableBossDropList);
            Lunar = CreateSelection(RoR2.Run.instance.availableLunarItemDropList);
            Equipment = CreateSelection(RoR2.Run.instance.availableEquipmentDropList);
            Prefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/OptionPickup");
        }

        public static void CommandOveride(
            On.RoR2.Artifacts.CommandArtifactManager.orig_OnDropletHitGroundServer orig, 
            ref RoR2.GenericPickupController.CreatePickupInfo pickupInfo, 
            ref bool shouldSpawn
        ) 
        {
            if (Tier1 == null)
            {
                // Check to make sure everything is loaded in
                // TODO Make this check happen with `get`
                SetupSelections();
            }
			PickupIndex pickupIndex = pickupInfo.pickupIndex;
			PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
			if (pickupDef == null || (pickupDef.itemIndex == ItemIndex.None && pickupDef.equipmentIndex == EquipmentIndex.None && pickupDef.itemTier == ItemTier.NoTier))
			{
				return;
			}
            Xoroshiro128Plus rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);

            ItemTier tier = pickupDef.itemTier;
            WeightedSelection<RoR2.PickupIndex> list = null;
            switch (tier)
            {
                case ItemTier.Tier1:
                    list = Tier1;
                    break;
                case ItemTier.Tier2:
                    list = Tier2;
                    break;
                case ItemTier.Tier3:
                    list = Tier3;
                    break;
                case ItemTier.Boss:
                    list = Boss;
                    break;
                case ItemTier.Lunar:
                    list = Lunar;
                    break;
                case ItemTier.NoTier:
                    list = Equipment;
                    break;
                case ItemTier.VoidTier1:
                    list = Void1;
                    break;
                case ItemTier.VoidTier2:
                    list = Void2;
                    break;
                case ItemTier.VoidTier3:
                    list = Void2;
                    break;
                default:
                    orig(ref pickupInfo, ref shouldSpawn);
                    return;
            }
            RoR2.GenericPickupController.CreatePickupInfo created = new RoR2.GenericPickupController.CreatePickupInfo {
			    pickerOptions = RoR2.PickupPickerController.GenerateOptionsFromArray(
                    RoR2.PickupDropTable.GenerateUniqueDropsFromWeightedSelection(3, rng, list)
                ),
				prefabOverride = Prefab,
				position = pickupInfo.position,
				rotation = Quaternion.identity,
				pickupIndex = RoR2.PickupCatalog.FindPickupIndex(tier)
			};
            pickupInfo = created;
        }

        public static void CreateDrop(RoR2.GenericPickupController.CreatePickupInfo pickupInfo) 
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PickupDropletController.pickupDropletPrefab, pickupInfo.position, Quaternion.identity);
			PickupDropletController component = gameObject.GetComponent<PickupDropletController>();
			component.createPickupInfo = pickupInfo;
			component.NetworkpickupIndex = pickupInfo.pickupIndex;
			NetworkServer.Spawn(gameObject);
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

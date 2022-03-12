using System.Collections.Generic;
using R2API;
using RoR2;
using UnityEngine;

namespace CommandPotential
{

    // Based off of https://github.com/Moffein/Risky_Artifacts/blob/master/Risky_Artifacts/Artifacts/Conformity.cs
    public class InfluenceArtifact
    {

        public static ArtifactDef Artifact;
        
        public InfluenceArtifact()
        {
            LanguageAPI.Add("COMMANDPOTENTIAL_INFLUENCE_NAME", "Artifact of Influence");
            LanguageAPI.Add("COMMANDPOTENTIAL_INFLUENCE_DESC", "Items are replaced with Void Potential.");

            Artifact = ScriptableObject.CreateInstance<ArtifactDef>();
            Artifact.cachedName = "ArtifactOfInfluence";
            Artifact.nameToken = "COMMANDPOTENTIAL_INFLUENCE_NAME";
            Artifact.descriptionToken = "COMMANDPOTENTIAL_INFLUENCE_DESC";
            Artifact.smallIconSelectedSprite = CommandPotential.AssetBundle.LoadAsset<Sprite>("texInfluenceEnabled.png");
            Artifact.smallIconDeselectedSprite = CommandPotential.AssetBundle.LoadAsset<Sprite>("texInfluenceDisabled.png");
            if (!CommandPotential.OverrideCommand.Value)
            {
                ContentAddition.AddArtifactDef(Artifact);
                PickupDropletController.onDropletHitGroundServer += OnDropletHitGroundServer;
            }
        }

        public static void OnDropletHitGroundServer(ref GenericPickupController.CreatePickupInfo createPickupInfo, ref bool shouldSpawn)
        {
            if (CommandPotential.OverrideCommand.Value)
            {
                return;
            }
            InfluenceDroplet(ref createPickupInfo, ref shouldSpawn);
        }

        public static bool InfluenceDroplet(ref GenericPickupController.CreatePickupInfo pickupInfo, ref bool shouldSpawn)
        {
			PickupIndex pickupIndex = pickupInfo.pickupIndex;
			PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
			if (pickupDef == null || (pickupDef.itemIndex == ItemIndex.None && pickupDef.equipmentIndex == EquipmentIndex.None && pickupDef.itemTier == ItemTier.NoTier))
			{
				return true;
			}
            Xoroshiro128Plus rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);

            ItemTier tier = pickupDef.itemTier;
            WeightedSelection<RoR2.PickupIndex> list = null;
            switch (tier)
            {
                case ItemTier.Tier1:
                    list = CommandPotential.Tier1;
                    break;
                case ItemTier.Tier2:
                    list = CommandPotential.Tier2;
                    break;
                case ItemTier.Tier3:
                    list = CommandPotential.Tier3;
                    break;
                case ItemTier.Boss:
                    list = CommandPotential.Boss;
                    break;
                case ItemTier.Lunar:
                    list = CommandPotential.Lunar;
                    break;
                case ItemTier.NoTier:
                    if (RoR2.Run.instance.availableLunarEquipmentDropList.Contains(pickupIndex)) {
                        list = CommandPotential.LunarEquipment;
                    } else {
                        list = CommandPotential.Equipment;
                    }
                    break;
                case ItemTier.VoidTier1:
                    list = CommandPotential.Void1;
                    break;
                case ItemTier.VoidTier2:
                    list = CommandPotential.Void2;
                    break;
                case ItemTier.VoidTier3:
                    list = CommandPotential.Void3;
                    break;
                default:
                    return false;
            }
            List<RoR2.PickupIndex> items = new List<RoR2.PickupIndex>(RoR2.PickupDropTable.GenerateUniqueDropsFromWeightedSelection(3, rng, list));
            items.RemoveAll((item) => item == pickupIndex);
            items = items.GetRange(0, 2);
            items.Insert(0, pickupIndex);
            RoR2.GenericPickupController.CreatePickupInfo created = new RoR2.GenericPickupController.CreatePickupInfo {
			    pickerOptions = RoR2.PickupPickerController.GenerateOptionsFromArray(items.ToArray()),
				prefabOverride = CommandPotential.Prefab,
				position = pickupInfo.position,
				rotation = Quaternion.identity,
				pickupIndex = RoR2.PickupCatalog.FindPickupIndex(tier)
			};
            pickupInfo = created;
            return true;
        }
    }

}
using System;
using System.Collections.Generic;
using R2API;
using RoR2;
using RoR2.Artifacts;
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
            Artifact.smallIconSelectedSprite = Storage.AssetBundle.LoadAsset<Sprite>("texInfluenceEnabled.png");
            Artifact.smallIconDeselectedSprite = Storage.AssetBundle.LoadAsset<Sprite>("texInfluenceDisabled.png");
            if (!Storage.OverrideCommand.Value)
            {
                ContentAddition.AddArtifactDef(Artifact);
                PickupDropletController.onDropletHitGroundServer += OnDropletHitGroundServer;
                SceneDirector.onGenerateInteractableCardSelection += OnGenerateInteractableCardSelection;
            }
        }

        public static void OnDropletHitGroundServer(ref GenericPickupController.CreatePickupInfo createPickupInfo, ref bool shouldSpawn)
        {
            if (Storage.OverrideCommand.Value || !RunArtifactManager.instance.IsArtifactEnabled(Artifact.artifactIndex))
            {
                return;
            }
            InfluenceDroplet(ref createPickupInfo, ref shouldSpawn);
        }

        public static void OnGenerateInteractableCardSelection(SceneDirector sceneDirector, DirectorCardCategorySelection dccs) 
        {
            if (RunArtifactManager.instance.IsArtifactEnabled(Artifact.artifactIndex) && !Storage.SpawnMultiShops.Value) 
            {
                CommandArtifactManager.OnGenerateInteractableCardSelection(sceneDirector, dccs);
            }
        }

        public static bool IsOn()
        {
            return 
            (Storage.OverrideCommand.Value && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Command.artifactIndex))
            ||
            (!Storage.OverrideCommand.Value && RunArtifactManager.instance.IsArtifactEnabled(Artifact.artifactIndex));
        }

        public static bool InfluenceDroplet(ref GenericPickupController.CreatePickupInfo pickupInfo, ref bool shouldSpawn)
        {
            if (!Storage.OverrideCommand.Value && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Command.artifactIndex) && RunArtifactManager.instance.IsArtifactEnabled(Artifact.artifactIndex))
            {
                // Command and this artifact should rely on command
                return false;
            }
            if (Storage.Garbage.Equals(pickupInfo.prefabOverride)) 
            {
                pickupInfo.prefabOverride = null;
                return false;
            }

            if (!Storage.EnabledInBazaar.Value && BazaarController.instance != null)
            {
                return false;
            }

			PickupIndex pickupIndex = pickupInfo.pickupIndex;
			PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
			if (pickupDef == null || (pickupDef.itemIndex == ItemIndex.None && pickupDef.equipmentIndex == EquipmentIndex.None && pickupDef.itemTier == ItemTier.NoTier))
			{
				return true;
			}

            if (ItemUtil.IsItemScrap(pickupIndex))
            {
                return false;
            }

            int amount = ItemUtil.GetAmountFromIndex(pickupIndex);

            if (amount <= 1)
            {
                return false;
            }
            if (amount > 1000)
            {
                // Spawn command object
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(CommandArtifactManager.commandCubePrefab, pickupInfo.position, pickupInfo.rotation);
			    gameObject.GetComponent<PickupIndexNetworker>().NetworkpickupIndex = pickupIndex;
			    gameObject.GetComponent<PickupPickerController>().SetOptionsFromPickupForCommandArtifact(pickupIndex);
			    UnityEngine.Networking.NetworkServer.Spawn(gameObject);
			    shouldSpawn = false;
                return true;
            }

            Xoroshiro128Plus rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
            ItemTier tier = pickupDef.itemTier;
            WeightedSelection<RoR2.PickupIndex> list = ItemUtil.GetItemsFromIndex(pickupIndex);
            amount = Math.Min(amount, list.Count);

            PickupIndex tierIndex = RoR2.PickupCatalog.FindPickupIndex(tier);

            List<RoR2.PickupIndex> items = null;
            items = new List<RoR2.PickupIndex>(RoR2.PickupDropTable.GenerateUniqueDropsFromWeightedSelection(amount, rng, list));
            if (pickupIndex != tierIndex)
            {
                items.RemoveAll((item) => item == pickupIndex);
                items = items.GetRange(0, amount - 1);
                items.Insert(0, pickupIndex);
            }
            RoR2.GenericPickupController.CreatePickupInfo created = new RoR2.GenericPickupController.CreatePickupInfo {
			    pickerOptions = RoR2.PickupPickerController.GenerateOptionsFromArray(items.ToArray()),
				prefabOverride = Storage.Prefab,
				position = pickupInfo.position,
				rotation = Quaternion.identity,
				pickupIndex = RoR2.PickupCatalog.FindPickupIndex(tier)
			};
            pickupInfo = created;
            return true;
        }
    }

}
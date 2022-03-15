using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using System.Reflection;

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
        public const string PluginVersion = "1.4.0";


        public void Awake()
        {
            Log.Init(Logger);
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommandPotential.commandpotentialbundle"))
            {
                Storage.AssetBundle = AssetBundle.LoadFromStream(stream);
            }
            InitConfig();

            On.RoR2.Artifacts.CommandArtifactManager.OnDropletHitGroundServer += CommandOveride;
            On.RoR2.Artifacts.CommandArtifactManager.OnGenerateInteractableCardSelection += CommandSpawnOverride;
            On.RoR2.ShopTerminalBehavior.DropPickup += OverrideDropPickup;
            On.RoR2.Run.Start += (orig, self) => {
                orig(self);
                Storage.SetupSelections();
            };

            new InfluenceArtifact();

            Log.LogInfo(nameof(Awake) + " done.");
        }

        public void InitConfig()
        {
            Storage.OverrideCommand = Config.Bind(
                "Settings",
                "OverrideCommand",
                false,
                "If enabled it will override the command artifact instead of creating a new one and make this work server-side"
            );
            Storage.EnabledInBazaar = Config.Bind(
                "Settings",
                "EnabledInBazaar",
                true,
                "If enabled the artifact works in the Bazaar"
            );
            Storage.SpawnMultiShops = Config.Bind(
                "Settings",
                "SpawnMultiShops",
                true,
                "Allows MultiShops, scrappers, and printers to spawn when artifact is enabled. If disabled, interactable spawning works like Command."
            );
            Storage.PrintersOnlyDropOne = Config.Bind(
                "Settings",
                "PrintersOnlyDropOne",
                true,
                "Makes it so void potential cannot be dropped from printers."
            );
            Storage.Tier1OptionsConfig = Config.Bind(
                "Settings",
                "Common Options",
                "3",
                "Amount of options that display in a common Void Potential. If 1 it shows the normal item, if over 1000 it's a command item." 
                + " You can supply multiple options that will be selected randomly. Use syntax `<options>|<weight>,<options>|weight>...` (i.e. "
                + "1|80,2|20 - This will have 80% chance for 1 item, 20% chance for 2.)"
            );
            Storage.Tier2OptionsConfig = Config.Bind(
                "Settings",
                "Uncommon Options",
                "3",
                "Amount of options that display in an uncommon Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
                + " You can supply multiple options that will be selected randomly. Use syntax `<options>|<weight>,<options>|weight>...` (i.e. "
                + "1|80,2|20 - This will have 80% chance for 1 item, 20% chance for 2.)"
            );
            Storage.Tier3OptionsConfig = Config.Bind(
                "Settings",
                "Legendary Options",
                "3",
                "Amount of options that display in a legendary Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
                + " You can supply multiple options that will be selected randomly. Use syntax `<options>|<weight>,<options>|weight>...` (i.e. "
                + "1|80,2|20 - This will have 80% chance for 1 item, 20% chance for 2.)"
            );
            Storage.BossOptionsConfig = Config.Bind(
                "Settings",
                "Boss Options",
                "3",
                "Amount of options that display in a boss Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
                + " You can supply multiple options that will be selected randomly. Use syntax `<options>|<weight>,<options>|weight>...` (i.e. "
                + "1|80,2|20 - This will have 80% chance for 1 item, 20% chance for 2.)"
            );
            Storage.Void1OptionsConfig = Config.Bind(
                "Settings",
                "Void Common Options",
                "3",
                "Amount of options that display in a common Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
                + " You can supply multiple options that will be selected randomly. Use syntax `<options>|<weight>,<options>|weight>...` (i.e. "
                + "1|80,2|20 - This will have 80% chance for 1 item, 20% chance for 2.)"
            );
            Storage.Void2OptionsConfig = Config.Bind(
                "Settings",
                "Void Uncommon Options",
                "3",
                "Amount of options that display in an uncommon Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
                + " You can supply multiple options that will be selected randomly. Use syntax `<options>|<weight>,<options>|weight>...` (i.e. "
                + "1|80,2|20 - This will have 80% chance for 1 item, 20% chance for 2.)"
            );
            Storage.Void3OptionsConfig = Config.Bind(
                "Settings",
                "Void Legendary Options",
                "3",
                "Amount of options that display in a legendary Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
                + " You can supply multiple options that will be selected randomly. Use syntax `<options>|<weight>,<options>|weight>...` (i.e. "
                + "1|80,2|20 - This will have 80% chance for 1 item, 20% chance for 2.)"
            );
            Storage.EquipmentOptionsConfig = Config.Bind(
                "Settings",
                "Equipment Options",
                "3",
                "Amount of options that display in an equipment Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
                + " You can supply multiple options that will be selected randomly. Use syntax `<options>|<weight>,<options>|weight>...` (i.e. "
                + "1|80,2|20 - This will have 80% chance for 1 item, 20% chance for 2.)"
            );
            Storage.LunarEquipmentOptionsConfig = Config.Bind(
                "Settings",
                "Lunar Equipment Options",
                "3",
                "Amount of options that display in a lunar equipment Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
                + " You can supply multiple options that will be selected randomly. Use syntax `<options>|<weight>,<options>|weight>...` (i.e. "
                + "1|80,2|20 - This will have 80% chance for 1 item, 20% chance for 2.)"
            );
            Storage.LunarOptionsConfig = Config.Bind(
                "Settings",
                "Lunar Options",
                "3",
                "Amount of options that display in a lunar item Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
                + " You can supply multiple options that will be selected randomly. Use syntax `<options>|<weight>,<options>|weight>...` (i.e. "
                + "1|80,2|20 - This will have 80% chance for 1 item, 20% chance for 2.)"
            );


            Storage.InitConfig();
        }


        // Based off of https://github.com/FunkFrog/ShareSuite/blob/master/ShareSuite/ItemSharingHooks.cs#L228
        // Under GPL-v3
        public static void OverrideDropPickup(
            On.RoR2.ShopTerminalBehavior.orig_DropPickup orig,
            ShopTerminalBehavior self
        )
        {
            CostTypeIndex costType = self.GetComponent<PurchaseInteraction>().costType;
            if (!Storage.PrintersOnlyDropOne.Value || !Storage.PrinterObjects.Contains(costType) || !InfluenceArtifact.IsOn())
            {
                orig(self);
                return;
            }
            self.SetHasBeenPurchased(true);
            var baseObj = (Component) self;
            PickupDropletController.CreatePickupDroplet(new GenericPickupController.CreatePickupInfo
			    {
				    rotation = Quaternion.identity,
				    pickupIndex = self.pickupIndex,
                    prefabOverride = Storage.Garbage,
			},  (self.dropTransform ? self.dropTransform : baseObj.transform).position, baseObj.transform.TransformVector(self.dropVelocity));
        }


        public static void CommandOveride(
            On.RoR2.Artifacts.CommandArtifactManager.orig_OnDropletHitGroundServer orig, 
            ref RoR2.GenericPickupController.CreatePickupInfo pickupInfo, 
            ref bool shouldSpawn
        ) 
        {
            if (!Storage.OverrideCommand.Value)
            {
                orig(ref pickupInfo, ref shouldSpawn);
                return;
            }
            InfluenceArtifact.InfluenceDroplet(ref pickupInfo, ref shouldSpawn);
        }

        public static void CommandSpawnOverride(
            On.RoR2.Artifacts.CommandArtifactManager.orig_OnGenerateInteractableCardSelection orig,
            SceneDirector sceneDirector, 
            DirectorCardCategorySelection dccs
        ) 
        {
            if (!Storage.OverrideCommand.Value && !RunArtifactManager.instance.IsArtifactEnabled(InfluenceArtifact.Artifact.artifactIndex))
            {
                orig(sceneDirector, dccs);
                return;
            }
            if (!Storage.SpawnMultiShops.Value) 
            {
                orig(sceneDirector, dccs);
            }
        }

    }
}

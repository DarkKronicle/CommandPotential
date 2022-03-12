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
        public const string PluginVersion = "1.3.0";


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
            Storage.Tier1Options = Config.Bind(
                "Settings",
                "Common Options",
                3,
                "Amount of options that display in a common Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
            );
            Storage.Tier2Options = Config.Bind(
                "Settings",
                "Uncommon Options",
                3,
                "Amount of options that display in an uncommon Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
            );
            Storage.Tier3Options = Config.Bind(
                "Settings",
                "Legendary Options",
                3,
                "Amount of options that display in a legendary Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
            );
            Storage.BossOptions = Config.Bind(
                "Settings",
                "Boss Options",
                3,
                "Amount of options that display in a boss Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
            );
            Storage.Void1Options = Config.Bind(
                "Settings",
                "Void Common Options",
                3,
                "Amount of options that display in a common Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
            );
            Storage.Void2Options = Config.Bind(
                "Settings",
                "Void Uncommon Options",
                3,
                "Amount of options that display in an uncommon Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
            );
            Storage.Void3Options = Config.Bind(
                "Settings",
                "Void Legendary Options",
                3,
                "Amount of options that display in a legendary Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
            );
            Storage.EquipmentOptions = Config.Bind(
                "Settings",
                "Equipment Options",
                3,
                "Amount of options that display in an equipment Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
            );
            Storage.LunarEquipmentOptions = Config.Bind(
                "Settings",
                "Lunar Equipment Options",
                3,
                "Amount of options that display in a lunar equipment Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
            );
            Storage.LunarOptions = Config.Bind(
                "Settings",
                "Lunar Options",
                3,
                "Amount of options that display in a lunar item Void Potential. If 1 it shows the normal item, if over 1000 it's a command item."
            );
        }


        public static void CommandOveride(
            On.RoR2.Artifacts.CommandArtifactManager.orig_OnDropletHitGroundServer orig, 
            ref RoR2.GenericPickupController.CreatePickupInfo pickupInfo, 
            ref bool shouldSpawn
        ) 
        {
            if (!Storage.OverrideCommand.Value || !InfluenceArtifact.InfluenceDroplet(ref pickupInfo, ref shouldSpawn))
            {
                return;
            }
        }

        public static void CommandSpawnOverride(
            On.RoR2.Artifacts.CommandArtifactManager.orig_OnGenerateInteractableCardSelection orig,
            SceneDirector sceneDirector, 
            DirectorCardCategorySelection dccs
        ) 
        {
            if (!Storage.SpawnMultiShops.Value) 
            {
                orig(sceneDirector, dccs);
            }
        }

    }
}

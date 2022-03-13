using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoR2;

namespace CommandPotential
{
    public class ItemUtil
    {
        
        private ItemUtil() {}

        public static WeightedSelection<PickupIndex> GetItemsFromIndex(PickupIndex index) 
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(index);
            ItemTier tier = pickupDef.itemTier;
            switch (tier)
            {
                case ItemTier.Tier1:
                    return Storage.Tier1;
                case ItemTier.Tier2:
                    return Storage.Tier2;
                case ItemTier.Tier3:
                    return Storage.Tier3;
                case ItemTier.Boss:
                    return Storage.Boss;
                case ItemTier.Lunar:
                    return Storage.Lunar;
                case ItemTier.NoTier:
                    // Lunar equipment is apparently not cool enough to be a tier
                    if (RoR2.Run.instance.availableLunarEquipmentDropList.Contains(index)) {
                        return Storage.LunarEquipment;
                    } else {
                        return Storage.Equipment;
                    }
                case ItemTier.VoidTier1:
                    return Storage.Void1;
                case ItemTier.VoidTier2:
                    return Storage.Void2;
                case ItemTier.VoidTier3:
                    return Storage.Void3;
                default:
                    return null;
            } 
        }

        public static int GetAmountFromIndex(PickupIndex index)
        {
            WeightedSelection<int> selection = GetAmountSelectionFromIndex(index);
            if (selection == null)
            {
                return 1;
            }
            Xoroshiro128Plus rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
            return selection.Evaluate(rng.nextNormalizedFloat);
        }

        public static WeightedSelection<int> GetAmountSelectionFromIndex(PickupIndex index)
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(index);
            ItemTier tier = pickupDef.itemTier;
            switch (tier)
            {
                case ItemTier.Tier1:
                    return Storage.Tier1Options;
                case ItemTier.Tier2:
                    return Storage.Tier2Options;
                case ItemTier.Tier3:
                    return Storage.Tier3Options;
                case ItemTier.Boss:
                    return Storage.BossOptions;
                case ItemTier.Lunar:
                    return Storage.LunarOptions;
                case ItemTier.NoTier:
                    // Lunar equipment is apparently not cool enough to be a tier
                    if (RoR2.Run.instance.availableLunarEquipmentDropList.Contains(index)) {
                        return Storage.LunarEquipmentOptions;
                    } else {
                        return Storage.EquipmentOptions;
                    }
                case ItemTier.VoidTier1:
                    return Storage.Void1Options;
                case ItemTier.VoidTier2:
                    return Storage.Void2Options;
                case ItemTier.VoidTier3:
                    return Storage.Void3Options;
                default:
                    return null;
            } 
        }

        public static bool IsItemScrap(PickupIndex index)
        {
            if (index.itemIndex.Equals(RoR2Content.Items.ScrapWhite.itemIndex))
            {
                return true;
            }
            if (index.itemIndex.Equals(RoR2Content.Items.ScrapGreen.itemIndex))
            {
                return true;
            }
            if (index.itemIndex.Equals(RoR2Content.Items.ScrapRed.itemIndex))
            {
                return true;
            }
            if (index.itemIndex.Equals(RoR2Content.Items.ScrapYellow.itemIndex))
            {
                return true;
            }
            return false;
        }
    }


}
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
            PickupDef pickupDef = PickupCatalog.GetPickupDef(index);
            ItemTier tier = pickupDef.itemTier;
            switch (tier)
            {
                case ItemTier.Tier1:
                    return Storage.Tier1Options.Value;
                case ItemTier.Tier2:
                    return Storage.Tier2Options.Value;
                case ItemTier.Tier3:
                    return Storage.Tier3Options.Value;
                case ItemTier.Boss:
                    return Storage.BossOptions.Value;
                case ItemTier.Lunar:
                    return Storage.LunarOptions.Value;
                case ItemTier.NoTier:
                    // Lunar equipment is apparently not cool enough to be a tier
                    if (RoR2.Run.instance.availableLunarEquipmentDropList.Contains(index)) {
                        return Storage.LunarEquipmentOptions.Value;
                    } else {
                        return Storage.EquipmentOptions.Value;
                    }
                case ItemTier.VoidTier1:
                    return Storage.Void1Options.Value;
                case ItemTier.VoidTier2:
                    return Storage.Void2Options.Value;
                case ItemTier.VoidTier3:
                    return Storage.Void3Options.Value;
                default:
                    return 1;
            } 
        }
    }


}
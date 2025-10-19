using System.Collections.Generic;

public static class AdWeight {
    public static Dictionary<AdType, AdRarityStruct> Weights { get; private set; } = new() {
        {
            AdType.None,
            default
        },
        {
            AdType.Static,
            new AdRarityStruct(AdRarity.Common)
        },
        {
            AdType.RandomMove,
            new AdRarityStruct(AdRarity.Common)
        },
        {
            AdType.Split,
            new AdRarityStruct(AdRarity.Epic)
        },
        {
            AdType.CursorFollower,
            new AdRarityStruct(AdRarity.Epic)
        },
        {
            AdType.ShrinkOverTime,
            new AdRarityStruct(AdRarity.Rare)
        },
        {
            AdType.ExpandOverTime,
            new AdRarityStruct(AdRarity.Rare)
        },
        {
            AdType.RotationAd,
            new AdRarityStruct(AdRarity.Epic)
        },
        {
            AdType.FallingAd,
            new AdRarityStruct(AdRarity.Rare)
        },
    };
    
    public enum AdRarity {
        Common,
        Rare,
        Epic
    }
    
    public struct AdRarityStruct {
        public AdRarity Rarity;
        public int Weight;

        public AdRarityStruct(AdRarity rarity) {
            Rarity = rarity;
            Weight = GetWeightByRarity(rarity);
        }
    }

    private static int GetWeightByRarity(AdRarity rarity) {
        return rarity switch {
            AdRarity.Common => 50,
            AdRarity.Rare => 20,
            AdRarity.Epic => 10,
            _ => 0
        };
    }
        
    
}

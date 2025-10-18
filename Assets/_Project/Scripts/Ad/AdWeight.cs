using System.Collections.Generic;

public static class AdWeight {
    public static Dictionary<AdType, int> Weights { get; private set; } = new() {
        { AdType.None, 0 },
        { AdType.Static, 50 },
        { AdType.Moveable, 30 },
        { AdType.Split, 15 }
    };
}

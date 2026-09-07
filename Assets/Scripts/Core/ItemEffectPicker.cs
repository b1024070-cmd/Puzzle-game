using UnityEngine;

// 好影響アイテムの抽選ロジック。確率は固定値でシンプルに管理する。
public static class ItemEffectPicker
{
    private struct WeightedEffect
    {
        public ItemEffectType type;
        public float weight;
    }

    private static readonly WeightedEffect[] goodEffects =
    {
        new WeightedEffect{ type = ItemEffectType.WallBreak,  weight = 35f },
        new WeightedEffect{ type = ItemEffectType.TimeExtend, weight = 35f },
        new WeightedEffect{ type = ItemEffectType.SpeedUp,    weight = 30f },
    };

    public static ItemEffectType PickGoodEffect()
    {
        float total = 0f;
        foreach (var e in goodEffects) total += e.weight;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var e in goodEffects)
        {
            cumulative += e.weight;
            if (roll <= cumulative) return e.type;
        }

        return goodEffects[goodEffects.Length - 1].type;
    }
}
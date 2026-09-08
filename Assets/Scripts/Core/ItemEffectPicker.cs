using UnityEngine;

// 宝箱アイテムの抽選ロジック。確率は固定値でシンプルに管理する。
public static class ItemEffectPicker
{
    private struct WeightedEffect
    {
        public ItemEffectType type;
        public float weight;
    }

    // 好影響グループ
    private static readonly WeightedEffect[] goodEffects =
    {
        new WeightedEffect{ type = ItemEffectType.WallBreak,  weight = 35f },
        new WeightedEffect{ type = ItemEffectType.TimeExtend, weight = 35f },
        new WeightedEffect{ type = ItemEffectType.SpeedUp,    weight = 30f },
    };

    // 妨害グループ
    private static readonly WeightedEffect[] badEffects =
    {
        new WeightedEffect{ type = ItemEffectType.SpeedDown, weight = 50f },
        new WeightedEffect{ type = ItemEffectType.AddWall,   weight = 50f },
    };

    // 好影響 / 妨害 のどちらが出るかの比率(好影響を少し優先)
    private const float GoodChance = 60f;
    private const float BadChance = 40f;

    // 宝箱を開けた時に呼ぶ、統合された抽選エントリーポイント
    public static ItemEffectType PickEffect()
    {
        float roll = Random.Range(0f, GoodChance + BadChance);
        return roll <= GoodChance ? PickFrom(goodEffects) : PickFrom(badEffects);
    }

    private static ItemEffectType PickFrom(WeightedEffect[] effects)
    {
        float total = 0f;
        foreach (var e in effects) total += e.weight;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var e in effects)
        {
            cumulative += e.weight;
            if (roll <= cumulative) return e.type;
        }

        return effects[effects.Length - 1].type;
    }
}
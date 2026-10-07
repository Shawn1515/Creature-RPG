public static class TypeChart
{
    public static float GetMultiplier(
        CreatureType attack,
        CreatureType defense)
    {
        if (attack == CreatureType.Fire &&
            defense == CreatureType.Grass)
            return 2f;

        if (attack == CreatureType.Grass &&
            defense == CreatureType.Fire)
            return 0.5f;

        if (attack == CreatureType.Water &&
            defense == CreatureType.Fire)
            return 2f;

        if (attack == CreatureType.Fire &&
            defense == CreatureType.Water)
            return 0.5f;

        if (attack == CreatureType.Grass &&
            defense == CreatureType.Water)
            return 2f;

        if (attack == CreatureType.Water &&
            defense == CreatureType.Grass)
            return 0.5f;

        if (attack == CreatureType.Electric &&
            defense == CreatureType.Steel)
            return 2f;

        if (attack == CreatureType.Steel &&
            defense == CreatureType.Electric)
            return 0.5f;

        if (attack == CreatureType.Steel &&
            defense == CreatureType.Ice)
            return 2f;

        if (attack == CreatureType.Ice &&
            defense == CreatureType.Steel)
            return 0.5f;

        if (attack == CreatureType.Ice &&
            defense == CreatureType.Electric)
            return 2f;

        if (attack == CreatureType.Electric &&
            defense == CreatureType.Ice)
            return 0.5f;

        return 1f;
    }
}
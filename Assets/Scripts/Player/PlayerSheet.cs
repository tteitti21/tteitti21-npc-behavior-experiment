using UnityEngine;
using static RelationshipDatabase;

public class PlayerSheet : CharacterStats
{
    [Header("Player Specific")]
    public static PlayerSheet Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        npcId = "Player";
        faction = Faction.Alliance;
        characterName = "Tobias";
        Health.Set(Health.Max);
        Tiredness.Set(Tiredness.Max);
        Hunger.Set(Hunger.Max);
    }

    public override void TickDaily()
    {
        base.TickDaily();
        // Player-specific daily logic
        Debug.Log($"{CharacterName} feels the passing of another day...");
    }
}
using UnityEngine;
using static RelationshipDatabase;

public class HerbalistSheet : CharacterStats
{


    void Awake()
    {
        characterName = "Misha";
        npcId = "Misha";
        faction = Faction.Alliance;

        RestoreHealt(100);
        RestoreHunger(100);
        RestoreTiredness(100);
    }

    public override void TickDaily()
    {
        base.TickDaily();
        // Player-specific daily logic
        Debug.Log($"{CharacterName} feels the passing of another day...");
    }
}

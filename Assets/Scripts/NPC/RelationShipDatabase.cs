using System.Collections.Generic;
using UnityEngine;

public static class RelationshipDatabase
{
    public enum Faction { Player, Alliance, Enemy, Beast }

    // Default faction vs faction
    public static readonly Dictionary<(Faction, Faction), string> FactionRelationships = new()
    {
        { (Faction.Player, Faction.Alliance), "friendly" },
        { (Faction.Player, Faction.Enemy), "hostile" },
        { (Faction.Player, Faction.Beast), "hostile" },
        { (Faction.Alliance, Faction.Player), "friendly" },
        { (Faction.Enemy, Faction.Player), "hostile" },
        { (Faction.Beast, Faction.Player), "hostile" },
    };

    // Specific character vs character (by unique IDs)
    private static readonly Dictionary<(string, string), string> SpecificRelationships = new()
{
    { ("Player", "Misha"), "friendly" },
    { ("Player", "Bandit01"), "hostile" },
    { ("Misha", "Guard01"), "neutral" }
};

    /// <summary>
    /// Get the relationship between two entities.
    /// Priority: Specific -> Faction defaults -> Neutral.
    /// </summary>
    public static string GetRelationship(string idA, Faction factionA, string idB, Faction factionB)
    {
        // 1. Check if specific relationship exists
        if (SpecificRelationships.TryGetValue((idA, idB), out string relation))
            return relation;

        if (SpecificRelationships.TryGetValue((idB, idA), out relation))
            return relation; // symmetrical

        // 2. Fallback to faction-level
        if (FactionRelationships.TryGetValue((factionA, factionB), out relation))
            return relation;

        if (FactionRelationships.TryGetValue((factionB, factionA), out relation))
            return relation;

        // 3. Default neutral if nothing found
        return "neutral";
    }

    /// <summary>
    /// Set or update a specific relationship between two NPCs.
    /// </summary>
    public static void SetSpecificRelationship(string idA, string idB, string relation)
    {
        SpecificRelationships[(idA, idB)] = relation;
    }

    // Example usage:

    // Set a specific NPC vs NPC relationship
    //RelationshipDatabase.SetSpecificRelationship("Misha", "Player01", "friendly");

    // Set/modify at runtime
    //RelationshipDatabase.SetSpecificRelationship(player.NpcId, herbalist.NpcId, "hostile");

}

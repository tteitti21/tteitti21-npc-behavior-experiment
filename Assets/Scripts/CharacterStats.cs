using UnityEngine;

public abstract class CharacterStats : MonoBehaviour, ICharacterStats
{
    [Header("Core Stats")]
    private CoreStat health = new();
    private CoreStat tiredness = new();
    private CoreStat hunger = new();
    [Range(0f, 10f)][SerializeField] private float stealthLevel = 0f;
    [Range(0f, 10f)][SerializeField] private float strengthLevel = 0f;

    [SerializeField] private float pushSuccessChance = 0.3f;
    [SerializeField] private float hitChance = 0.3f;

    public float HitChance
    {
        get => hitChance;
        set => hitChance = Mathf.Clamp01(value); // keep within 0–1
    }

    [Header("Relationships and factions")]
    [SerializeField] protected string npcId; // unique per NPC (e.g. "Herbalist01")
    [SerializeField] protected RelationshipDatabase.Faction faction;
    public string NpcId => npcId;
    public RelationshipDatabase.Faction Faction => faction;

    [Header("Info")]
    [SerializeField] protected string characterName = "Unknown";

    // ✅ Properties for interface
    public CoreStat Health => health;
    public CoreStat Hunger => hunger;
    public CoreStat Tiredness => tiredness;
    public string CharacterName => characterName;
    public float StealthLevel => stealthLevel;
    public float StrengthLevel => strengthLevel;
    public float PushSuccessChance => pushSuccessChance;

    // ✅ Common behavior
    public virtual void TickDaily()
    {
        hunger.Modify(-30);
        tiredness.Modify(-30);
    }

    public virtual void RestoreHealt(float amount) =>
        health.Modify(amount);
    public virtual void RestoreHunger(float amount) =>
        hunger.Modify(amount);
    public virtual void RestoreTiredness(float amount) =>
        tiredness.Modify(amount);
    public virtual void IncreaseHitChance(float amount) =>
        hitChance += amount;
    public virtual void IncreaseStealthLevel(float amount) =>
        stealthLevel += amount;
    public virtual void IncreaseStrengthLevel(float amount) =>
        strengthLevel += amount;
}


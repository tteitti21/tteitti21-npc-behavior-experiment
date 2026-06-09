public interface ICharacterStats
{
    CoreStat Health { get; }
    CoreStat Hunger { get; }
    CoreStat Tiredness { get; }
    string CharacterName { get; }
    float StealthLevel { get; }
    float StrengthLevel { get; }
    float PushSuccessChance { get; }
    float HitChance { get; }
}

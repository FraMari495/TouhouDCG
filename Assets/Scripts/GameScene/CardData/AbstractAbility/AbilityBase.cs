using UnityEngine;


/// <summary>
/// どのタイミングで発動するか
/// </summary>
public enum AbilityType
{
    OnPlay,
    OnTurnEnd,
    OnDefeated,
    Bomb
}

public abstract class AbilityBase : ScriptableObject
{
    /// <summary>
    /// どのタイミングで発動するか
    /// </summary>
    public abstract AbilityType AbilityType { get; }

    /// <summary>
    /// アビリティーの名前
    /// </summary>
    public abstract string SkillName { get; }
}

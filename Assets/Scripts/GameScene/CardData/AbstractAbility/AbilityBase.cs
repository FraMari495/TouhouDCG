using UnityEngine;


/// <summary>
/// �ǂ̃^�C�~���O�Ŕ������邩
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
    /// �ǂ̃^�C�~���O�Ŕ������邩
    /// </summary>
    public abstract AbilityType AbilityType { get; }

    /// <summary>
    /// �A�r���e�B�[�̖��O
    /// </summary>
    public abstract string SkillName { get; }
}
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

/// <summary>
/// カードが持つアビリティーの抽象クラス
/// </summary>
public abstract class Ability : AbilityBase
{

    /// <summary>
    /// アビリティーを発動する
    /// </summary>
    /// <param name="owner">アビリティーの発動者</param>
    /// <returns></returns>
    public int[] Run(StatusBase owner,int[] indices)
    {
        //ヒストリーに追加
        HistoryDisplay.I.AddHistory(new History_Ability(owner.IsPlayer, owner, this));

        //アビリティーの本体
        var indices2 = RunAbility(owner, indices);

        AnimationManager.I.AddSequence(MakeAnimation,SkillName);

        return indices2;
    }

    /// <summary>
    /// アビリティーの本体
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    protected abstract int[] RunAbility(StatusBase owner, int[] indices);

    protected virtual Sequence MakeAnimation() => null;
}

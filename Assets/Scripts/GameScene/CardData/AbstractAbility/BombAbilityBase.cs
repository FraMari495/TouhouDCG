using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BombAbilityBase : Ability,IBombAbility
{
    public override AbilityType AbilityType => AbilityType.Bomb;

    /// <summary>
    /// for AI
    /// アビリティーの評価を計算
    /// </summary>
    /// <param name="owner">アビリティーの使用者</param>
    /// <param name="target">アビティーターゲット</param>
    /// <returns></returns>
    public abstract float CalculateScore(StatusBase owner);
}




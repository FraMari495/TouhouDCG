using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BombAbilityBase : Ability,IBombAbility
{
    public override AbilityType AbilityType => AbilityType.Bomb;

    /// <summary>
    /// for AI
    /// �A�r���e�B�[�̕]�����v�Z
    /// </summary>
    /// <param name="owner">�A�r���e�B�[�̎g�p��</param>
    /// <param name="target">�A�r�e�B�[�^�[�Q�b�g</param>
    /// <returns></returns>
    public abstract float CalculateScore(StatusBase owner);
}




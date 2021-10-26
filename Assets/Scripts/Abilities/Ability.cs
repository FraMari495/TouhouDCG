using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

/// <summary>
/// �J�[�h�����A�r���e�B�[�̒��ۃN���X
/// </summary>
public abstract class Ability : AbilityBase
{

    /// <summary>
    /// �A�r���e�B�[�𔭓�����
    /// </summary>
    /// <param name="owner">�A�r���e�B�[�̔�����</param>
    /// <returns></returns>
    public int[] Run(StatusBase owner,int[] indices)
    {
        //�q�X�g���[�ɒǉ�
        HistoryDisplay.I.AddHistory(new History_Ability(owner.IsPlayer, owner, this));

        //�A�r���e�B�[�̖{��
        var indices2 = RunAbility(owner, indices);

        AnimationManager.I.AddSequence(MakeAnimation,SkillName);

        return indices2;
    }

    /// <summary>
    /// �A�r���e�B�[�̖{��
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    protected abstract int[] RunAbility(StatusBase owner, int[] indices);

    protected virtual Sequence MakeAnimation() => null;
}

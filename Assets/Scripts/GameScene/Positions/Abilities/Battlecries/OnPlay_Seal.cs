using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����A�r���e�B�[
/// �J�[�h��������\��(RunningAbility�N���X�̒��g)��S�ď���
/// </summary>
[CreateAssetMenu(menuName = "Skill/Battlecry/Seal")]
internal class OnPlay_Seal : OnPlayAbility
{
    /// <summary>
    /// �A�r���e�B�[�̑Ώۂ̓L�����N�^�[�J�[�h
    /// </summary>
    public override CardType[] TargetType => new CardType[1] { CardType.Chara };

    public override string SkillName => "����";

    protected override float CalculateScore(StatusBase owner, StatusBase target)
    {
        float ans = -6;
        Debug.LogWarning("����U�������̗͂́A�񋓌^�ɂ��ׂ�");
        if(target is Status_Chara chara)
        {
            ans = Status_Chara.RunningAbilities.GetAbilityScore(chara);
        }
        return ans;
    }

    protected override int[] RunAbility(StatusBase owner, StatusBase targets, int[] indices)
    {
        if (targets == null) return null;
        ((Status_Chara)targets).AddEffect(StatusEffect.Sealed);
        return null;
    }
}

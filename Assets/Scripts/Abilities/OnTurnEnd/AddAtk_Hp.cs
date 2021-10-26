using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/OnTurnEnd/PowerUp")]
public class AddAtk_Hp : OnTurnEndSkill
{
    [SerializeField] private int atk;
    [SerializeField] private int hp;

    public override string SkillName => "�p���[�A�b�v";

    protected override int[] RunAbility(StatusBase targets, int[] indices)
    {
        if (targets is Status_Chara chara)
        {
            chara.AddHp(hp);
            chara.AddAtk(atk);
        }
        else
        {
            Debug.LogError("���̃A�r���e�B�[�̑Ώ̂̓L�����J�[�h�݂̂ł�");
        }
        return null;
    }
}

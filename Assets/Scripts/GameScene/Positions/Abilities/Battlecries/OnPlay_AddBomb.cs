using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Battlecry/AddBomb")]
internal class OnPlay_AddBomb : OnPlayAbility
{
    [SerializeField] private int addNumber;
    public override CardType[] TargetType => new CardType[1] { CardType.Chara };

    public override string SkillName => "ƒ{ƒ€";

    /// <summary>
    /// !!!’ˆÓ!!!
    /// ”­“®‚Å‚«‚é‚à‚Ì‚Æ‚·‚é
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    protected override float CalculateScore(StatusBase owner, StatusBase target)
    {
        if (target is Status_Chara chara && chara.CharaData.MyAbilities.BombAbility is BombAbilityBase bomb && chara.CharaData.MyAbilities.BombCost>0)
        {
            return bomb.CalculateScore(chara);
        }
        return -(int)1e8;
    }

    protected override int[] RunAbility(StatusBase owner, StatusBase targets,int[] indices)
    {
        if (targets is Status_Chara chara)
        {
            return chara.AddBomb(addNumber, indices);

        }
        else
        {
            return null;
        }
    }
}

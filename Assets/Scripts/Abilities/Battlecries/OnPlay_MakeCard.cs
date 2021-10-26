using Position;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Battlecry/MakeCard")]
public class OnPlay_MakeCard : OnPlayAbility
{
    [SerializeField] private MakingCard[] cards;

    public override string SkillName => "ãŠC–H—‰";

    public override CardType[] TargetType => null;

    protected override float CalculateScore(StatusBase owner, StatusBase target)
    {
        float ans = 0;

        foreach (var item in cards)
        {
            if(item.Card is CardBook_Chara chara)
            {
                ans += chara.Atk * 2;
                ans += chara.Hp;
            }
        }
        return ans;
    }

    protected override int[] RunAbility(StatusBase owner, StatusBase targets, int[] indices)
    {

        int ownerPosition = Position.Field.I(owner.IsPlayer).GetIndex(owner);

        Array.Sort(cards,(x,y)=>x.Right-y.Right);

        foreach (var card in cards)
        {

            IPlayable playable = card.Card.MakeCardToField(card.IsOwnerCard&&owner.IsPlayer);
            Field.I(playable.IsPlayer).SpecialSummon(card.Right + ownerPosition + 1,playable);
        }

        return null;

    }
    // Start is called before the first frame update

}

[System.Serializable]
public class MakingCard
{
    [SerializeField] private int right;
    [SerializeField] private CardBook card;
    [SerializeField] private bool isOwnerCard = true;

    public int Right { get => right;  }
    public CardBook Card { get => card;  }
    public bool IsOwnerCard { get => isOwnerCard; }
}

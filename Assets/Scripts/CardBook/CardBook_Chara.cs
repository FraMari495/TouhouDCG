using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum SpecialStatus
{
    Gardian,
    Killer,
    Surprise,
    Diffense,
    Freeze,
    Penetrate
}

public enum Race
{
    Normal,
    Fairy,
}

/// <summary>
/// カード図鑑(キャラクター用)
/// </summary>
[CreateAssetMenu(menuName = "Card/CharaCard", fileName = "new CharaCard")]
public class CardBook_Chara : CardBook
{
    #region 図鑑に載せたい項目
    [SerializeField] private int atk;
    [SerializeField] private int hp;
    [SerializeField] private OnDefeatedAbility onDefeatedSkill;
    [SerializeField] private OnTurnEndSkill onTurnEndSkill;
    [SerializeField] private BombAbilityBase bombAbility;
    [SerializeField] private int bombCost = -1;

    [SerializeField] private SpecialStatus[] specialStatuses;
    [SerializeField] private Race race = Race.Normal;



    public int Atk { get => atk; }
    public int Hp { get => hp; }
    public OnDefeatedAbility OnDefeatedSkill => onDefeatedSkill;
    public OnTurnEndSkill OnTurnEndSkill => onTurnEndSkill;
    public BombAbilityBase BombAbility => bombAbility;
    public int BombCost => bombCost;
    public Race Race => race;

    public SpecialStatus[] SpecialStatuses
    {
        get
        {
            if (specialStatuses != null) return specialStatuses;
            else return new SpecialStatus[0];
        }
    }





    #endregion

    protected override string PrefabPath => "CharacterCard";
}

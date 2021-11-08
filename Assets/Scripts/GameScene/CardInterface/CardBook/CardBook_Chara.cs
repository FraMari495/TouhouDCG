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

/// <summary>
/// �L�����J�[�h�����X�e�[�^�X
/// </summary>
public enum Parameter
{
    Cost,
    Hp,
    Atk
}

public enum Race
{
    Normal,
    Fairy,
}


//namespace Book
//{
    /// <summary>
    /// �J�[�h�}��(�L�����N�^�[�p)
    /// </summary>
    [CreateAssetMenu(menuName = "Card/CharaCard", fileName = "new CharaCard")]
    public class CardBook_Chara : CardBook
    {
        #region �}�ӂɍڂ���������
        [SerializeField] private int atk;
        [SerializeField] private int hp;
        [SerializeField] private PrefabInterface<IOnDefeatedAbility> onDefeatedSkill;
        [SerializeField] private PrefabInterface<IOnTurnEndAbility> onTurnEndSkill;
        [SerializeField] private PrefabInterface<IBombAbility> bombAbility;
        [SerializeField] private int bombCost = -1;

        [SerializeField] private SpecialStatus[] specialStatuses;
        [SerializeField] private Race race = Race.Normal;



        public int Atk { get => atk; }
        public int Hp { get => hp; }
        public IOnDefeatedAbility OnDefeatedSkill => onDefeatedSkill.GetInterface();
        public IOnTurnEndAbility OnTurnEndSkill => onTurnEndSkill.GetInterface();
        public IBombAbility BombAbility => bombAbility.GetInterface();
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

        public override string PrefabPath => "CharacterCard";
    }

//}

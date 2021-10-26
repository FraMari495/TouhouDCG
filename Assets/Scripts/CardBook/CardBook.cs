using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �J�[�h�}��(���ʕ���)
/// </summary>
public abstract class CardBook : ScriptableObject
{
    #region �}�ӂɍڂ���������(���ʕ���)
    [SerializeField] private int id;
    [SerializeField] private string cardName;
    [SerializeField] private Sprite cardImage;
    [SerializeField,TextArea(3,7)] private string description;
    [SerializeField] private int cost;
    [SerializeField] private OnPlayAbility onPlayAbility;
    [SerializeField] private float imageSizeRatio = 1;

    public int Id { get => id;}
    public string CardName { get => cardName; }
    public Sprite CardImage { get => cardImage; }
    public string Description { get => description; }
    public int Cost { get => cost;}
    public OnPlayAbility OnPlaySkill => onPlayAbility;
    public float ImageSizeRatio => imageSizeRatio;
    #endregion

    /// <summary>
    /// �v���n�u�ւ̃p�X
    /// </summary>
    protected abstract string PrefabPath { get; }

    /// <summary>
    /// �J�[�h�̃I�u�W�F�N�g�𐶐�����
    /// ���������J�[�h��Status��Ԃ�
    /// </summary>
    /// <param name="isPlayer">�v���C���[�̃J�[�h���ۂ�</param>
    private IPlayable MakeCard(bool isPlayer, int? playableId = null)
    {

        //�J�[�h�̃v���n�u���C���X�^���X��
        //�J�[�h�̎�ނɂ���āA�p����v���n�u���قȂ�
        GameObject cardObj = Instantiate(Resources.Load<GameObject>(PrefabPath), GameObject.Find("Canvas").transform, false);
        cardObj.GetComponentInChildren<CardForDeckMaking>().gameObject.SetActive(false);

        //�l�X�ȏ�����
        cardObj.GetComponentsInChildren<ICardViewInitializer>().ForEach(init => init.Initialize(isPlayer, this));

        //PlayableId��ݒ� (�J�[�h�̃C���X�^���X���ƂɈقȂ�Id��^����B�@�I�����C���ΐ�̂Ƃ��A"�����C���X�^���X"�ɂ͓���Id���ݒ肳���)
        IPlayable playable = cardObj.GetComponent<IPlayable>();
        playable.RequirePlayableId(playableId);

        //������₷���悤�ɁA�I�u�W�F�N�g���Ƃ��ăJ�[�h�̖��O��p����
        cardObj.name = CardName;

        return playable;
    }




    /// <summary>
    /// �J�[�h�̃I�u�W�F�N�g�𐶐�����
    /// ���������J�[�h��Status��Ԃ�
    /// </summary>
    /// <param name="isPlayer">�v���C���[�̃J�[�h���ۂ�</param>
    private IPlayable MakeCard()
    {

        //�J�[�h�̃v���n�u���C���X�^���X��
        //�J�[�h�̎�ނɂ���āA�p����v���n�u���قȂ�
        GameObject cardObj = Instantiate(Resources.Load<GameObject>(PrefabPath), GameObject.Find("Canvas").transform, false);
        cardObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
        Destroy(cardObj.GetComponent<StatusBase>());

        //�l�X�ȏ�����
        cardObj.GetComponent<CardInputHandler>().Initialize(true, this);
        cardObj.GetComponent<CardVisualController>().Initialize(true,this);
        //cardObj.GetComponentsInChildren<ICardViewInitializer>().ForEach(init => init.Initialize(true, this));

        //PlayableId��ݒ� (�J�[�h�̃C���X�^���X���ƂɈقȂ�Id��^����B�@�I�����C���ΐ�̂Ƃ��A"�����C���X�^���X"�ɂ͓���Id���ݒ肳���)
        IPlayable playable = cardObj.GetComponent<IPlayable>();
        // playable.RequirePlayableId(-1);

        //������₷���悤�ɁA�I�u�W�F�N�g���Ƃ��ăJ�[�h�̖��O��p����
        cardObj.name = CardName;

        return playable;
    }



    #region �J�[�h���쐬���Aposition(Deck�AHand�AField�ADiscard)�ɓn��  (public methodsh)


    public CardForDeckMaking MakeCardForDeckMaker()
    {
        //�J�[�h�̃v���n�u���C���X�^���X��
        //�J�[�h�̎�ނɂ���āA�p����v���n�u���قȂ�
        //GameObject cardObj = Instantiate(Resources.Load<GameObject>(PrefabPath), GameObject.Find("Canvas").transform, false);
        GameObject cardObj =MakeCard().GameObject;
        CardForDeckMaking card = cardObj.GetComponentInChildren<CardForDeckMaking>();
        cardObj.name = CardName;
        return card;
    }

    public IPlayable MakeCardToDeck(bool isPlayer,int pos,int? playableId =null)
    {
        return MakeCard(isPlayer, playableId);
        //c.GameObject.GetComponent<CardInputHandler>().ToDeck(pos);
        //return c;
    }

    public IPlayable MakeCardToField(bool isPlayer)
    {

       return MakeCard(isPlayer);

        //return c.GameObject.GetComponent<CardInputHandler_Chara>().ToField(pos);
    }

    public IPlayable MakeCardToHand(bool isPlayer)
    {
        return MakeCard(isPlayer);
    }

    public IPlayable MakeCardToDiscard(bool isPlayer)
    {
        return MakeCard(isPlayer);
    }
    #endregion
}

/// <summary>
/// ���������K�v��(�J�[�h��)�N���X����������C���^�[�t�F�[�X
/// </summary>
public interface ICardViewInitializer
{
    void Initialize(bool isPlayer, CardBook book);
}

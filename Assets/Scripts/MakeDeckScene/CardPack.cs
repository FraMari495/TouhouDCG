using Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPack :MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private Text numberText;
    [SerializeField] private Transform cardTrn;
    public int CardId { get; private set; }
    private int number;
    private ListType ListType { get; set; }

    public CardForDeckMaking Card { get;private set; }
    public int Number { get => number; set
        {
            numberText.text ="x"+ value.ToString();
            number = value;
        }
    }

    public void Initialize(CardBook book,ListType listType)
    {
        ListType = listType;

        CardForDeckMaking card = MakeCardForDeckMaker(book).GetComponentInChildren<CardForDeckMaking>();


        card.Initialize(book, listType);
        card.transform.parent.SetParent(cardTrn, false);
        CardId = card.CardId;
        Card = card;
        Number = 1;

        this.gameObject.AddComponent<Button>().onClick.AddListener(normalClick);
    }



    /// <summary>
    /// �J�[�h�̃I�u�W�F�N�g�𐶐�����
    /// ���������J�[�h��Status��Ԃ�
    /// </summary>
    /// <param name="isPlayer">�v���C���[�̃J�[�h���ۂ�</param>
    private IPlayable MakeCard(CardBook book)
    {

        //�J�[�h�̃v���n�u���C���X�^���X��
        //�J�[�h�̎�ނɂ���āA�p����v���n�u���قȂ�
        GameObject cardObj = Instantiate(Resources.Load<GameObject>(book.PrefabPath), GameObject.Find("Canvas").transform, false);
        cardObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
        Destroy(cardObj.GetComponent<StatusBase>());

        cardObj.GetComponent<CardInputHandler>().Initialize(true, book,true);
        cardObj.GetComponent<CardVisualController>().Initialize(true, book,true);
        //cardObj.GetComponentsInChildren<ICardViewInitializer>().ForEach(init => init.Initialize(true, book,true));

        //PlayableId��ݒ� (�J�[�h�̃C���X�^���X���ƂɈقȂ�Id��^����B�@�I�����C���ΐ�̂Ƃ��A"�����C���X�^���X"�ɂ͓���Id���ݒ肳���)
        IPlayable playable = cardObj.GetComponent<IPlayable>();
        // playable.RequirePlayableId(-1);

        //������₷���悤�ɁA�I�u�W�F�N�g���Ƃ��ăJ�[�h�̖��O��p����
        cardObj.name = playable.ToString();

        return playable;
    }


    private GameObject MakeCardForDeckMaker(CardBook book)
    {
        //�J�[�h�̃v���n�u���C���X�^���X��
        //�J�[�h�̎�ނɂ���āA�p����v���n�u���قȂ�
        //GameObject cardObj = Instantiate(Resources.Load<GameObject>(PrefabPath), GameObject.Find("Canvas").transform, false);
        GameObject cardObj = MakeCard(book).GameObject;
        return cardObj;
    }

    private void normalClick()
    {
        if (ListType == ListType.All)
        {
            DeckCardList.Instance.Add(CardId);
            AllCardList.Instance.Remove(CardId);
        }
        else
        {
            AllCardList.Instance.Add(CardId);
            DeckCardList.Instance.Remove(CardId);
        }
    }

    public void Add()
    {
        Number++;

    }

    public void Remove()
    {
        Number--;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.pointerId==-2) CardExplanation.I.Initialize(Instantiate(Card.transform.parent.gameObject));
    }

    //protected override void NormalClick()
    //{
    //    normalClick();
    //}

    //protected override void LongClick()
    //{
    //    CardExplanation.I.Initialize(Instantiate(Card.transform.parent.gameObject));
    //}
}

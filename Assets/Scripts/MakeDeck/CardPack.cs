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
        CardForDeckMaking card = book.MakeCardForDeckMaker();
        card.Initialize(book, listType);
        card.transform.parent.SetParent(cardTrn, false);
        CardId = card.CardId;
        Card = card;
        Number = 1;

        this.gameObject.AddComponent<Button>().onClick.AddListener(normalClick);
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

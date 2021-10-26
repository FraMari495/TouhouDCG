using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardForDeckMaking :MonoBehaviour// LongClickButton
{
    [SerializeField] private CardVisualController card;

    [SerializeField] private GameObject deckView;
    [SerializeField] private GameObject fieldView;
    [SerializeField] private GameObject discardView;

    private ListType CurrentType { get; set; }

    public int CardId { get; private set; }

    public void Initialize(CardBook book, ListType currentType)
    {
        CurrentType = currentType;
        deckView.SetActive(false);
        fieldView.SetActive(false);
        discardView.SetActive(false);
        CardId = book.Id;
        card.GetComponent<IPlayable>().Initialize(true, book);
        card.ChangeObject(PosEnum.Hand);
        Destroy(card);
    }

    //protected override void LongClick()
    //{
    //    CardExplanation.I.Initialize(Instantiate(this.transform.parent.gameObject));
    //}
}

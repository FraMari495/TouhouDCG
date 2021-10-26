using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DeckCardList : CardList
{
    [SerializeField] private DeckFinishButton finishButton;

    [SerializeField] private TextMeshProUGUI[] costCardTexts;
    [SerializeField] private TextMeshProUGUI deckCardsText;

    #region Singleton
    private static DeckCardList instance;

    public static DeckCardList Instance
    {
        get
        {
            DeckCardList[] instances = null;
            if (instance == null)
            {
                instances = FindObjectsOfType<DeckCardList>();
                if (instances.Length == 0)
                {
                    Debug.LogError("DeckCardListのインスタンスが存在しません");
                    return null;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError("DeckCardListのインスタンスが複数存在します");
                    return null;
                }
                else
                {
                    instance = instances[0];
                }
            }
            return instance;
        }
    }
    #endregion



    // Start is called before the first frame update
    void Start()
    {
        allCardBook = Resources.LoadAll<CardBook>("CardBook");


        List<UserData_Card> deck = SaveSystem.Instance.UserData.Deck;

        foreach (var cardData in deck)
        {
            for (int i = 0; i < cardData.Number; i++)
            {
                AllCardList.Instance.Remove(cardData.CardId);
                Add(cardData.CardId);
            }


        }
    }

    public override void Add(int cardId)
    {
        base.Add(cardId);
        finishButton.Show(Count == 30);
        UpdateDeckInfo();
    }

    public override void Remove(int cardId)
    {
        base.Remove(cardId);
        finishButton.Show(Count == 30);
        UpdateDeckInfo();
    }


    private void UpdateDeckInfo()
    {
        var d = list.GetNumberOfCost();
        var number = list.DeckCount;

        deckCardsText.text = $"{number}/30";

        int under7 = 0;

        for (int cost = 0; cost < 7; cost++)
        {
            if (d.ContainsKey(cost))
            {
                costCardTexts[cost].text = d[cost].ToString();
                under7 += d[cost];
            }
        }

        int over7 = d.Values.Sum() - under7;

        costCardTexts[7].text = over7.ToString();

        SaveSystem.Instance.ChangeDeckData(list);
    }





    public override ListType ListType => ListType.Deck;

    private void OnDestroy()
    {
        instance = null;
    }
}

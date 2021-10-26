using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ListType
{
    Deck,
    All
}

public abstract class CardList : MonoBehaviour
{
    public abstract ListType ListType { get; }

    [SerializeField] protected Transform layout;
    [SerializeField] private GameObject cardPackPrefab;
    protected CardBook[] allCardBook;

    //private List<CardPack> list = new List<CardPack>();
    protected SortedCardList list = new SortedCardList();


    public int Count => list.DeckCount;


    public virtual void Add(int cardId)
    {
        CardPack pack = Array.Find(layout.transform.GetComponentsInChildren<CardPack>(),c=>c.CardId == cardId);
        if (pack != null)
        {
            pack.Add();
            list.AddNumber(cardId);
        }
        else
        {
            CardPack newPack = Instantiate(cardPackPrefab, layout).GetComponent<CardPack>();
            CardBook book = Array.Find(allCardBook, b => b.Id == cardId);
            //var c = book.MakeCardForDeckMaker();
            //c.Initialize(book, ListType);
            newPack.Initialize(book,ListType);
            list.Add(book.Cost,cardId);
            newPack.transform.SetParent(layout,false);

            int index = list.GetIndex(cardId);
            newPack.transform.SetSiblingIndex(index);
        }
    }

    public virtual void Remove(int cardId)
    {
        var array = layout.GetComponentsInChildren<CardPack>();
        CardPack pack = Array.Find(array, c=>c.CardId == cardId);
        try
        {
            pack.Remove();
        }
        catch
        {
            Debug.Log(cardId);
        }
        list.RemoveNumber(cardId);
        if (pack.Number == 0)
        {
            list.Remove(cardId);
            Destroy(pack.gameObject);
        }
    }

}





public class SortedCardList:ICollection
{

    public SortedCardList()
    {
        cardIds = new List<int>();
        cardCosts = new List<int>();
        cardNum = new List<int>();
    }

    private List<int> cardIds;
    private List<int> cardCosts;
    private List<int> cardNum;

    /// <summary>
    /// Id Num の辞書
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, int> GetData()
    {
        Dictionary<int, int> id_Num_map = new Dictionary<int, int>();

        for (int i = 0; i < cardIds.Count; i++)
        {
            id_Num_map.Add(cardIds[i], cardNum[i]);
        }

        return id_Num_map;
    }

    public int Count => cardIds.Count == cardCosts.Count ? cardIds.Count : throw new Exception("cardIdsとcardCostsのカウントがあっていません");

    int ICollection.Count => Count;

    public int DeckCount => cardNum.Sum();

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public void Add(int cost,int id)
    {
        if (cardIds.Contains(id))
        {
            Debug.LogError($"id = {id}のカードは既にリストに存在します");
        }


        if (cardCosts.Contains(cost))
        {
            int initialIndex = 0;

            for (int i = 0; i < cardCosts.Count; i++)
            {
                if (cardCosts[i] == cost)
                {
                    initialIndex = i;
                }
            }

            for (int i = initialIndex; i < cardIds.Count; i++)
            {
                if (cardIds[i] > id || cardCosts[i] > cost)
                {
                    cardIds.Insert(i, id);
                    cardCosts.Insert(i, cost);
                    cardNum.Insert(i, 1);

                    return;
                }
            }

            cardIds.Insert(cardIds.Count, id);
            cardCosts.Insert(cardCosts.Count, cost);
            cardNum.Insert(cardNum.Count, 1);


        }
        else
        {


            for (int i = 0; i < cardIds.Count; i++)
            {
                if (cardCosts[i] > cost)
                {
                    cardIds.Insert(i, id);
                    cardCosts.Insert(i, cost);
                    cardNum.Insert(i, 1);
                    return;
                }
            }

            cardIds.Insert(cardIds.Count, id);
            cardCosts.Insert(cardCosts.Count, cost);
            cardNum.Insert(cardNum.Count, 1);
        }
    }

    public void AddNumber(int id)
    {
        cardNum[GetIndex(id)]++;
    }
    public void RemoveNumber(int id)
    {
        cardNum[GetIndex(id)]--;
    }
    public Dictionary<int,int> GetNumberOfCost()
    {
        Dictionary<int, int> d = new Dictionary<int, int>();

        for (int i = 0; i < Count; i++)
        {
            int cost = cardCosts[i];
            int number = cardNum[i];
            if (d.ContainsKey(cost))
            {
                d[cost] += number;
            }
            else
            {
                d.Add(cost, number);
            }
        }

        return d;
    }

    public void Remove(int id)
    {

        int index = GetIndex(id);
        if (cardNum[index] != 0)
        {
            Debug.LogError($"id = {id}はゼロ枚ではありません");
            return;
        }

        cardIds.RemoveAt(index);
        cardCosts.RemoveAt(index);
        cardNum.RemoveAt(index);

    }


    public int this[int n]
    {
        get => cardIds[n];
    }


    public int GetIndex(int id)
    {
        for (int i = 0; i < Count; i++)
        {
            if(cardIds[i] == id)
            {
                return i;
            }
        }

        Debug.LogError($"id = {id}はリストに存在しません");
        return -1;
    }

    /// <summary>
    /// ICollection.CopyTo
    /// </summary>
    /// <param name="list">コピー先のリスト</param>
    /// <param name="index">開始インデックス</param>
    void ICollection.CopyTo(Array list, int index)
    {
        foreach (var id in cardIds)
        {
            list.SetValue(id, index);
            index++;
        }
    }

    public IEnumerator GetEnumerator()
    {
        return cardIds.GetEnumerator();
    }
}

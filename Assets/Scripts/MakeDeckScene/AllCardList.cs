using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class AllCardList :CardList
{
    #region Singleton
    private static AllCardList instance;

    public static AllCardList Instance
    {
        get
        {
            AllCardList[] instances = null;
            if (instance == null)
            {
                instances = FindObjectsOfType<AllCardList>();
                if (instances.Length == 0)
                {
                    Debug.LogError("AllCardListのインスタンスが存在しません");
                    return null;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError("AllCardListのインスタンスが複数存在します");
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



    public override ListType ListType => ListType.All;



    // Start is called before the first frame update
    void Awake()
    {
        allCardBook = Resources.LoadAll<CardBook>("CardBook");

        List<UserData_Card> remained = SaveSystem.Instance.UserData.CardNumber;

        foreach (var cardData in remained)
        {
            CardBook item = Array.Find(allCardBook, b => b.Id == cardData.CardId);
   

            for (int i = 0; i < cardData.Number; i++)
            {
                Add(item.Id);
            }
      
        }
    }


    private void OnDestroy()
    {
        instance = null;
    }
}




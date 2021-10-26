using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;

public class CardInfo : MonoSingleton<CardInfo>
{
    [SerializeField] private NotificationManager notification;
    [SerializeField] private Transform cardObjTrn;

    private bool showing = false;

    public void Show(CardBook cardBook, GameObject cardObj)
    {
        //StartCoroutine(Waiting(cardBook, cardObj));
    }

    private IEnumerator Waiting(CardBook cardBook, GameObject cardObj)
    {
        yield return new WaitWhile(() => showing);
        showing = true;
        ShowDetail(cardBook, cardObj);
        yield return new WaitForSeconds(2);
        showing = false;
    }

    private void ShowDetail(CardBook cardBook, GameObject cardObj)
    {
        if(cardObjTrn.childCount>0) Destroy(cardObjTrn.GetChild(0).gameObject);
        cardObj.transform.SetParent(cardObjTrn,false);
        notification.descriptionObj.text = cardBook.Description;
        notification.titleObj.text = cardBook.CardName;
     //   notification.UpdateUI();
        notification.OpenNotification();
    }


}

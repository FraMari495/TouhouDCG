//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class CardViewManager : MonoSingleton<CardViewManager>
//{
//    public void ToHand(IPlayable card)
//    {
//        //見た目を手札のViewに変更する
//        card.ChangePos(PosEnum.Hand);

//        //カードオブジェクトの移動(親はHandにする)
//        card.transform.SetParent(Hand.I(card.IsPlayer).transform, false);
//    }

//    public void ToField(IPlayable card, int pos)
//    {

//        //見た目をフィールドのViewに変更する
//        card.ChangePos(PosEnum.Field);

//        //カードオブジェクトの移動(親はHandにする)
//        card.transform.SetParent(this.transform, false);
//        card.transform.SetSiblingIndex(pos);
//    }
//}


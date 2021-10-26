using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    /// <summary>
    /// 手札からフィールドにカードがD&Dされたときの処理 (カードをプレイ使用としたときの、フィールド側の処理)
    ///     ドロップ位置を CardState_Hand_Chara に返す
    /// </summary>
    public class FieldDropTarget : MonoBehaviour, IDropHandler
    {
        [SerializeField] private bool isPlayer;

        public void OnDrop(PointerEventData eventData)
        {
            //プレイ位置が決まる3つの条件

            //1. ポインターがオブジェクトを持っている
            GameObject obj = eventData.pointerDrag;
            if (obj == null) return;

            //2. そのカードのステートが CardState_Hand_Chara である
            if (obj.GetComponent<CardInputHandler>().CurrentState is CardState_Hand hand)
            {

                //3. HandViewが対戦相手のものではない
                if (hand.IsPlayer != isPlayer) return;

                //プレイ位置はポインターの位置を用いて決定する

                //最初は答えを0としておく
                int answer = 0;

                //フィールド上のカードを順番に確認する
                foreach (Transform trn in this.transform)
                {
                    //ポインターより右にあるか
                    if (trn.position.x > eventData.position.x)
                    {
                        //Yes
                        break;
                    }
                    else
                    {
                        //No
                        answer++;
                    }
                }

                //CardState_Hand_Chara にプレイ位置を伝える
                hand.PlayPos = answer;
            }
        }
    }
}

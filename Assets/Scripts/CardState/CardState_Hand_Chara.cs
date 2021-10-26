using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;

public class CardState_Hand_Chara : CardState_Hand
{

    public CardState_Hand_Chara(IPlayable playable,bool special = false) : base(playable,special)
    {
        if(playable is Status_Chara chara)
        {
        }
        else
        {
            Debug.LogError("キャラカード以外がCardState_Hand_Charaステートに遷移できません");
        }
    }


}

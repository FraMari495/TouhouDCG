using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;

public class CardState_Hand_Spell : CardState_Hand
{
    public CardState_Hand_Spell(IPlayable playable,bool special = false) : base(playable,special)
    {
        if (playable is Status_Spell spell)
        {
        }
        else
        {
            Debug.LogError("スペルカード以外がCardState_Hand_Spellステートに遷移できません");
        }
    }


}

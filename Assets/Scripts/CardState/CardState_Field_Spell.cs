using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardState_Field_Spell : CardState_Field
{
    public CardState_Field_Spell(IPlayable playable, int index, bool special) : base(playable, index, special)
    {
    }

    public override bool Enter()
    {

        //アニメーションを作成しEnqueue
        AnimationManager.I.AddSequence(() => AnimationMaker.SpellAnimation(Playable, GameObject.Find("Canvas").transform), "カードをプレイ");

        Debug.LogWarning("7枚まで");
        return true;
    }
}

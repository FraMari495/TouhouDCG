using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardState_Field_Chara : CardState_Field
{
    public CardState_Field_Chara(IPlayable playable, int index, bool special) : base(playable, index, special)
    {
    }

    public override bool Enter()
    {
        if (Special)
        {
            Playable.GameObject.GetComponent<CanvasGroup>().alpha = 0;
            //アニメーションを作成しEnqueue
            AnimationManager.I.AddSequence<AnimationManager.SpecialSummon>(() => AnimationManager.I.AnimationMaker.SpecialSummonAnimation(Playable, Index)
            , "カードをプレイ");
        }
        else
        {
            //アニメーションを作成しEnqueue
            AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.PlayAnimation(Playable, Index, GameObject.Find("Canvas").transform)
            , "カードをプレイ");
        }


        Debug.LogWarning("7枚まで");
        return true;
    }
}

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
            //�A�j���[�V�������쐬��Enqueue
            AnimationManager.I.AddSequence<AnimationManager.SpecialSummon>(() => AnimationManager.I.AnimationMaker.SpecialSummonAnimation(Playable, Index)
            , "�J�[�h���v���C");
        }
        else
        {
            //�A�j���[�V�������쐬��Enqueue
            AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.PlayAnimation(Playable, Index, GameObject.Find("Canvas").transform)
            , "�J�[�h���v���C");
        }


        Debug.LogWarning("7���܂�");
        return true;
    }
}

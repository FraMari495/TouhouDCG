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
            Debug.LogError("�L�����J�[�h�ȊO��CardState_Hand_Chara�X�e�[�g�ɑJ�ڂł��܂���");
        }
    }


}

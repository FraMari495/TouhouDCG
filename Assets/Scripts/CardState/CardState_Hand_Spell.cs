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
            Debug.LogError("�X�y���J�[�h�ȊO��CardState_Hand_Spell�X�e�[�g�ɑJ�ڂł��܂���");
        }
    }


}

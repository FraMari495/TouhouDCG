//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class CardViewManager : MonoSingleton<CardViewManager>
//{
//    public void ToHand(IPlayable card)
//    {
//        //�����ڂ���D��View�ɕύX����
//        card.ChangePos(PosEnum.Hand);

//        //�J�[�h�I�u�W�F�N�g�̈ړ�(�e��Hand�ɂ���)
//        card.transform.SetParent(Hand.I(card.IsPlayer).transform, false);
//    }

//    public void ToField(IPlayable card, int pos)
//    {

//        //�����ڂ��t�B�[���h��View�ɕύX����
//        card.ChangePos(PosEnum.Field);

//        //�J�[�h�I�u�W�F�N�g�̈ړ�(�e��Hand�ɂ���)
//        card.transform.SetParent(this.transform, false);
//        card.transform.SetSiblingIndex(pos);
//    }
//}


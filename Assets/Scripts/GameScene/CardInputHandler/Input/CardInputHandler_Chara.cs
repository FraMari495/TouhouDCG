using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class CardInputHandler_Chara : CardInputHandler
    {
        public override bool ToField(int pos)
        {
            CurrentState = new CardState_Field_Chara(playable, pos,false);
            return CurrentState.Enter();
        }

        public override bool ToHand()
        {
            CurrentState = new CardState_Hand_Chara(playable);
            return CurrentState.Enter();
        }

        public bool ToFieldSpecial(int pos)
        {
            CurrentState = new CardState_Field_Chara(playable, pos,true);
            return CurrentState.Enter();
        }



    }


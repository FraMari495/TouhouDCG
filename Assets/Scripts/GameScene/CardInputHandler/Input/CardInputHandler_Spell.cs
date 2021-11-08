using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    namespace CardHandler
    {
        public class CardInputHandler_Spell : CardInputHandler
        {
            public override bool ToField(int pos)
            {
                CurrentState = new CardState_Field_Spell(playable, pos, false);
                return CurrentState.Enter();
            }

            public override bool ToHand()
            {
                CurrentState = new CardState_Hand_Spell(playable);
                return CurrentState.Enter();
            }


        }
    }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
//using Position;
using System;
//using Command;

namespace Player
{
    public class Target_Chara : Target, ICardViewInitializer
    {
        public override bool IsPlayer { get; protected set; }

        private IPlayable statusChara;

        protected override void Start()
        {
            base.Start();

            Status = GetComponentInParent<StatusBase>();
        }

        public void Initialize(bool isPlayer,CardBook book, bool deckMaking = false)
        {
            IsPlayer = isPlayer;
            statusChara = this.GetComponentInParent<IPlayable>();
        }

        protected override void SelectedAsAttackTarget(IPlayable attacker)
        {
            //FieldView_CharaÇ…çUåÇëŒè€Çì`Ç¶ÇÈ
            if (attacker.GameObject.GetComponent<CardInputHandler>().CurrentState is CardState_Field_Chara attackerState) {

                attackerState.Target = this.GetComponentInParent<IPlayable>().PlayableCardId;
            }
        }
    }
}

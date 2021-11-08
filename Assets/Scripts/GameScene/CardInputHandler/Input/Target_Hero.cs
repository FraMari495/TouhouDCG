using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;

namespace Player
{
    /// <summary>
    /// 攻撃対象、アビリティー対象としてヒーローを選択できるようにする機能
    /// </summary>
    public class Target_Hero : Target
    {
        [SerializeField] private bool isPlayer;

        /// <summary>
        /// プレイヤーのヒーローか否か
        /// </summary>
        public override bool IsPlayer { get => isPlayer; protected set => throw new System.Exception("ヒーローのIsPlayerは変更できません"); }

        protected override void Start()
        {
            base.Start();
            Status = this.GetComponentInParent<StatusBase>();
        }

        /// <summary>
        /// 攻撃対象に選ばれた時に呼ばれる
        /// </summary>
        /// <param name="attacker"></param>
        protected override void SelectedAsAttackTarget(IPlayable attacker)
        {
            //FieldView_Charaに攻撃対象を伝える
            if (attacker.GameObject.GetComponent<CardInputHandler>().CurrentState is CardState_Field_Chara attackerState)
            {

                attackerState.AttackHero = true;
            }
        }

    }
}

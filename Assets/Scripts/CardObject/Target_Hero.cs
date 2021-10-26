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
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private bool isPlayer;

        /// <summary>
        /// プレイヤーのヒーローか否か
        /// </summary>
        public override bool IsPlayer { get => isPlayer; protected set => throw new System.Exception("ヒーローのIsPlayerは変更できません"); }

        /// <summary>
        /// ヒーローのステータス(状態異常やHpなど)
        /// </summary>
        public Status_Hero Status { get; private set; }

        /// <summary>
        /// 攻撃対象にできる条件(Hpが残っており、かつ場面に守護が存在しない)
        /// </summary>
        protected override bool Condition => !Position.Field.I(IsPlayer).ExistGardian() && Status.Hp>0;

        private void Start()
        {
            Debug.Log(this.GetType());

            Status = this.GetComponent<Status_Hero>();
            Status.UpdateHpUI.Subscribe(hp=>hpText.text = hp.ToString());
            Status.Initialize(IsPlayer);

            Debug.Log(this.GetType()+"end");

        }

        /// <summary>
        /// 攻撃対象に選ばれた時に呼ばれる
        /// </summary>
        /// <param name="field"></param>
        protected override void SelectedAsAttackTarget(CardState_Field field)
        {
            //FieldView_Charaにヒーローを攻撃することを伝える
            field.AttackHero = true;
        }

    }
}

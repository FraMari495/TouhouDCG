using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;

namespace Player
{
    /// <summary>
    /// �U���ΏہA�A�r���e�B�[�ΏۂƂ��ăq�[���[��I���ł���悤�ɂ���@�\
    /// </summary>
    public class Target_Hero : Target
    {
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private bool isPlayer;

        /// <summary>
        /// �v���C���[�̃q�[���[���ۂ�
        /// </summary>
        public override bool IsPlayer { get => isPlayer; protected set => throw new System.Exception("�q�[���[��IsPlayer�͕ύX�ł��܂���"); }

        /// <summary>
        /// �q�[���[�̃X�e�[�^�X(��Ԉُ��Hp�Ȃ�)
        /// </summary>
        public Status_Hero Status { get; private set; }

        /// <summary>
        /// �U���Ώۂɂł������(Hp���c���Ă���A����ʂɎ�삪���݂��Ȃ�)
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
        /// �U���ΏۂɑI�΂ꂽ���ɌĂ΂��
        /// </summary>
        /// <param name="field"></param>
        protected override void SelectedAsAttackTarget(CardState_Field field)
        {
            //FieldView_Chara�Ƀq�[���[���U�����邱�Ƃ�`����
            field.AttackHero = true;
        }

    }
}

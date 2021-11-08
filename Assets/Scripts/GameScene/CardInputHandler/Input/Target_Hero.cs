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
        [SerializeField] private bool isPlayer;

        /// <summary>
        /// �v���C���[�̃q�[���[���ۂ�
        /// </summary>
        public override bool IsPlayer { get => isPlayer; protected set => throw new System.Exception("�q�[���[��IsPlayer�͕ύX�ł��܂���"); }

        protected override void Start()
        {
            base.Start();
            Status = this.GetComponentInParent<StatusBase>();
        }

        /// <summary>
        /// �U���ΏۂɑI�΂ꂽ���ɌĂ΂��
        /// </summary>
        /// <param name="attacker"></param>
        protected override void SelectedAsAttackTarget(IPlayable attacker)
        {
            //FieldView_Chara�ɍU���Ώۂ�`����
            if (attacker.GameObject.GetComponent<CardInputHandler>().CurrentState is CardState_Field_Chara attackerState)
            {

                attackerState.AttackHero = true;
            }
        }

    }
}

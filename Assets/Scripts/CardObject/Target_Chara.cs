using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Target_Chara : Target, ICardViewInitializer
    {
        public override bool IsPlayer { get; protected set; }

        private Status_Chara statusChara;

        /// <summary>
        /// �U���ΏۂƂȂ肤�邩
        /// </summary>
        protected override bool Condition
        {
            get
            {
                //���̃J�[�h(target)��HP��0�ȏ�
                bool condition1 = statusChara.Hp > 0;

                //���̃J�[�h�͎��ł���
                bool condition2 = statusChara.CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Gardian);

                //�t�B�[���h�Ɏ�삪���݂��Ȃ�
                bool condition3 = !Position.Field.I(statusChara.IsPlayer).ExistGardian();

                return condition1 && (condition2||condition3);
            }
        }

        public void Initialize(bool isPlayer,CardBook book)
        {
            IsPlayer = isPlayer;
            statusChara = this.GetComponentInParent<Status_Chara>();
        }

        protected override void SelectedAsAttackTarget(CardState_Field field)
        {
            //FieldView_Chara�ɍU���Ώۂ�`����
            field.Target = this.GetComponentInParent<IPlayable>().PlayableId;
        }
    }
}

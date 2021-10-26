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
        /// 攻撃対象となりうるか
        /// </summary>
        protected override bool Condition
        {
            get
            {
                //このカード(target)のHPが0以上
                bool condition1 = statusChara.Hp > 0;

                //このカードは守護である
                bool condition2 = statusChara.CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Gardian);

                //フィールドに守護が存在しない
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
            //FieldView_Charaに攻撃対象を伝える
            field.Target = this.GetComponentInParent<IPlayable>().PlayableId;
        }
    }
}

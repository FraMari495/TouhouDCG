
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

namespace Position
{
    public class Field : PositionBase<Field>
    {
        protected override PosEnum Pos => PosEnum.Field;

        /// <summary>
        /// フィールドのカードのうち、CardType型のカードを返す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<StatusBase> GetStatus(CardType[] type)
        {
            //type型のカードを抜き出す
            List<StatusBase> list = Cards.ConvertType<StatusBase>().Where(x => type.Contains(x.Type)).ToList();

            //ヒーローも選択可能ならばlistに追加する
            if (type.Contains(CardType.Hero)) list.Add(Status_Hero.I(IsPlayer));

            return list;
        }


        /// <summary>
        /// 引数のカードが、Fieldの何番目に配置されているか
        /// </summary>
        /// <param name="statusBase"></param>
        /// <returns></returns>
        public int GetIndex(StatusBase statusBase)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                if (Cards[i] as StatusBase == statusBase)
                {
                    return i;
                }
            }
            Debug.LogError($"{statusBase}はFieldに見つかりませんでした");
            return -1;
        }

        /// <summary>
        /// カードがカードを攻撃
        /// </summary>
        /// <param name="attackerId">アタッカーの位置</param>
        /// <param name="targetId">ターゲットの位置</param>
        public bool Attack(int attackerId, int targetId)
        {
            //PlayableIdからカードを特定
            var attacker = (Status_Chara)PlayableIdManager.I.GetPlayableById(attackerId);
            var target = (Status_Chara)PlayableIdManager.I.GetPlayableById(targetId);

            var allFieldCards = new List<IPlayable>();
            allFieldCards.AddRange(Field.I(true).Cards);
            allFieldCards.AddRange(Field.I(false).Cards);

            //デバッグ(アタッカーはIsPlayableでなければならない。両者がフィールドに存在するか確認)
            if (!allFieldCards.Contains(attacker) || !allFieldCards.Contains(target))
            {
                return false;
            }

            //カードからData_Charaクラス(カードの状態を表現するクラス)を抽出
            Status_Chara.Data_Chara attackerData = attacker.CharaData;
            Status_Chara.Data_Chara targetData = target.CharaData;

           　
            //攻撃アニメーション(不意打ちの場合はアニメーションが異なる)
            bool isSurpriseAttack = attackerData.MyAbilities.HaveSpecialStatus(SpecialStatus.Surprise);
            if (!isSurpriseAttack) AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.AttackAnimation(attacker), "攻撃アニメーション");
            else AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.LockOnAnimation(attacker, target), "攻撃アニメーション");

            ChangeStatus(attackerData, targetData, Status_Hero.I(!attackerData.IsPlayer));

            return true;
        }

        private void ChangeStatus(Status_Chara.Data_Chara attackerData, Status_Chara.Data_Chara targetData,Status_Hero rival_Hero)
        {

            //アタッカーの残りの攻撃回数を減らす
            attackerData.AttackNum--;

            //攻撃し、与えたダメージを取得
            int damageT = targetData.DamageHp(attackerData.AtkData, attackerData.MyAbilities.HaveSpecialStatus(SpecialStatus.Killer));
            if (attackerData.MyAbilities.HaveSpecialStatus(SpecialStatus.Killer) && damageT > 0)
            {
                targetData.AddStatusEffect(StatusEffect.Dead);
                //Debug.LogError("状態異常のアニメーション");
            }

            //不意打ちではないなら反撃
            bool isSurpriseAttack = attackerData.MyAbilities.HaveSpecialStatus(SpecialStatus.Surprise);
            if (!isSurpriseAttack)
            {
                int damageA = attackerData.DamageHp(targetData.AtkData, targetData.MyAbilities.HaveSpecialStatus(SpecialStatus.Killer));
                if (targetData.MyAbilities.HaveSpecialStatus(SpecialStatus.Killer) && damageA > 0)
                {
                    attackerData.AddStatusEffect(StatusEffect.Dead);
                   // Debug.LogError("状態異常のアニメーション");
                }
            }

            //凍結
            if (attackerData.MyAbilities.HaveSpecialStatus(SpecialStatus.Freeze))
            {
                targetData.AddStatusEffect(StatusEffect.Freeze);
                //Debug.LogError("状態異常のアニメーション");
            }

            //貫通
            if (attackerData.MyAbilities.HaveSpecialStatus(SpecialStatus.Penetrate))
            {
                //Aiはrival_Heroとしてnullを代入する。今後修正 ( = 現状、Aiは貫通攻撃を考えない)
                if (rival_Hero!=null) rival_Hero.CardData.DamageHp(damageT / 2);
            }
        }

        /// <summary>
        /// AIの行動決定のため、攻撃のシミュレーションを行う
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="target"></param>
        public void AttackForAISimulation(Status_Chara.Data_Chara attacker, Status_Chara.Data_Chara target)
        {
            //現在AIは貫通攻撃を考えないため、第3引数はnull
            ChangeStatus(attacker, target,null);
        }

        /// <summary>
        /// ヒーローを攻撃する
        /// </summary>
        /// <param name="attackerPos"></param>
        /// <returns></returns>
        public bool AttackHero(int attackerPos)
        {

            if ((Status_Chara)PlayableIdManager.I.GetPlayableById(attackerPos) is Status_Chara attacker)
            {
                //デバッグ(アタッカーはIsPlayableでなければならない。両者がフィールドに存在するか確認)
                if (!attacker.IsPlayable || !Field.I(attacker.IsPlayer).Cards.Contains(attacker))
                {
                    return false;
                }

                attacker.CharaData.AttackNum--;
                Status_Hero hero = Status_Hero.I(!IsPlayer);

                //攻撃のアニメーション(不意打ちとその他でアニメーションが異なる)
                bool isSurpriseAttack = attacker.CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Surprise);
                if (!isSurpriseAttack) AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.AttackAnimation(attacker), "攻撃アニメーション");
                else AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.LockOnAnimation(attacker, hero), "攻撃アニメーション");

                //ダメージを与える
                hero.CardData.DamageHp(attacker.Atk);
                return true;
            }
            else
            {
                Debug.LogError($"{(Status_Chara)PlayableIdManager.I.GetPlayableById(attackerPos)}はキャラクターカードではありません");
                return false;
            }
        }

        /// <summary>
        /// 死亡判定 & 行動可能か判定
        /// アタッカーの選択肢
        /// </summary>
        protected override void Judge()
        {
            //死亡判定
            foreach (var card in new List<IPlayable>(Cards))
            {

                //キャラクターのステータスを参照
                Status_Chara chara = card.GameObject.GetComponent<Status_Chara>();

                //死亡判定
                if (chara != null && chara.CharaData.StatusEffects.Contains(StatusEffect.Dead))
                {
                    //死亡したらJudgeされない
                    //しかし盤面には(アニメーション中に)残るため、死ぬ直前にPlayable = false;をする
                    chara.IsPlayable = false;
                    DeadCard(chara);
                }


            }

            //行動可能か判定
            foreach (var card in Cards)
            {
                if (card is Status_Chara chara)
                {
                    //攻撃可能条件
                    bool condition1 = TurnManager.I.Turn == IsPlayer; //自分のターン
                    bool condition2 = chara.CharaData.AttackNum > 0; //攻撃可能回数が残っているか
                    chara.IsPlayable = condition1 && condition2;
                }
            }

            // Cards.ForEach(c => c.GameObject.GetComponent<CardInputHandler>().UpdatePlayableAura());

            ReactivePropertyList.I.UpdatePlayableAura(IsPlayer);

        }

        /// <summary>
        /// カードが死亡したときの処理
        /// </summary>
        /// <param name="status"></param>
        private void DeadCard(Status_Chara status)
        {
            status.RunOnDefeatedSkill();
            Move(status, Discard.I(status.IsPlayer), Discard.I(status.IsPlayer).Cards.Count);
        }

        public bool SpecialSummon(int posIndex,IPlayable playable)
        {
            //データ上でフィールドに追加
            if (AddCard(posIndex, playable))
            {
                //ポジション変更を通知(アニメーション)
                playable.UpdatePosition.OnNext(( PosEnum.None, Pos, posIndex));
                return true;
            }

            return false;
        }

        /// <summary>
        /// ターン開始時に呼ばれる
        /// </summary>
        protected override void OnBeginTurn()
        {
            //Status_Chara型のカードすべてを探し、ResetAttackNumを実行
            Cards.ToList().ConvertAll(c => c.GameObject.GetComponent<Status_Chara>()).NonNull().ForEach(x => x.ResetAttackNum());
        }

        public void OnTurnEnd()
            => Cards.ConvertType<Status_Chara>().NonNull().ForEach(chara => chara.RunOnTurnEndSkill());
        

        public bool ExistGardian()
            => Cards.ConvertType<Status_Chara>().NonNull().Any(c => c.CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Gardian));



    }
}

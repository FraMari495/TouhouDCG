using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class Status : StatusBase
{
    #region UniRx.Subject
    /// <summary>
    /// Hpの表示を変更するように通知を送る
    /// </summary>
    public Subject<int> UpdateHpUI { get; } = new Subject<int>();
    public Subject<StatusEffect> UpdateEffect { get; } = new Subject<StatusEffect>();
    public Subject<StatusEffect> UpdateRemoveEffect { get; } = new Subject<StatusEffect>();
    #endregion


    #region public methods
    /// <summary>
    /// 状態異常を取り除く
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool RemoveStatusEfect(StatusEffect effect)
    {
        if (!CardData.StatusEffects.Remove(effect)) return false;
        UpdateRemoveEffect.OnNext(effect);
        return true;
    }

    /// <summary>
    /// Hpの値
    /// </summary>
    public int Hp => HpData;

    /// <summary>
    /// Hpの減少
    /// </summary>
    /// <param name="delta"></param>
    public void DownHp(int delta)
    {
        HpData.Down(delta);
        Debug.LogError("Hp減少アニメーション");
    }

    /// <summary>
    /// Hpを追加
    /// </summary>
    /// <param name="delta"></param>
    public void AddHp(int delta)
    {
        HpData.Add(delta);
        AnimationManager.I.AddSequence(() => DOTween.Sequence().AppendCallback(() => UpdateHpUI.OnNext(CardData.HpData)), "攻撃力増加");
    }

    #endregion

    public Data CardData { get; protected set; }
    protected Hp HpData => CardData.HpData;

    protected abstract void Dead();

    /// <summary>
    /// カードの状態を表すクラス
    /// </summary>
    public abstract class Data
    {
        private Transform Trn { get; } = null;

        public Data(bool isPlayer,Transform trn,int currentHp,int maxHp,Action onDead)
        {
            Trn = trn;
            StatusEffects = new HashSet<StatusEffect>();
            HpData = new Hp( currentHp, maxHp, onDead);
            IsPlayer = isPlayer;
        }

        /// <summary>
        /// プレイヤーのカードか否か
        /// </summary>
        public bool IsPlayer { get; }

        /// <summary>
        /// 状態異常
        /// </summary>
        public HashSet<StatusEffect> StatusEffects { get; set; }

        /// <summary>
        /// Hp
        /// </summary>
        public Hp HpData { get; set; }


        /// <summary>
        /// Hpを減少させ、実際に減少した値を返す
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public int DamageHp(int damage, bool killer = false)
        {
            int before = HpData;

            if (this is Status_Chara.Data_Chara chara)
            {
                damage -= chara.MyAbilities.Diffense;
                damage = damage < 0 ? 0 : damage;
            }
            int hpTemp = HpData - damage;

            if (Trn != null)
            {
                AnimationManager.I.AddSequence<AnimationManager.Damage>(() => AnimationMaker.DamageAnimation(damage, hpTemp, Trn, killer), "ダメージアニメーション");
            }
            HpData.Damage(damage);

            return before - HpData;
        }

        /// <summary>
        /// 状態異常を追加する
        /// </summary>
        /// <param name="effect"></param>
        public void AddStatusEffect(StatusEffect effect)
        {
            StatusEffects.Add(effect);
        }
    }

}




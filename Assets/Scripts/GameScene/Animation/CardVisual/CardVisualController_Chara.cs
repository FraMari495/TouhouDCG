using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace Animation
{
    /// <summary>
    /// キャラカードの見た目を司るクラス
    /// </summary>
    internal class CardVisualController_Chara : CardVisualController
    {
        #region Serializable Field
        [SerializeField] private TextMeshProUGUI[] atkTexts, hpTexts;
        [SerializeField] private Image hpImage_field, hpImage_hand, atkImage_field, atkImage_hand;
        [SerializeField] private Sprite gardianHpSprite, killerAtkSprite, normalHpSprite, normalAtkSprite, penetrateSprite, penetrateKiller, armedHp, armedGardian, surprise;
        [SerializeField] private TextMeshProUGUI atkText_hand, atkText_field,bombCostText;
        [SerializeField] private GameObject freezeEffect,bombCostObj;
        [SerializeField] private Race_TextPair[] race_TextPairs;
        #endregion


        protected override void Start()
        {
            if (Playable is Status_Chara chara)
            {
                base.Start();

                //ゲーム中に変化するステータスを追う
                Subscribing(chara);

                bombCostObj.SetActive(chara.CharaData.MyAbilities.BombAbility!=null);

                //初期の見た目を作る
                InitialVisual(chara.CharaData);
            }
            else
            {
                Debug.LogError("これはキャラクターカードではありません");
                Debug.LogError("ディスプレイ用の処理");
            }

        }


        /// <summary>
        /// 初期の見た目を作る
        /// </summary>
        /// <param name="CharaData"></param>
        private void InitialVisual(Status_Chara.Data_Chara CharaData)
        {

            if (CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Gardian))
            {
                hpImage_hand.sprite = gardianHpSprite;
                hpImage_field.sprite = gardianHpSprite;
            }

            if (CharaData.MyAbilities.Diffense > 0)
            {
                hpImage_hand.sprite = armedHp;
                hpImage_field.sprite = armedHp;
            }

            if (CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Surprise))
            {
                atkImage_hand.sprite = surprise;
                atkImage_field.sprite = surprise;
            }

            if (CharaData.MyAbilities.Diffense > 0 && CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Gardian))
            {
                hpImage_hand.sprite = armedGardian;
                hpImage_field.sprite = armedGardian;
            }

            if (CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Killer))
            {
                atkImage_hand.sprite = killerAtkSprite;
                atkImage_field.sprite = killerAtkSprite;
                atkText_hand.color = Color.red;
                atkText_field.color = Color.red;

            }

            if (CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Penetrate))
            {
                atkImage_field.sprite = penetrateSprite;
                atkImage_hand.sprite = penetrateSprite;
            }

            if (CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Penetrate) && CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Killer))
            {
                atkImage_field.sprite = penetrateKiller;
                atkImage_hand.sprite = penetrateKiller;
            }

        }

        /// <summary>
        /// 変化するステータスを監視する
        /// </summary>
        /// <param name="chara"></param>
        private void Subscribing(Status_Chara chara)
        {
            chara.UpdateAtkUI.Subscribe(atk => atkTexts.ForEach(atkText => atkText.text = atk.ToString()));
            chara.UpdateHpUI.Subscribe(hp => hpTexts.ForEach(hpText => hpText.text = hp.ToString()));



            chara.UpdateAtkUI.OnNext(chara.Atk);
            chara.UpdateHpUI.OnNext(chara.Hp);

            chara.UpdateEffect.Subscribe(effect => AddStatusEffectAnimation(effect));
            chara.UpdateRemoveEffect.Subscribe(effect => RemoveStatusEfect(effect));

            chara.UpdateBombUI.Subscribe(bomb => { 
                bombCostText.text = bomb.ToString();
                bombCostObj.SetActive((Playable as Status_Chara).CharaData.MyAbilities.BombCost>0);
            });
            chara.UpdateBombUI.OnNext(chara.CharaData.MyAbilities.BombCost);
        }

        /// <summary>
        /// 状態異常が追加された際のアニメーション
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="changeData"></param>
        private void AddStatusEffectAnimation(StatusEffect effect, bool changeData = true)
        {

            switch (effect)
            {
                case StatusEffect.Dead:
                    break;

                case StatusEffect.Freeze:
                    AnimationManager.I.AddSequence(() => DOTween.Sequence()
                    .AppendCallback(() => freezeEffect.SetActive(true))
                        .AppendInterval(0.5f), "フリーズ");
                    break;

                case StatusEffect.Sealed:
                    atkImage_field.sprite = normalAtkSprite;
                    atkImage_hand.sprite = normalAtkSprite;
                    hpImage_field.sprite = normalHpSprite;
                    atkText_field.color = Color.black;
                    atkText_hand.color = Color.black;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 状態異常が解除されたときのアニメーション
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        public bool RemoveStatusEfect(StatusEffect effect)
        {
            switch (effect)
            {
                case StatusEffect.Dead:
                    break;
                case StatusEffect.Freeze:
                    freezeEffect.SetActive(false);
                    break;
                default:
                    break;
            }
            return true;
        }


        public override void Initialize(bool isPlayer, CardBook cardBook, bool deckMaking = false)
        {

            base.Initialize(isPlayer, cardBook, deckMaking);
            CardBook_Chara charaBook = cardBook as CardBook_Chara;
            race_TextPairs.ForEach(t => t.SetActive(t.Race == charaBook.Race));
            hpTexts.ForEach(hpText => hpText.text = charaBook.Hp.ToString());
            atkTexts.ForEach(atkText => atkText.text = charaBook.Atk.ToString());
        }
    }

    [System.Serializable]
    public class Race_TextPair
    {
        [SerializeField] private Race race;
        [SerializeField] private GameObject raceText_hand;
        [SerializeField] private GameObject raceText_field;


        public Race Race { get => race;  }

        public void SetActive(bool show)
        {
            raceText_hand.SetActive(show);
            raceText_field.SetActive(show);
        }

    }
}

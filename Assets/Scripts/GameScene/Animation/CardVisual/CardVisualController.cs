using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Command;

namespace Animation
{
    /// <summary>
    /// カードの見た目を作成する機能
    /// </summary>
    public abstract class CardVisualController : MonoBehaviour,ICardViewInitializer
    {
        #region Serializable Fields
        [SerializeField] protected TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI cardNameText, description;
        [SerializeField] private Image cardImage_hand, cardImage_field;
        [SerializeField] private GameObject playableAura;
        [SerializeField] private GameObject attackableAura;
        #endregion


        public PosEnum CurrentPos { get; private set; }

        /// <summary>
        /// カードが存在する位置(手札、山札、デッキ、フィールド)と、
        /// その位置に移動した際にアクティブにするゲームオブジェクトを紐づける
        /// </summary>
        private Dictionary<PosEnum, GameObject> viewMap = new Dictionary<PosEnum, GameObject>();

        #region public methods

        /// <summary>
        /// カードの表示を、引数のポジションの見た目に変える
        /// </summary>
        /// <param name="pos">ポジション</param>
        public void ChangeObject(PosEnum pos)
        {
            CurrentPos = pos;
            viewMap.ForEach(pos_obj => pos_obj.Value.SetActive(pos_obj.Key == pos));
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="isPlayer">プレイヤーのカードか否か</param>
        /// <param name="cardBook">カードのデータベース</param>
        public  virtual void Initialize(bool isPlayer, CardBook cardBook, bool deckMaking = false)
        {
            Playable = this.GetComponent<IPlayable>();

            //カードの空白に文字やSpriteを設定
            cardNameText.text = cardBook.CardName;
            description.text = cardBook.Description;
            cardImage_hand.sprite = cardBook.CardImage;
            cardImage_field.sprite = cardBook.CardImage;

            //カードの絵柄の大きさを調節
            cardImage_hand.rectTransform.sizeDelta *= cardBook.ImageSizeRatio;
            cardImage_field.rectTransform.sizeDelta *= cardBook.ImageSizeRatio;
            costText.text = cardBook.Cost.ToString();


            if (!isPlayer && !ForDebugging.I.DebugMode)
            {
                viewMap[PosEnum.Hand].SetActive(false);
                viewMap.Remove(PosEnum.Hand);
            }

            if (!deckMaking)
            {
                this.GetComponent<CardInputHandler>().O_ShowExampleCard.Subscribe(_ => GetExampleCard());

                //プレイ可能か否かを示すオーラの表示(非表示)
                ReactivePropertyList.I.O_UpdatePlayableAura.Subscribe(_ => UpdatePlayableAura());
            }
        }

        private void GetExampleCard()
        {
            CardVisualController exampleCard = Instantiate(this.gameObject).GetComponent<CardVisualController>();
            exampleCard.ChangeObject(PosEnum.Hand);
            exampleCard.transform.Find("_HandView").gameObject.SetActive(true);
            exampleCard.transform.Find("_DeckView").gameObject.SetActive(false);
            exampleCard.ChangeUsable(false);
            exampleCard.GetComponent<CardInputHandler>().enabled = false;

            CardExplanation.I.Initialize(exampleCard.gameObject);
        }

        #endregion

        #region private (protected) method

        /// <summary>
        /// プレイ可能か否かを変更する(オーラの表示非表示を切り替える)
        /// </summary>
        /// <param name="usable"></param>
        private void ChangeUsable(bool usable)
        {
            if(Playable is Status_Spell)
            {
                bool isPl = Playable.IsPlayer;
                Debug.Log("o");
            }

            playableAura.SetActive(usable);
            attackableAura.SetActive(usable);
        }


        protected virtual void Start()
        {
            if (Playable != null)
            {
                //コストが変更された際に発行される通知を受け取る(コストに表示を変更する)
                Playable.UpdateCostUI.Subscribe(cost => costText.text = cost.ToString());

                Playable.UpdateCostUI.OnNext(Playable.GetCost());


            }
        }

        /// <summary>
        /// プレイ可能オーラの表示非表示
        /// </summary>
        internal void UpdatePlayableAura() => ChangeUsable(Playable.IsPlayable);

        /// <summary>
        /// カード(キャラカードとスペルカードが実装するインターフェース)
        /// </summary>
        protected IPlayable Playable { get; private set; }

        private void Awake()
        {
            //各ポジションに対応する見た目(GameObject) と ポジションを紐づける
            viewMap.Add(PosEnum.Deck, this.transform.Find("_DeckView").gameObject);
            viewMap.Add(PosEnum.Hand, this.transform.Find("_HandView").gameObject);
            viewMap.Add(PosEnum.Field, this.transform.Find("_FieldView").gameObject);
            viewMap.Add(PosEnum.Discard, this.transform.Find("_DiscardView").gameObject);

        }

        #endregion

    }


}

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
    /// �J�[�h�̌����ڂ��쐬����@�\
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
        /// �J�[�h�����݂���ʒu(��D�A�R�D�A�f�b�L�A�t�B�[���h)�ƁA
        /// ���̈ʒu�Ɉړ������ۂɃA�N�e�B�u�ɂ���Q�[���I�u�W�F�N�g��R�Â���
        /// </summary>
        private Dictionary<PosEnum, GameObject> viewMap = new Dictionary<PosEnum, GameObject>();

        #region public methods

        /// <summary>
        /// �J�[�h�̕\�����A�����̃|�W�V�����̌����ڂɕς���
        /// </summary>
        /// <param name="pos">�|�W�V����</param>
        public void ChangeObject(PosEnum pos)
        {
            CurrentPos = pos;
            viewMap.ForEach(pos_obj => pos_obj.Value.SetActive(pos_obj.Key == pos));
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="isPlayer">�v���C���[�̃J�[�h���ۂ�</param>
        /// <param name="cardBook">�J�[�h�̃f�[�^�x�[�X</param>
        public  virtual void Initialize(bool isPlayer, CardBook cardBook, bool deckMaking = false)
        {
            Playable = this.GetComponent<IPlayable>();

            //�J�[�h�̋󔒂ɕ�����Sprite��ݒ�
            cardNameText.text = cardBook.CardName;
            description.text = cardBook.Description;
            cardImage_hand.sprite = cardBook.CardImage;
            cardImage_field.sprite = cardBook.CardImage;

            //�J�[�h�̊G���̑傫���𒲐�
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

                //�v���C�\���ۂ��������I�[���̕\��(��\��)
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
        /// �v���C�\���ۂ���ύX����(�I�[���̕\����\����؂�ւ���)
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
                //�R�X�g���ύX���ꂽ�ۂɔ��s�����ʒm���󂯎��(�R�X�g�ɕ\����ύX����)
                Playable.UpdateCostUI.Subscribe(cost => costText.text = cost.ToString());

                Playable.UpdateCostUI.OnNext(Playable.GetCost());


            }
        }

        /// <summary>
        /// �v���C�\�I�[���̕\����\��
        /// </summary>
        internal void UpdatePlayableAura() => ChangeUsable(Playable.IsPlayable);

        /// <summary>
        /// �J�[�h(�L�����J�[�h�ƃX�y���J�[�h����������C���^�[�t�F�[�X)
        /// </summary>
        protected IPlayable Playable { get; private set; }

        private void Awake()
        {
            //�e�|�W�V�����ɑΉ����錩����(GameObject) �� �|�W�V������R�Â���
            viewMap.Add(PosEnum.Deck, this.transform.Find("_DeckView").gameObject);
            viewMap.Add(PosEnum.Hand, this.transform.Find("_HandView").gameObject);
            viewMap.Add(PosEnum.Field, this.transform.Find("_FieldView").gameObject);
            viewMap.Add(PosEnum.Discard, this.transform.Find("_DiscardView").gameObject);

        }

        #endregion

    }


}

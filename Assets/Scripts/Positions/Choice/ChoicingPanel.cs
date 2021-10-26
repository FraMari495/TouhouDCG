using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChoicingPanel : MonoBehaviour
{
    [SerializeField] private Button endButton;
    [SerializeField] private AudioClip endSE;
    private Image buttonImage;
    private Text buttonText;
    private bool endButtonClicked = false;
    private int number;
    private CanvasGroup cg;
    [SerializeField] private Transform layout;
    private List<IPlayable> options;


    public List<IPlayable> selected;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playables">�I����</param>
    /// <param name="number">�I���\�Ȗ���</param>
    /// <param name="onSelected">���肵���Ƃ���Callback</param>
    public IEnumerator  StartSelecting(List<IPlayable> playables,int number, Action<List<IPlayable>> onSelected)
    {
        cg = this.GetComponent<CanvasGroup>();
        options = playables;
        Show(true);

        this.number = number;
        List<CardInputHandler> cards = playables.ConvertAll(c => c.GameObject.GetComponent<CardInputHandler>());

        foreach (var item in cards)
        {
            // item.ChangePos(PosEnum.Hand);
            item.GetComponent<CardVisualController>().ChangeObject(PosEnum.Hand);

            item.transform.SetParent(layout, false);
        }



        buttonImage = endButton.GetComponent<Image>();
        buttonText = endButton.GetComponentInChildren<Text>();



        selected = new List<IPlayable>();
        ButtonEnableCheck();

        yield return Selecting(cards, onSelected);

    }

    /// <summary>
    /// �I���@�\�̖{��
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="onSelected"></param>
    /// <returns></returns>
    private IEnumerator Selecting(List<CardInputHandler> cards, Action<List<IPlayable>> onSelected)
    {
        //�J�[�h�́A�I��p�R���|�[�l���g�������A����������
        List<ChoicingButton> cardButtons = cards.ConvertAll(c => c.GetComponentInChildren<ChoicingButton>());
        cardButtons.ForEach(c => c.Initialize(OptionClicked));

        //�I���I���܂�(endButtonClicked�t���O�����܂�)�҂�
        yield return new WaitWhile(() => !endButtonClicked);

        //�I�����I��������

        //���ꂼ��̃J�[�h�ɁA�I���I����`����
        cardButtons.ForEach(c => c.EndSelecting());

        //�I���I���̃R�[���o�b�N���\�b�h���Ă�
        onSelected(selected);

        //�I�΂�Ȃ������J�[�h�̓f�b�L�̌����ڂɖ߂�
        cards.Where(c => !selected.Contains(c.GetComponent<IPlayable>())).ForEach(c => c.ChanegCardView(PosEnum.Deck));
        //foreach (var item in cards)
        //{
        //    if (!selected.Contains(item.GetComponent<IPlayable>())) item.ChanegCardView( PosEnum.Deck);
        //}

        //�I���p�l�������
        Show(false);
    }

    /// <summary>
    /// �I���I���{�^�����N���b�N���ꂽ�ۂɌĂ΂��
    /// </summary>
    public void EndButtonClicked()
    {
        //�w�薇���I������Ă��邩���m�F
        if (selected.Count == number)
        {
            //�I�����ꂽ�J�[�h�̐e�I�u�W�F�N�g���L�����o�X�ɂ���
            options.ForEach(c => c.GameObject.transform.SetParent(GameObject.Find("Canvas").transform));

            //�I���I���t���O�𗧂Ă�(�R���[�`���̏������ĊJ�����)
            endButtonClicked = true;

            //SE��炷
            SoundManager.I.PlaySE(endSE);
        }
    }

    /// <summary>
    /// �I�������N���b�N���ꂽ�Ƃ��ɌĂ΂��
    /// </summary>
    private void OptionClicked(IPlayable card,bool selected)
    {
        if (selected)
        {
            if (!this.selected.Contains(card))
            {
                this.selected.Add(card);
                Debug.Log(card.GameObject + "��ǉ����܂���");
            }
            else
            {
                Debug.LogError("���������Ƃ�Ă��܂���");
            }
        }
        else
        {
            if (!this.selected.Remove(card))
            {
                Debug.LogError("���������Ƃ�Ă��܂���");
            }
            else
            {
                Debug.Log(card.GameObject + "�����O���܂���");
            }
        }
        Debug.Log("���݂�"+string.Join(",",this.selected.ConvertAll(c=>c.GameObject))+"���I������Ă��܂�");

        ButtonEnableCheck();
    }

    private void Show(bool show)
    {
        cg.alpha = show ? 1 : 0;
        cg.blocksRaycasts = show;
    }

    private void ButtonEnableCheck()
    {
        bool enable = selected.Count == number;
        buttonImage.color = enable ? Color.white : Color.gray;
        buttonText.text = enable?"����!":"3���I��";
        endButton.enabled = enable;
    }

}

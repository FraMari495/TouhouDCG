using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

/// <summary>
/// �^�[���I��������ۂɃN���b�N����{�^��
/// </summary>
public class TurnEndButton : MonoSingleton<TurnEndButton>
{
    private Image buttonImage;
    private Text buttonText;
    private Button button;

    /// <summary>
    /// �{�^���������ꂽ�ۂɌĂ΂�郁�\�b�h
    /// </summary>
    public void ChangeTurnButtonClicked()
    {
        //����̃^�[���ɂȂ邽�߁A�N���b�N�s�ɂ���
        ChangeTurn(false);

        //�R�}���h�𐶐�
        Command.CommandManager.I.Run(new Command.Command_TurnEnd(true));
    }

    /// <summary>
    /// show ? �{�^�����N���b�N�\�ɂ��� : �N���b�N�s�ɂ���
    /// </summary>
    /// <param name="show"></param>
    public void ChangeTurn(bool show)
    {
        buttonImage.color = show ? Color.white : Color.gray;
        buttonText.text = show ? "�^�[���I��" : "����̃^�[��";
        button.enabled = show;
    }

    private void Wait(bool wait)
    {
        buttonImage.color = !wait ? Color.white : Color.gray;
        button.enabled = !wait;
    }



    public void Awake()
    {
        buttonImage = this.GetComponent<Image>();
        buttonText = this.GetComponentInChildren<Text>();
        button = this.GetComponent<Button>();
        Debug.Log(this.GetType());

        ReactivePropertyList.I.O_EndTurnNotif.Subscribe(MakeAnimation);
        ReactivePropertyList.I.O_Wait.Subscribe(Wait);

    }

    /// <summary>
    /// �^�[������シ��A�j���[�V�������Đ�
    /// </summary>
    /// <param name="endedTurn">"�I��"�����^�[��</param>
    private void MakeAnimation(bool endedTurn)
    {
        //�^�[���`�F���W�̃e�L�X�g�A�j���[�V����
        AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.TurnEndAnimation(!endedTurn), "�^�[���`�F���W");
    }
}

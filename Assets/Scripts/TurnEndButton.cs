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
        Show(false);

        //�R�}���h�𐶐�
        Command.CommandManager.I.Run(new Command.Command_TurnEnd(true));
    }

    /// <summary>
    /// show ? �{�^�����N���b�N�\�ɂ��� : �N���b�N�s�ɂ���
    /// </summary>
    /// <param name="show"></param>
    public void Show(bool show)
    {
        buttonImage.color = show ? Color.white : Color.gray;
        buttonText.text = show ? "�^�[���I��" : "����̃^�[��";
        button.enabled = show;
    }



    public void Awake()
    {
        buttonImage = this.GetComponent<Image>();
        buttonText = this.GetComponentInChildren<Text>();
        button = this.GetComponent<Button>();
        Debug.Log(this.GetType());
    }
}

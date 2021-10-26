using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

/// <summary>
/// ターン終了時する際にクリックするボタン
/// </summary>
public class TurnEndButton : MonoSingleton<TurnEndButton>
{
    private Image buttonImage;
    private Text buttonText;
    private Button button;

    /// <summary>
    /// ボタンが押された際に呼ばれるメソッド
    /// </summary>
    public void ChangeTurnButtonClicked()
    {
        //相手のターンになるため、クリック不可にする
        Show(false);

        //コマンドを生成
        Command.CommandManager.I.Run(new Command.Command_TurnEnd(true));
    }

    /// <summary>
    /// show ? ボタンをクリック可能にする : クリック不可にする
    /// </summary>
    /// <param name="show"></param>
    public void Show(bool show)
    {
        buttonImage.color = show ? Color.white : Color.gray;
        buttonText.text = show ? "ターン終了" : "相手のターン";
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

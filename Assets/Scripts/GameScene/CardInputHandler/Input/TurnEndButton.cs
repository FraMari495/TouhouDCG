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
        ChangeTurn(false);

        //コマンドを生成
        Command.CommandManager.I.Run(new Command.Command_TurnEnd(true));
    }

    /// <summary>
    /// show ? ボタンをクリック可能にする : クリック不可にする
    /// </summary>
    /// <param name="show"></param>
    public void ChangeTurn(bool show)
    {
        buttonImage.color = show ? Color.white : Color.gray;
        buttonText.text = show ? "ターン終了" : "相手のターン";
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
    /// ターンが交代するアニメーションを再生
    /// </summary>
    /// <param name="endedTurn">"終了"されるターン</param>
    private void MakeAnimation(bool endedTurn)
    {
        //ターンチェンジのテキストアニメーション
        AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.TurnEndAnimation(!endedTurn), "ターンチェンジ");
    }
}

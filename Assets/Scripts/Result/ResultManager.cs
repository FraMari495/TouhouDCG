using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームセットの画面のデザイン
/// </summary>
public class ResultManager : MonoBehaviour
{
    [SerializeField] private AudioClip enterClip;
    [SerializeField] private GameObject youwinText;
    [SerializeField] private GameObject youloseText;
    [SerializeField] private Image myChara;
    [SerializeField] private Image rivalChara;


    public void GameSet(bool win,Sprite myChara,Sprite rivalChara)
    {
        this.myChara.sprite = myChara;
        this.rivalChara.sprite = rivalChara;

        (win ? youloseText : youwinText).SetActive(false);
    }

    /// <summary>
    /// ホームへ戻るボタンがクリックされた際に呼ばれる
    /// </summary>
    public void GoTitleButtonClicked()
    {
        SoundManager.I.PlaySE(enterClip);

        ConnectionManager.Instance.CloseRoom();

        WBTransition.SceneManager.LoadScene("Lobby");
    }
}

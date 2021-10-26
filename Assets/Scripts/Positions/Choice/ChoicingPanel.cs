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
    /// <param name="playables">選択肢</param>
    /// <param name="number">選択可能な枚数</param>
    /// <param name="onSelected">決定したときのCallback</param>
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
    /// 選択機能の本体
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="onSelected"></param>
    /// <returns></returns>
    private IEnumerator Selecting(List<CardInputHandler> cards, Action<List<IPlayable>> onSelected)
    {
        //カードの、選択用コンポーネントを見つけ、初期化する
        List<ChoicingButton> cardButtons = cards.ConvertAll(c => c.GetComponentInChildren<ChoicingButton>());
        cardButtons.ForEach(c => c.Initialize(OptionClicked));

        //選択終了まで(endButtonClickedフラグが立つまで)待つ
        yield return new WaitWhile(() => !endButtonClicked);

        //選択が終了したら

        //それぞれのカードに、選択終了を伝える
        cardButtons.ForEach(c => c.EndSelecting());

        //選択終了のコールバックメソッドを呼ぶ
        onSelected(selected);

        //選ばれなかったカードはデッキの見た目に戻す
        cards.Where(c => !selected.Contains(c.GetComponent<IPlayable>())).ForEach(c => c.ChanegCardView(PosEnum.Deck));
        //foreach (var item in cards)
        //{
        //    if (!selected.Contains(item.GetComponent<IPlayable>())) item.ChanegCardView( PosEnum.Deck);
        //}

        //選択パネルを閉じる
        Show(false);
    }

    /// <summary>
    /// 選択終了ボタンがクリックされた際に呼ばれる
    /// </summary>
    public void EndButtonClicked()
    {
        //指定枚数選択されているかを確認
        if (selected.Count == number)
        {
            //選択されたカードの親オブジェクトをキャンバスにする
            options.ForEach(c => c.GameObject.transform.SetParent(GameObject.Find("Canvas").transform));

            //選択終了フラグを立てる(コルーチンの処理が再開される)
            endButtonClicked = true;

            //SEを鳴らす
            SoundManager.I.PlaySE(endSE);
        }
    }

    /// <summary>
    /// 選択肢がクリックされたときに呼ばれる
    /// </summary>
    private void OptionClicked(IPlayable card,bool selected)
    {
        if (selected)
        {
            if (!this.selected.Contains(card))
            {
                this.selected.Add(card);
                Debug.Log(card.GameObject + "を追加しました");
            }
            else
            {
                Debug.LogError("整合性がとれていません");
            }
        }
        else
        {
            if (!this.selected.Remove(card))
            {
                Debug.LogError("整合性がとれていません");
            }
            else
            {
                Debug.Log(card.GameObject + "を除外しました");
            }
        }
        Debug.Log("現在は"+string.Join(",",this.selected.ConvertAll(c=>c.GameObject))+"が選択されています");

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
        buttonText.text = enable?"決定!":"3枚選択";
        endButton.enabled = enable;
    }

}

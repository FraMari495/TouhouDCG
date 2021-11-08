using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ゲーム中、カードをクリックすると詳細が表示される。
/// この説明用の画面を司る
/// </summary>
public class CardExplanation : MonoSingleton<CardExplanation>,IPointerClickHandler
{
    /// <summary>
    /// 表示、非表示を切り替える際に用いる
    /// </summary>
    private CanvasGroup cg;

    /// <summary>
    /// 表示するカードオブジェクト
    /// </summary>
    private GameObject cardObj;

    /// <summary>
    /// 表示位置
    /// </summary>
    [SerializeField]private Transform center;

  
    private void Awake()
    {
        Debug.Log(this.GetType());

        cg = this.GetComponent<CanvasGroup>();

        //通常は非表示
        Show(false);
        Debug.Log(this.GetType()+"end");

    }

    public void Initialize(GameObject cardObj)
    {
        this.cardObj = cardObj;
        cardObj.transform.SetParent(this.transform);

        //カードはクリック不可にする
        cardObj.GetComponent<CanvasGroup>().blocksRaycasts = false;

        //カードの位置を調節
        cardObj.transform.position = center.position;

        //拡大
        cardObj.GetComponent<RectTransform>().localScale= new Vector3(3,3,3);

        //表示
        Show(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //クリックされたときにこの画面を閉じる
        Destroy(cardObj);
        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }

    /// <summary>
    /// 非表示にする
    /// </summary>
    private void Show(bool show)
    {
        cg.alpha = show ? 1 : 0; ;
        cg.blocksRaycasts = show;
    }
}

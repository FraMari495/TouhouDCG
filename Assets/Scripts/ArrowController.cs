using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 攻撃時に表示する矢印
/// </summary>
[RequireComponent(typeof(CanvasGroup),typeof(Image))]
public class ArrowController : MonoBehaviour
{
    private RectTransform rect;
    private Transform trn;
    
    /// <summary>
    /// 矢印オブジェクトの生成
    /// </summary>
    /// <param name="startTrn"></param>
    public static void CreateArrow(PointerEventData eventData,Transform trn)
    {
        //ArrowControllerをもつオブジェクトの生成(自動的にCanvasGroup,Imageもアタッチされる)
        ArrowController ctrl = (new GameObject("Arrow",typeof(ArrowController))).GetComponent<ArrowController>();
        ctrl.transform.SetParent(trn);
       
        //矢印の画像を設定
        ctrl.GetComponent<Image>().sprite = Resources.Load<Sprite>("Arrow");
        ctrl.GetComponent<Image>().maskable = false;

        //矢印オブジェクトの生成が終わったため、初期設定を行う
        ctrl.Initialize(eventData);
    }

    private void Initialize(PointerEventData eventData)
    {
        //Raycastをブロックしない
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;

        //オブジェクトの位置を、矢印の始点に揃える
        rect = this.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0, 0.5f);

        //クリックされていない場合、このオブジェクトを削除する
        if (!Input.GetMouseButton(0))
        {
            Debug.Log("OnBeginDragメソッド中にCreateArrowを呼んでください");
            Destroy(this.gameObject);
            return;
        }

        //this.transformは遅いです。そのためこれをループ中(Scalingのwhileループ)には使うべきではありません
        //よって、あらかじめthis.transformの中身をtrnに入れておきます
        trn = this.transform;

        //始点をCreateArrow()の引数のポジションに設定
        trn.position = eventData.pointerDrag.transform.position;

        //マウスの動きに合わせて伸び縮みさせる
        StartCoroutine(Scaling(eventData));
    }

    private IEnumerator Scaling(PointerEventData eventData)
    {
        ShowTargetAura(true);
        while (true)
        {
            //マウスが離されていたら、ループを抜ける( = このオブジェクトを削除する)
            if (eventData.pointerDrag==null) break;

            //終点(マウスのポジション)-始点(このオブジェクトの位置)
            var diff = eventData.position - (Vector2)trn.position;

            //偏角(度)
            var theta = Mathf.Atan2(diff.y , diff.x) * 180 / Mathf.PI;

            //矢印の長さを、diffの大きさに合わせる
            rect.sizeDelta = new Vector3(diff.magnitude, rect.sizeDelta.y);

            //向きをdiffに合わせる
            trn.rotation = Quaternion.Euler(0, 0, theta);

            //0.1秒待つ
            yield return new WaitForSeconds(0.1f);
        }
        ShowTargetAura(false);

        //ドラッグが終わったため、矢印を削除する
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 攻撃可能なカードのオーラを表示する
    /// </summary>
    public void ShowTargetAura(bool show)
    {
        Position.Field.I(false).GetComponentsInChildren<Player.Target>().ForEach(c => c.ShowAura(show));
        Status_Hero.I(false).GetComponentInChildren<Player.Target>().ShowAura(show);
    }

}

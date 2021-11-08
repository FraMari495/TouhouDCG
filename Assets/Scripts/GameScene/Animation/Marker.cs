using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{

    /// <summary>
    /// カードのアニメーションを行う際に、移動先の目印として使用
    /// (このゲームオブジェクトめがけてアニメーションする)
    /// </summary>
    internal class Marker : MonoBehaviour
    {
        /// <summary>
        /// このオブジェクトのTransformを格納
        /// 計算量削減のため
        /// </summary>
        private Transform Trn { get; set; }

        /// <summary>
        /// 引数のオブジェクトを、このオブジェクトの子オブジェクトとする
        /// (引数のオブジェクトのlocalPositionを zero vectorとすると、両者の位置が重なる)
        /// </summary>
        /// <param name="obj"></param>
        internal void Initialize(GameObject obj)
        {
            //このオブジェクトのTransformを格納
            Trn = this.transform;

            //移動オブジェクトを、このオブジェクトの子とする
            obj.transform.SetParent(Trn, true);

            //markerのサイズを、移動させたいオブジェクトの画像サイズにあわせる
            this.GetComponent<RectTransform>().sizeDelta = obj.GetComponent<RectTransform>().sizeDelta;

            StartCoroutine(CheckChild());
        }

        /// <summary>
        /// 移動オブジェクトがこのオブジェクトの子から外れた場合、このオブジェクトを削除する
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckChild()
        {
            yield return new WaitWhile(() => Trn.childCount > 0);
            Destroy(this.gameObject);
        }
    }
}

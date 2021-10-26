using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Marker : MonoBehaviour
{
    private Transform trn;

    public void Initialize(GameObject obj)
    {
        trn = this.transform;
        RectTransform rect = obj.GetComponent<RectTransform>();

        obj.transform.SetParent(trn,true);
        this.GetComponent<RectTransform>().sizeDelta = rect.sizeDelta;

        StartCoroutine(CheckChild());
    }

    public IEnumerator CheckChild()
    {
        yield return new WaitWhile(() => trn.childCount > 0);
        Destroy(this.gameObject);
    }
}

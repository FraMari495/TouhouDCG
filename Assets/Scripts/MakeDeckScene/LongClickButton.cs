using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    enum ClickType
    {
        None,
        Normal,
        Long
    }
    private Coroutine coroutine;

    //private void Start()
    //{
    //    Debug.Log(this.gameObject);
    //    this.gameObject.AddComponent<Button>().onClick.AddListener(OnPointerDown);

    //}


    public void OnPointerDown(PointerEventData eventData)
    {
        coroutine = StartCoroutine(LongTapJudge());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (coroutine == null) return;
        StopCoroutine(coroutine);
        coroutine = null;
        if (clickType == ClickType.Normal)
        {
            NormalClick();
        }
    }

    private ClickType clickType = ClickType.None;
    private IEnumerator LongTapJudge()
    {
        clickType = ClickType.Normal;
        yield return new WaitForSeconds(0.2f);
        clickType = ClickType.None;
        yield return new WaitForSeconds(0.3f);
        LongClick();
        coroutine = null;
    }

    protected abstract void NormalClick();
    protected abstract void LongClick();

    private void OnDestroy()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}


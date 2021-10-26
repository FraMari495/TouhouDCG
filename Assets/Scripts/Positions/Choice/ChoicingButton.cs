using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChoicingButton : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private GameObject checkMark;
    private Action<IPlayable,bool> onClicked;
    private CanvasGroup cg;
    bool check = false;

    public void Initialize(Action<IPlayable,bool> onClicked)
    {
        checkMark.SetActive(false);
        check = false;
        cg = this.GetComponent<CanvasGroup>();
        this.onClicked = onClicked;
        Show(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        check = !check;
        checkMark.SetActive(check);
        onClicked(this.GetComponentInParent<IPlayable>(), check);
    }

    public void EndSelecting()
    {
        Show(false);
    }

    private void Show(bool show)
    {
        cg.alpha = show ? 1 : 0;
        cg.blocksRaycasts = show;
    }

}

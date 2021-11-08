using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NullState : CardState
{
    public NullState(IPlayable playable) : base(playable)
    {
    }

    public override bool Showing => true;

    public override PosEnum Pos => PosEnum.None;

    public override bool Enter() => true;

    public override void OnBeginDrag(PointerEventData eventData)
    {
    }

    public override void OnDrag(PointerEventData eventData)
    {
    }

    public override IEnumerator OnEndDrag(PointerEventData eventData)
    {
        yield break;
    }
}

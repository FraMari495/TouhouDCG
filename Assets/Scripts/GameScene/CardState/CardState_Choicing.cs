using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardState_Choicing : CardState
{
    public CardState_Choicing(IPlayable playable) : base(playable)
    {
    }

    public override bool Showing => true;

    public override PosEnum Pos => PosEnum.Choicing;

    public override bool Enter()
    {
        AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.MoveToChoicingPanel(Playable), "ƒhƒ[");
        return true;
    }

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

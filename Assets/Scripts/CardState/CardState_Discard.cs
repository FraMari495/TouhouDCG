using Position;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardState_Discard : CardState
{
    private bool Defeated { get; set; }
    public CardState_Discard(IPlayable playable,bool defeated) : base(playable)
    {
        Defeated = defeated;
    }

    public override bool Showing => true;

    public override PosEnum Pos => PosEnum.Discard;

    public override bool Enter()
    {
        AnimationManager.I.AddSequence<AnimationManager.Defeated>(() => AnimationMaker.DeadAnimation(Playable, Defeated), "ƒJ[ƒh‚Ì”j‰ó");
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
        yield return null;
    }
}

//using Position;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardState_Deck : CardState
{
    private int Index { get; set; }

    public override bool Showing =>false;

    public override PosEnum Pos  => PosEnum.Deck;

    public CardState_Deck(IPlayable playable,int index) : base(playable)
    {
        Index = index;
    }

    public override bool Enter()
    {
        AnimationManager.I.AddSequence<AnimationManager.ToDeck>(() => AnimationManager.I.AnimationMaker.ToDeckAnimation(Playable),"ƒfƒbƒL’Ç‰Á");
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

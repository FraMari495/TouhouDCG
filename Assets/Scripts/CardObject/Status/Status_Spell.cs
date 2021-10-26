using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Status_Spell : StatusBase, ICardViewInitializer, IPlayable
{
    [SerializeField] private Text idText;

    public override CardType Type => CardType.Spell;
    public Cost Cost { get; private set; }
    public GameObject GameObject => gameObject;
    public string CardName { get; private set; }
    public PlayableId PlayableId { get; private set; } = PlayableId.Default;
    public int CardBookId { get; private set; }
    public CardBook CardBook { get; set; }
    public void RequirePlayableId(int? playableId)
    {
        if (PlayableId != PlayableId.Default)
        {
            Debug.LogError("Šù‚ÉID‚ªÝ’è‚³‚ê‚Ä‚¢‚Ü‚·");
            return;
        }
        PlayableId = PlayableIdManager.I.GetId(this, playableId);
        idText.text = PlayableId.ToString();
    }

    protected override string ObjectName => CardName;

    public int GetCost()
    {
        return Cost;
    }

    public void Initialize(bool isPlayer, CardBook cardBook)
    {
        CardBook = cardBook;
        IsPlayer = isPlayer;

        //hp = new RangeInt(20, 20);
        Cost = new Cost(cardBook.Cost);

        CardName = cardBook.CardName;
        OnPlayAbility = cardBook.OnPlaySkill;
        CardBookId = cardBook.Id;
    }

    public OnPlayAbility OnPlayAbility { get; private set; }


    private bool playable;
    public bool IsPlayable
    {
        get => playable; set
        {
            playable = value;
        }
    }

    public Subject<int> UpdateCostUI { get; } = new Subject<int>();

    public Subject<(PosEnum from, PosEnum to, int index)> UpdatePosition { get; } = new Subject<(PosEnum from, PosEnum to, int index)>();
}

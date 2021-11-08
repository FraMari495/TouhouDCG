using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;




/// <summary>
/// カードが存在しうるポジション
/// </summary>
public enum PosEnum
{
    Deck = 0,
    Hand = 1,
    Field = 2,
    Discard = 3,
    Choicing = 4,
    None = -1
}


/// <summary>
/// カードの種類
/// (ヒーローもカードの一種と考える)
/// </summary>
public enum CardType
{
    Chara,
    Spell,
    Hero
}
public interface IRequireTarget
{
    bool TargetRequired { get; }
    Condition Condition { get; }
    CardType[] TargetType { get; }
}

public interface IOnDefeatedAbility { }
public interface IOnTurnEndAbility { }
public interface IBombAbility { }


public abstract class StatusBase : MonoBehaviour
{
    protected abstract string ObjectName { get; }

    public abstract CardType Type { get; }

    public bool IsPlayer { get; protected set; }

    public override string ToString() => ObjectName;
}

public interface IPlayable
{
    CardType Type { get; }
    int GetCost();
    IRequireTarget OnPlayAbility { get; }
    bool IsPlayable { get; set; }
    GameObject GameObject { get; }
    bool IsPlayer { get; }
    int PlayableCardId { get; }
    void RequirePlayableId(int? playableId);
    int CardBookId { get; }
    public void Initialize(bool isPlayer, CardBook cardBook, bool deckMaking = false);
    CardBook CardBook { get; }
    Subject<int> UpdateCostUI { get; }
    Subject<(PosEnum from, PosEnum to, int index)> UpdatePosition { get; }
    int? GetAtk();
    int? GetHp();


}

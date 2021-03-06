using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 現在のターンがどちらであるか
/// </summary>
public class TurnManager : MonoSingleton<TurnManager>
{

    private TurnManager() { }

    public bool Turn { get; private set; }
    internal void ChangeTurn() => Turn = !Turn;
    internal void GameStart(bool isPlayer) => Turn = isPlayer;
}

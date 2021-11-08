using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Œ»İ‚Ìƒ^[ƒ“‚ª‚Ç‚¿‚ç‚Å‚ ‚é‚©
/// </summary>
public class TurnManager : MonoSingleton<TurnManager>
{

    private TurnManager() { }

    public bool Turn { get; private set; }
    internal void ChangeTurn() => Turn = !Turn;
    internal void GameStart(bool isPlayer) => Turn = isPlayer;
}

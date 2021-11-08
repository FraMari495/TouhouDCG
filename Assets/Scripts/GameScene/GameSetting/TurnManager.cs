using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���݂̃^�[�����ǂ���ł��邩
/// </summary>
public class TurnManager : MonoSingleton<TurnManager>
{

    private TurnManager() { }

    public bool Turn { get; private set; }
    internal void ChangeTurn() => Turn = !Turn;
    internal void GameStart(bool isPlayer) => Turn = isPlayer;
}

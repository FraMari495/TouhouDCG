using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class ReactivePropertyList:MonoSingleton<ReactivePropertyList>
{
    

    public bool FirstAttack { get; set; } = true;
    public bool Tutorial { get; set; }

    #region Subject (private)
    private ISubject<bool> _NewTurnNotif { get; } = new Subject<bool>();
    private ISubject<bool> _EndTurnNotif { get; } = new Subject<bool>();
    private ISubject<Unit> _Judge { get; } = new Subject<Unit>();
    private ISubject<bool> _UpdatePlayableAura { get; } = new Subject<bool>();
    private ISubject<(bool isPlayer, bool show)> _ShowAttackTarget { get; } = new Subject<(bool isPlayer, bool show)>();
    private ISubject<bool> _Wait { get; } = new Subject<bool>();
    #endregion

    #region Observable (public)
    public IObservable<Unit> O_Judge => _Judge;
    public IObservable<bool> O_NewTurnNotif => _NewTurnNotif;
    public IObservable<bool> O_EndTurnNotif => _EndTurnNotif;
    public IObservable<bool> O_UpdatePlayableAura => _UpdatePlayableAura;
    public IObservable<(bool isPlayer,bool show)> O_ShowAttackTarget => _ShowAttackTarget;
    public IObservable<bool> O_Wait => _Wait;
    #endregion

    /// <summary>
    /// !!!ユーザーの入力直前に必ず呼ぶ!!!
    /// 死亡判定、プレイ可能なカードの判定、アタッカーの選択肢等
    /// </summary>
    public void StartJudge(bool rpc) => _Judge.OnNext(Unit.Default);

    /// <summary>
    /// プレイ可能を示すオーラの表示(非表示)
    /// </summary>
    /// <param name="isPlayer"></param>
    public void UpdatePlayableAura(bool isPlayer) => _UpdatePlayableAura.OnNext(isPlayer);

    /// <summary>
    /// ターン交代
    /// </summary>
    public void ChangeTurn()
    {
        //ターン終了の通知
        _EndTurnNotif.OnNext(TurnManager.I.Turn);

        //ターンを変更
        TurnManager.I.ChangeTurn();

        //新しいターン開始の通知
        _NewTurnNotif.OnNext(TurnManager.I.Turn);
    }

    /// <summary>
    /// ゲーム開始
    /// 新しいターンを開始してBGMを再生する
    /// </summary>
    /// <param name="myTurn"></param>
    public void GameStart(bool isPlayer,bool tutorial)
    {

        //新しいターンの開始
        TurnManager.I.GameStart(isPlayer);
        _EndTurnNotif.OnNext(!isPlayer);
        _NewTurnNotif.OnNext(isPlayer);

        //BGMの再生
        var intro = Resources.Load<AudioClip>("sadonofutatsuiwa_into_takurosu");
        var main = Resources.Load<AudioClip>("sadonofutatsuiwa_main_takurosu");
        SoundManager.I.Play(intro, main);
    }

    /// <summary>
    /// アタッカーカードをドッグした際に呼ばれる
    /// アタック可能な対象にオーラを表示する
    /// </summary>
    /// <param name="show">表示(非表示)</param>
    /// <param name="isPlayer">isPlayerのターゲットオーラを表示(非表示)</param>
    public void ShowAttackTarget(bool isPlayer,bool show) => _ShowAttackTarget.OnNext((isPlayer,show));

    public void Wait(bool wait) => _Wait.OnNext(wait);
}

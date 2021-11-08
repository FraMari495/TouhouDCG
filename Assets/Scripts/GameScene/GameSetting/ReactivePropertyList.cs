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
    /// !!!���[�U�[�̓��͒��O�ɕK���Ă�!!!
    /// ���S����A�v���C�\�ȃJ�[�h�̔���A�A�^�b�J�[�̑I������
    /// </summary>
    public void StartJudge(bool rpc) => _Judge.OnNext(Unit.Default);

    /// <summary>
    /// �v���C�\�������I�[���̕\��(��\��)
    /// </summary>
    /// <param name="isPlayer"></param>
    public void UpdatePlayableAura(bool isPlayer) => _UpdatePlayableAura.OnNext(isPlayer);

    /// <summary>
    /// �^�[�����
    /// </summary>
    public void ChangeTurn()
    {
        //�^�[���I���̒ʒm
        _EndTurnNotif.OnNext(TurnManager.I.Turn);

        //�^�[����ύX
        TurnManager.I.ChangeTurn();

        //�V�����^�[���J�n�̒ʒm
        _NewTurnNotif.OnNext(TurnManager.I.Turn);
    }

    /// <summary>
    /// �Q�[���J�n
    /// �V�����^�[�����J�n����BGM���Đ�����
    /// </summary>
    /// <param name="myTurn"></param>
    public void GameStart(bool isPlayer,bool tutorial)
    {

        //�V�����^�[���̊J�n
        TurnManager.I.GameStart(isPlayer);
        _EndTurnNotif.OnNext(!isPlayer);
        _NewTurnNotif.OnNext(isPlayer);

        //BGM�̍Đ�
        var intro = Resources.Load<AudioClip>("sadonofutatsuiwa_into_takurosu");
        var main = Resources.Load<AudioClip>("sadonofutatsuiwa_main_takurosu");
        SoundManager.I.Play(intro, main);
    }

    /// <summary>
    /// �A�^�b�J�[�J�[�h���h�b�O�����ۂɌĂ΂��
    /// �A�^�b�N�\�ȑΏۂɃI�[����\������
    /// </summary>
    /// <param name="show">�\��(��\��)</param>
    /// <param name="isPlayer">isPlayer�̃^�[�Q�b�g�I�[����\��(��\��)</param>
    public void ShowAttackTarget(bool isPlayer,bool show) => _ShowAttackTarget.OnNext((isPlayer,show));

    public void Wait(bool wait) => _Wait.OnNext(wait);
}

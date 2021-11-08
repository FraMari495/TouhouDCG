using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using Command;

public interface IOfflineRival
{
    IEnumerator Run();
}

public class RivalInputManager : MonoSingleton<RivalInputManager>
{
    public IOfflineRival OfflineRival { get; set; }

    private void Start()
    {
        ReactivePropertyList.I.O_EndTurnNotif.Subscribe(endTurn=>ChangeTurn(endTurn));
        ReactivePropertyList.I.O_NewTurnNotif.Subscribe(turn=> {
            //ターンチェンジのアニメーション & ターンチェンジ
            AnimationManager.I.AddSequence(() => DOTween.Sequence().AppendCallback(()=>AfterPreparedForNewTurn(turn)), "ターンチェンジ");
        });

    }

    /// <summary>
    /// ターン終了スキル & 
    /// </summary>
    /// <returns></returns>
    public void ChangeTurn(bool endTurn)
    {
        //ターン終了のスキルを発動
        CommandManager.I.OnTurnEnd(endTurn);


    }

    private void AfterPreparedForNewTurn(bool newTurn)
    {
        //オフラインモードで相手のターンの場合、AIを動かす
        if (ConnectionManager.Instance.OfflineMode && !newTurn) StartCoroutine(OfflineRival.Run());

        ConnectionManager.Instance.Judge();
    }
}

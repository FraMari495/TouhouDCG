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
            //�^�[���`�F���W�̃A�j���[�V���� & �^�[���`�F���W
            AnimationManager.I.AddSequence(() => DOTween.Sequence().AppendCallback(()=>AfterPreparedForNewTurn(turn)), "�^�[���`�F���W");
        });

    }

    /// <summary>
    /// �^�[���I���X�L�� & 
    /// </summary>
    /// <returns></returns>
    public void ChangeTurn(bool endTurn)
    {
        //�^�[���I���̃X�L���𔭓�
        CommandManager.I.OnTurnEnd(endTurn);


    }

    private void AfterPreparedForNewTurn(bool newTurn)
    {
        //�I�t���C�����[�h�ő���̃^�[���̏ꍇ�AAI�𓮂���
        if (ConnectionManager.Instance.OfflineMode && !newTurn) StartCoroutine(OfflineRival.Run());

        ConnectionManager.Instance.Judge();
    }
}

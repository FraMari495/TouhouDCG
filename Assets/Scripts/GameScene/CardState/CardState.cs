using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// �X�e�[�g
/// �J�[�h�̓��͎�t�̋�̓I�ȏ�����S��
/// </summary>
public abstract class CardState
{

    /// <summary>
    /// �R���X�g���N�^
    /// </summary>
    /// <param name="playable">�֘A�t������IPlayable</param>
    public CardState(IPlayable playable)
    {
        Playable = playable;
        IsPlayer = playable.IsPlayer;
        Trn = playable.GameObject.transform;
    }

    /// <summary>
    /// �J�[�h��Transform
    /// </summary>
    protected Transform Trn { get; }

    public abstract void OnBeginDrag(PointerEventData eventData);
    public abstract void OnDrag(PointerEventData eventData);
    public abstract IEnumerator OnEndDrag(PointerEventData eventData);

    /// <summary>
    /// �s��(�v���C)�\���ۂ�
    /// </summary>
    public bool IsPlayable => Playable.IsPlayable;

    /// <summary>
    /// �֘A�t�����Ă���IPlayable
    /// </summary>
    public IPlayable Playable { get; }

    /// <summary>
    /// �v���C���[�̃J�[�h���ۂ�
    /// </summary>
    public  bool IsPlayer { get; }

    /// <summary>
    /// �J�[�h�����̏�ԂɑJ�ڂ����Ƃ��̏���
    /// </summary>
    /// <returns�ړ��ɐ���������></returns>
    public abstract bool Enter();


    /// <summary>
    /// �J�[�h���\�������ۂ�
    /// </summary>
    public abstract bool Showing { get; }

    /// <summary>
    /// �J�[�h���u����Ă���ꏊ
    /// </summary>
    public abstract PosEnum Pos { get; }
}

using Command;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using Position;

/// <summary>
/// �J�[�h����D�ɑ��݂���Ƃ��ɗL���ɂȂ�X�e�[�g
/// </summary>
public abstract class CardState_Hand : CardState
{
    protected bool Special { get; private set; }

    /// <summary>
    /// �R���X�g���N�^
    /// </summary>
    /// <param name="playable">�֘A�t������IPlayable</param>
    /// <param name="costText">�R�X�g��\������e�L�X�g</param>
    public CardState_Hand(IPlayable playable,bool special) : base(playable)
    {
        Special = special;
        CanvasGroup = Trn.GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// ��D�ɂ��鎞�A�����̃J�[�h�̂ݕ\��
    /// </summary>
    public override bool Showing => IsPlayer;

    public override PosEnum Pos => PosEnum.Hand;

    /// <summary>
    /// �h���b�O���́AblocksRaycasts��false�Ƃ���
    /// </summary>
    private CanvasGroup CanvasGroup { get; }

    public override bool Enter()
    {

        if (!Special) AnimationManager.I.AddSequence<AnimationManager.ToHand>(() => AnimationManager.I.AnimationMaker.DrawAnimation(Playable), "�h���[");
        else AnimationManager.I.AddSequence<AnimationManager.ToHand>(() => AnimationManager.I.AnimationMaker.SpecialSummonAnimation_Hand(Playable), "�h���[");
        return true;
    }



    #region Dragging
    private Vector3 previousPos;

    //�v���C�ʒu��Field����󂯎��
    public int PlayPos { get; set; }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        //�v���C�ł��Ȃ��ۂ́A�h���b�O�ł��Ȃ�
        if (!IsPlayable) return;

        //�h���b�O���̃I�u�W�F�N�g����L���X�g���u���b�N���Ȃ��悤�ɐݒ�
        CanvasGroup.blocksRaycasts = false;

        //�h���b�O�O�̈ʒu���L��
        previousPos = Trn.parent.position;

        //PlayPos�̒l�́A�h���b�O�J�n���ɂ�-1�ɂ��Ă���
        //EndDrag�̃^�C�~���O��-1�o�Ȃ�������A�v���C�ʒu�����肵�����ƂɂȂ�
        PlayPos = -1;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        //�v���C�ł��Ȃ��ۂ́A�h���b�O�ł��Ȃ�
        if (!IsPlayable) return;

        //�h���b�O���̃I�u�W�F�N�g���|�C���^�[��Ǐ]����悤�ɂ���
        Trn.parent.position = eventData.position;
    }

    public override IEnumerator OnEndDrag(PointerEventData eventData)
    {
        //�v���C�ł��Ȃ��ۂ́A�h���b�O�ł��Ȃ�
        if (!IsPlayable) yield break;

        //���C�L���X�g���u���b�N����悤�ɁA�ݒ��߂�
        CanvasGroup.blocksRaycasts = true;

        //�v���C�ʒu�����܂��Ă���A���}�i������ł�����(�����ŏ���͂��Ȃ�)�A�J�[�h���v���C
        if (PlayPos != -1 && CommandManager.I.GetMana(IsPlayer) >= Playable.GetCost())
        {

            int playingCard = Trn.GetComponentInParent<IPlayable>().PlayableCardId;


            if (Playable.OnPlayAbility!=null && Playable.OnPlayAbility.TargetRequired )
            {
                yield return new TargetSelector().StartSelection((StatusBase)Playable, Playable.OnPlayAbility.TargetType, Playable.OnPlayAbility.Condition, target => OnTargetSelected(playingCard, target));
            }
            else
            {
                OnTargetSelected(playingCard, null);
            }

        }
        else
        {
            //�ʒu��߂�
            Trn.parent.position = previousPos;
        }
    }
    #endregion


    /// <summary>
    /// �X�L���̑ΏۂƂ��đI�����ꂽ�Ƃ��ɌĂ΂��
    /// </summary>
    /// <param name="playingCard"></param>
    /// <param name="target"></param>
    private void OnTargetSelected(int playingCard, StatusBase target)
    {
        Command.Command c = null;
        if (target == null) c = new Command.Command_CardPlay(IsPlayer, playingCard, PlayPos);
        else
        {
            //�R�}���h���쐬
            c = target.Type switch
            {

                //�A�r���e�B�[�^�[�Q�b�g�Ƃ��ăq�[���[���I�����ꂽ�ꍇ
                CardType.Hero => new Command.Command_CardPlay(IsPlayer, playingCard, PlayPos, target.IsPlayer),

                //�A�r���e�B�[�^�[�Q�b�g�Ƃ��ăJ�[�h���I�����ꂽ�ꍇ
                CardType.Chara => new Command.Command_CardPlay(IsPlayer, playingCard, PlayPos, (target as IPlayable).PlayableCardId, target.IsPlayer),

                //�A�r���e�B�[�^�[�Q�b�g���I������Ă��Ȃ��ꍇ
                _ => throw new NotImplementedException()
            };
        }

        //�R�}���h�����s
        Command.CommandManager.I.Run(c);

        //���[�U�[�̓��͂̒��O
        ConnectionManager.Instance.Judge();
    }


}

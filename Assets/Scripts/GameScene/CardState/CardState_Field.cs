using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardState_Field : CardState
{
    /// <summary>
    /// �v���C�ʒu(���݂̈ʒu�ł͂Ȃ����ߒ���)
    /// �ԈႢ�����Ȃ̂ŁAprivate�Ƃ���
    /// </summary>
    protected int Index { get;private set; }

    protected bool Special { get; private set; }
    public CardState_Field(IPlayable playable,int index,bool special) : base(playable)
    {
        //�v���C�ʒu
        Index = index;
        Special = special;
      //  if (!(playable is Status_Chara)) Debug.LogError("�L�����J�[�h�ȊO��CardState_Hand_Chara�X�e�[�g�ɑJ�ڂł��܂���");
    }




    public override bool Showing =>true;

    public override PosEnum Pos => PosEnum.Field;

    #region Dragging

    public int Target { get; set; }
    public bool AttackHero { get; set; }



    public override void OnBeginDrag(PointerEventData eventData)
    {
        //�U���\�łȂ��Ƃ��́A�h���b�O�ł��Ȃ�
        if (!IsPlayable) return;

        //���̕\��
        ArrowController.CreateArrow(IsPlayer,eventData, Trn);

        //TargetPos�̒l�́A�h���b�O�J�n����-1�Ƃ���
        //OnEndDrag�̎��_��-1�o�Ȃ������ꍇ�́A�U���Ώۂ����肵���Ƃ�������
        Target = (int)PlayableId.Default;

        //AttackHero�̒l�́A�h���b�O�J�n����false�Ƃ���
        //OnEndDrag�̎��_��true�������ꍇ�́A�U���Ώۂ�Hero�ƂȂ����Ƃ�������
        AttackHero = false;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        //�U���\�łȂ��Ƃ��́A�h���b�O�ł��Ȃ�
        if (!IsPlayable) return;
    }

    public override IEnumerator OnEndDrag(PointerEventData eventData)
    {
        //�U���\�łȂ��Ƃ��́A�h���b�O�ł��Ȃ�
        if (!IsPlayable) yield break;

        int attacker = Playable.PlayableCardId;
        //�U���Ώۂ��G�J�[�h�������ꍇ
        if (Target != (int)PlayableId.Default)
        {
            Command.CommandManager.I.Run(new Command.Command_Attack(IsPlayer, attacker, Target));
        }

        //�U���Ώۂ�Hero�������ꍇ
        if (AttackHero)
        {
            Command.CommandManager.I.Run(new Command.Command_Attack(IsPlayer, attacker));
        }

        //���[�U�[�̓��͂̒��O
        //TurnManager.I.StartJudge(false);
        ConnectionManager.Instance.Judge();
    }
    #endregion

}

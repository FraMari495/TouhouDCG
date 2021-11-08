using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using System;
using CardHandler;


/// <summary>
/// �J�[�h�ɑ΂��郆�[�U�[�̓��͂��󂯕t����@�\
/// </summary>
public abstract class CardInputHandler : MonoBehaviour, ICardViewInitializer, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private ISubject<Unit> _ShowExampleCard { get; } = new Subject<Unit>();
    public IObservable<Unit> O_ShowExampleCard => _ShowExampleCard;

    private string CardName { get; set; }
    protected IPlayable playable;

    /// <summary>
    /// �J�[�h�̈ʒu
    /// </summary>
    public PosEnum Pos => CurrentState.Pos;

    /// <summary>
    /// �J�[�h���\�������ۂ�
    /// </summary>
    public bool Showing => CurrentState.Showing;

    /// <summary>
    /// �v���C���[�̃J�[�h���ۂ�
    /// </summary>
    public bool IsPlayer { get; private set; }

    /// <summary>
    /// �X�e�[�g
    /// </summary>
    public CardState CurrentState { get; protected set; }





    public override string ToString() => CardName;

    /// <summary>
    /// �J�[�h�̏�����
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <param name="cardBook"></param>
    public virtual void Initialize(bool isPlayer, CardBook cardBook, bool deckMaking = false)
    {
        IsPlayer = isPlayer;
        CardName = cardBook.CardName;
        playable = this.GetComponent<IPlayable>();
        CurrentState = null;//new NullState(playable);
        Subscribing();
    }

    private void Subscribing()
    {
        //�J�[�h�̈ʒu�ω��Ɋւ���C�x���g���󂯎��
        playable.UpdatePosition.Subscribe(pos =>
        {

            if (pos.from == PosEnum.None)
            {
                switch (pos.to)
                {
                    case PosEnum.Field:
                        if (this is CardInputHandler_Chara chara) chara.ToFieldSpecial(pos.index);
                        else throw new Exception(this + "�̓L�����J�[�h�ł͂���܂���");
                        break;
                    case PosEnum.Hand:
                        ToHandSpecial();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                return;
            }
            else
            {

                switch (pos.to)
                {
                    case PosEnum.Deck:
                        ToDeck(pos.index);
                        break;
                    case PosEnum.Hand:
                        ToHand();
                        break;
                    case PosEnum.Field:
                        ToField(pos.index);
                        //else throw new Exception(this + "�̓L�����J�[�h�ł͂���܂���");
                        break;
                    case PosEnum.Discard:
                        ToDiscard(this is CardInputHandler_Chara);
                        break;
                    case PosEnum.Choicing:
                        ToChoicing();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        });



    }




    #region Event Handler
    //���݂̃X�e�[�g�ɉ����ċ@�\���ω�����
    public void OnBeginDrag(PointerEventData eventData)
    {
        CurrentState?.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        CurrentState?.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (CurrentState != null) StartCoroutine(CurrentState.OnEndDrag(eventData));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentState != null && Showing)
        {
            _ShowExampleCard.OnNext(Unit.Default);
            //CardExplanation.I.Initialize(cardVisual.GetExampleCard());
            //Debug.LogError("���[�ǂ̏ڍ�");
        }
    }

    #endregion

    #region �J�[�h�̑J��
    /// <summary>
    /// �J�[�h���f�b�L�Ɉړ�
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool ToDeck(int pos)
    {
        //   if (CurrentState != null) CurrentState.Exit();
        CurrentState = new CardState_Deck(playable, pos);
        return CurrentState.Enter();
    }

    /// <summary>
    /// �J�[�h����D�Ɉړ�
    /// </summary>
    /// <returns></returns>
    public abstract bool ToHand();

    /// <summary>
    /// �J�[�h����D�ɏ���
    /// </summary>
    /// <returns></returns>
    public bool ToHandSpecial()
    {
        if (this is CardInputHandler_Chara chara)
        {
            CurrentState = new CardState_Hand_Chara(playable, true);
        }
        else if (this is CardInputHandler_Spell spell)
        {
            CurrentState = new CardState_Hand_Spell(playable, true);
        }
        return CurrentState.Enter();
    }

    /// <summary>
    /// �J�[�h���t�B�[���h�Ɉړ�
    /// </summary>
    /// <returns></returns>
    public abstract bool ToField(int pos);

    /// <summary>
    /// �J�[�h���̂ĎD�Ɉړ�
    /// </summary>
    /// <returns></returns>
    public bool ToDiscard(bool defeated)
    {
        // if (CurrentState != null) CurrentState.Exit();
        CurrentState = new CardState_Discard(playable, defeated);
        return CurrentState.Enter();
    }

    private bool ToChoicing()
    {
        CurrentState = new CardState_Choicing(playable);
        return CurrentState.Enter();
    }
    #endregion
}


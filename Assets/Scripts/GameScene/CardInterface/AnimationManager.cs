using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public interface IAnimationMaker
{
    public Sequence AttackAnimation(StatusBase status);
    public Sequence LockOnAnimation(StatusBase attacker, StatusBase target);
    public Sequence DamageAnimation(int damage, int newHpValue, Transform trn, bool killer);
    public Sequence DeadAnimation(IPlayable playable, bool makeSound = true);
    public Sequence PlayAnimation(IPlayable playable, int pos, Transform center);
    public Sequence SpellAnimation(IPlayable playable, Transform center);
    public Sequence DrawAnimation(IPlayable playable);
    public Sequence SpecialSummonAnimation(IPlayable playable, int pos);
    public Sequence ToDeckAnimation(IPlayable playable);
    public Sequence SpecialSummonAnimation_Hand(IPlayable playable);
    public Sequence MoveToChoicingPanel(IPlayable playable);
    public Sequence TurnEndAnimation(bool show);
}



/// <summary>
/// �����ƃA�j���[�V�����𕪗����邱�Ƃ��ړI�̃N���X
/// queue�ɑ}�����ꂽSequence(�A�j���[�V����)���A���ԂɎ��s����
/// </summary>
public class AnimationManager : MonoSingleton<AnimationManager>
{
    /// <summary>
    /// �A�j���[�V�������}�������
    /// </summary>
    private Queue<(Func<Sequence>[] anim,string msg)> queue;

    /// <summary>
    /// queue�̒��g����������R���[�`��
    /// </summary>
    private Coroutine coroutine;

    /// <summary>
    /// �Q�[�����I�������^�C�~���O��true�ƂȂ�
    /// �Q�[�����I��������A����ȍ~queue�̒��g�����o���Ȃ�
    /// </summary>
    public bool GameOver { get; set; } = false;

    private void Awake()
    {
        Debug.Log(this.GetType());

        queue = new Queue<(Func<Sequence>[] anim, string msg)>();
        coroutine = StartCoroutine(SequencePlayer());
        Debug.Log(this.GetType()+"end");

    }

    private IEnumerator SequencePlayer()
    {
        //�Q�[�����I������܂ŁA�A�j���[�V����queue���m�F��������
        while (!GameOver)
        {
            //queue����ł͂Ȃ��ꍇ�A���o���ăA�j���[�V����������
            if(queue.Count>0)
            {
                //�����ɍĐ����ׂ��A�j���[�V�������Asequence�Ɍ�������
                Sequence sequence = DOTween.Sequence();

                //queue����A�j���[�V������Array������o���A���ׂĂ�(NonNull)�v�f������
                Array.ConvertAll(queue.Dequeue().anim, s => s()).NonNull().ForEach(s => sequence.Join(s));

                //�A�j���[�V�����̎��s
                yield return sequence.Play().WaitForCompletion();
            }

            //sequenceSet�����݂����queue�ɓ����
            if (sequenceSet?.Count > 0) queue.Enqueue(sequenceSet.Unite());

            yield return new WaitForSeconds(0.05f);
        }
    }

    public void AddSequence(Func<Sequence>[] sequence,string message)
    {
        if (sequenceSet?.Count > 0) queue.Enqueue(sequenceSet.Unite());
        sequenceSet = new Default();
        sequenceSet.Add(sequence,message);
    }

    public void AddSequence(Func<Sequence> sequence,string message)
    {
        AddSequence(new Func<Sequence>[1] { sequence }, message);
    }

    private SequenceSet sequenceSet;

    public void AddSequence<T>(Func<Sequence> sequence,string message) where T : SequenceSet, new()
    {

        if (sequenceSet is null) sequenceSet = new T();

        else if (!(sequenceSet is T))
        {
            if(sequenceSet.Count > 0) queue.Enqueue(sequenceSet.Unite());
            sequenceSet = new T();
        }

        sequenceSet.Add(sequence,message);

    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        StopCoroutine(coroutine);
    }


    public IAnimationMaker AnimationMaker { get; set; }  


    #region SequenceSet Class
    public abstract class SequenceSet
    {
        private List<Func<Sequence>> sequences = new List<Func<Sequence>>();
        public int Count => sequences.Count;
        private string Message { get; set; }

        public (Func<Sequence>[],string message) Unite()
        {
            var ans = sequences.ToArray();
            sequences = new List<Func<Sequence>>();
            return (ans, Message + "���Đ�");
        }

        public void Add(Func<Sequence> sequence,string message)
        {
            Message = message;
            sequences.Add(sequence);
        }
        public void Add(Func<Sequence>[] sequence,string message)
        {
            Message=message;
            sequences.AddRange(sequence.ToList());
        }
    }
    public class Damage : SequenceSet { }
    public class Defeated : SequenceSet { }
    public class SpecialSummon : SequenceSet { }
    public class ToDeck : SequenceSet { }
    private class Default : SequenceSet { }
    public class ToHand : SequenceSet { }
    #endregion

}



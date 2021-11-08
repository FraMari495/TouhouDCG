using System;
using UnityEngine;

public abstract class StatusParameter
{
    /// <summary>
    /// �X�e�[�^�X�̎��
    /// </summary>
    public abstract Parameter Parameter { get; }
    public StatusParameter( int initValue, int maxValue)
    {
        value = new RangeInt(initValue, maxValue);
    }

    /// <summary>
    /// �ő�l
    /// </summary>
    public int Max => value.Max;

    /// <summary>
    /// �X�e�[�^�X
    /// </summary>
    protected RangeInt value;

    /// <summary>
    /// �X�e�[�^�X�̒l
    /// </summary>
    public virtual int Value { get => value.Value; protected set => this.value.Value = value; }

    /// <summary>
    /// �\������Ƃ��́A�C���X�^���X�ł͂Ȃ��l��\������
    /// </summary>
    /// <param name="param"></param>
    public static implicit operator int(StatusParameter param) => param.Value;

    public override string ToString()=> value.ToString();

    /// <summary>
    /// �ő�l��ύX
    /// </summary>
    /// <param name="delta"></param>
    public virtual void ChangeMax(int delta)=> value.Max += delta;

    /// <summary>
    /// ��(�ő�l���I�[�o�[���Ȃ�)
    /// </summary>
    /// <param name="delta"></param>
    public void Heal(int delta)=> Value += delta;

    /// <summary>
    /// �ǉ�(�ő�l���I�[�o�[�����ہA���̕��ő�l�𑝉�)
    /// </summary>
    /// <param name="delta"></param>
    public void Add(int delta)
    {
        //�f�o�b�O
        if (delta < 0) Debug.LogError("���������郁�\�b�h�ł͂���܂���");

        //�l��ǉ����Ă݂āA�A�A
        int temp = Value + delta;

        //�ő�l���I�[�o�[�����Ƃ��́A���̕��ő�l�𑝉�������
        if (temp > value.Max) ChangeMax(temp);

        //�l���X�V
        Value = temp;
    }

    /// <summary>
    /// �ቺ(�������A�ő�l������)
    /// </summary>
    /// <param name="delta"></param>
    public void Down(int delta)
    {
        //�f�o�b�O
        if (delta < 0) Debug.LogError("�㏸�����郁�\�b�h�ł͂���܂���");

        //�l��ύX
        Damage(delta);

        //�ő�l��ύX
        value.Max -= delta;  
    }



    public void Initialize(int initHp)=> Value = initHp;

    public void Damage(int damage)
    {
        //�f�o�b�O
        if (damage < 0) Debug.LogError("�񕜂����郁�\�b�h�ł͂���܂���");

        //�l��ύX
        Value -= damage;
    }
}


public class Cost : StatusParameter
{
    public Cost(int initValue, int maxValue = (int)1e8) : base(initValue, maxValue) { }

    public override Parameter Parameter => Parameter.Cost;
}

public class Hp : StatusParameter
{
    public Hp( int initValue, int maxValue,Action onDead) : base(initValue, maxValue)
    {
        OnDead = onDead;
    }
    public override Parameter Parameter => Parameter.Hp;
    public Action OnDead { get; set; }


    public override int Value
    {
        get => value.Value;
        protected set
        {
            int previousHp = this.value.Value;
            this.value.Value = value;

            int hpChange = this.value.Value - previousHp;

            if (this.value.Value == 0) OnDead();
        }
    }
    public override void ChangeMax(int delta)
    {
        base.ChangeMax(delta);
        if (value.Value == 0) OnDead();
    }
}

public class Atk : StatusParameter
{
    public Atk(int initValue, int maxValue = (int)1e8) : base(initValue, maxValue) { }
    public override Parameter Parameter => Parameter.Atk;

}
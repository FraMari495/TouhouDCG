using System;
using UnityEngine;

public abstract class StatusParameter
{
    /// <summary>
    /// ステータスの種類
    /// </summary>
    public abstract Parameter Parameter { get; }
    public StatusParameter( int initValue, int maxValue)
    {
        value = new RangeInt(initValue, maxValue);
    }

    /// <summary>
    /// 最大値
    /// </summary>
    public int Max => value.Max;

    /// <summary>
    /// ステータス
    /// </summary>
    protected RangeInt value;

    /// <summary>
    /// ステータスの値
    /// </summary>
    public virtual int Value { get => value.Value; protected set => this.value.Value = value; }

    /// <summary>
    /// 表示するときは、インスタンスではなく値を表示する
    /// </summary>
    /// <param name="param"></param>
    public static implicit operator int(StatusParameter param) => param.Value;

    public override string ToString()=> value.ToString();

    /// <summary>
    /// 最大値を変更
    /// </summary>
    /// <param name="delta"></param>
    public virtual void ChangeMax(int delta)=> value.Max += delta;

    /// <summary>
    /// 回復(最大値をオーバーしない)
    /// </summary>
    /// <param name="delta"></param>
    public void Heal(int delta)=> Value += delta;

    /// <summary>
    /// 追加(最大値をオーバーした際、その分最大値を増加)
    /// </summary>
    /// <param name="delta"></param>
    public void Add(int delta)
    {
        //デバッグ
        if (delta < 0) Debug.LogError("減少させるメソッドではありません");

        //値を追加してみて、、、
        int temp = Value + delta;

        //最大値をオーバーしたときは、その分最大値を増加させる
        if (temp > value.Max) ChangeMax(temp);

        //値を更新
        Value = temp;
    }

    /// <summary>
    /// 低下(減少分、最大値を減少)
    /// </summary>
    /// <param name="delta"></param>
    public void Down(int delta)
    {
        //デバッグ
        if (delta < 0) Debug.LogError("上昇させるメソッドではありません");

        //値を変更
        Damage(delta);

        //最大値を変更
        value.Max -= delta;  
    }



    public void Initialize(int initHp)=> Value = initHp;

    public void Damage(int damage)
    {
        //デバッグ
        if (damage < 0) Debug.LogError("回復させるメソッドではありません");

        //値を変更
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 最大値と最小値をもつ整数
/// </summary>
public class RangeInt
{
    /// <summary>
    /// 現在の数値
    /// </summary>
    private int val;

    /// <summary>
    /// 最大値
    /// </summary>
    private int max;

    /// <summary>
    /// 最小値
    /// </summary>
    private int min;

    public RangeInt(int val, int max = (int)1e8, int min = 0)
    {
        if (max < min)
        {
            Debug.LogError("最大値は最小値以上に設定してください");
        }

        this.val = val;
        this.max = max;
        this.min = min;
    }

    /// <summary>
    /// 最大値
    /// </summary>
    public int Max
    {
        get => max;
        set
        {
            max = value;
            if (max < min)
            {
                Debug.LogError("最大値が最小値未満になりました");
                max = min;
            }
            if (Value > max)
            {
                Value = max;
            }
        }
    }

    /// <summary>
    /// 現在の値
    /// </summary>
    public int Value
    {
        get => val;
        set
        {
            if (value > Max)
            {
                //最大値を超える場合は最大値にする
                val = Max;
            }
            else if (value < min)
            {
                //最小値を下回る場合は最小値にする
                val = min;
            }
            else
            {
                val = value;
            }
        }
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

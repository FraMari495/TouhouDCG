using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ő�l�ƍŏ��l��������
/// </summary>
public class RangeInt
{
    /// <summary>
    /// ���݂̐��l
    /// </summary>
    private int val;

    /// <summary>
    /// �ő�l
    /// </summary>
    private int max;

    /// <summary>
    /// �ŏ��l
    /// </summary>
    private int min;

    public RangeInt(int val, int max = (int)1e8, int min = 0)
    {
        if (max < min)
        {
            Debug.LogError("�ő�l�͍ŏ��l�ȏ�ɐݒ肵�Ă�������");
        }

        this.val = val;
        this.max = max;
        this.min = min;
    }

    /// <summary>
    /// �ő�l
    /// </summary>
    public int Max
    {
        get => max;
        set
        {
            max = value;
            if (max < min)
            {
                Debug.LogError("�ő�l���ŏ��l�����ɂȂ�܂���");
                max = min;
            }
            if (Value > max)
            {
                Value = max;
            }
        }
    }

    /// <summary>
    /// ���݂̒l
    /// </summary>
    public int Value
    {
        get => val;
        set
        {
            if (value > Max)
            {
                //�ő�l�𒴂���ꍇ�͍ő�l�ɂ���
                val = Max;
            }
            else if (value < min)
            {
                //�ŏ��l�������ꍇ�͍ŏ��l�ɂ���
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

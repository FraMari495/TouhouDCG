using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// �}�i(�J�[�h�̃R�X�g���x����)���R���g���[������N���X
/// </summary>
public class Mana
{
    private TextMeshProUGUI ManaText { get; }

    public Mana(TextMeshProUGUI manaText)
    {
        ManaText = manaText;

        //�Q�[���J�n���Ɏ����Ă���}�i��0
        RemainMana = 0;
    }

    //�}�i�̓^�[�����Ƃɑ����Ă������AlimitMana�𒴂��邱�Ƃ͂Ȃ�
    private int limitMana = 10;

    //�u�^�[���J�n���́v�}�i
    private int initialMana = 0;

    //�g�p�\�ȃ}�i
    private int remainMana = 0;

    /// <summary>
    /// �c��̃}�i
    /// </summary>
    public int RemainMana
    {
        get => remainMana;
        private set
        {
            //�f�o�b�O
            if (value < 0) Debug.LogError("�c��̃}�i�����ɂȂ�܂���");

            //�}�i���X�V
            remainMana = value;

            //�\���̕ύX
            ManaText.text = value.ToString();
        }
    }

    /// <summary>
    /// �}�i�������
    /// ����ł�����true�A����ł��Ȃ�������false;��Ԃ�
    /// </summary>
    /// <param name="cost">������</param>
    /// <returns></returns>
    public bool UseMana(int cost)
    {
        //�R�X�g���g�p�����ꍇ�̎c����v�Z���Ă݂�
        int temp = RemainMana - cost;

        //�c�肪0�ȏ�Ȃ�X�V
        if (temp >= 0) RemainMana = temp;

        //�R�X�g���g�p�ł������ۂ���Ԃ�
        return temp >= 0;
    }

    /// <summary>
    /// �^�[���J�n���Ɏg�p�\�ȃ}�i�����Z�b�g����
    /// </summary>
    public void NewTurn()
    {
        //�����}�i�𑝂₷(limitMana�𒴂��Ȃ�)
        initialMana = Mathf.Min(initialMana + 1, limitMana);

        //�g�p�\�ȃ}�i�����Z�b�g����
        RemainMana = initialMana;
    }

}

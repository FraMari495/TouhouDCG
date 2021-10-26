using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// マナ(カードのコストを支払う)をコントロールするクラス
/// </summary>
public class Mana
{
    private TextMeshProUGUI ManaText { get; }

    public Mana(TextMeshProUGUI manaText)
    {
        ManaText = manaText;

        //ゲーム開始時に持っているマナは0
        RemainMana = 0;
    }

    //マナはターンごとに増えていくが、limitManaを超えることはない
    private int limitMana = 10;

    //「ターン開始時の」マナ
    private int initialMana = 0;

    //使用可能なマナ
    private int remainMana = 0;

    /// <summary>
    /// 残りのマナ
    /// </summary>
    public int RemainMana
    {
        get => remainMana;
        private set
        {
            //デバッグ
            if (value < 0) Debug.LogError("残りのマナが負になりました");

            //マナを更新
            remainMana = value;

            //表示の変更
            ManaText.text = value.ToString();
        }
    }

    /// <summary>
    /// マナを消費する
    /// 消費できたらtrue、消費できなかったらfalse;を返す
    /// </summary>
    /// <param name="cost">消費する量</param>
    /// <returns></returns>
    public bool UseMana(int cost)
    {
        //コストを使用した場合の残りを計算してみる
        int temp = RemainMana - cost;

        //残りが0以上なら更新
        if (temp >= 0) RemainMana = temp;

        //コストが使用できたか否かを返す
        return temp >= 0;
    }

    /// <summary>
    /// ターン開始時に使用可能なマナをリセットする
    /// </summary>
    public void NewTurn()
    {
        //初期マナを増やす(limitManaを超えない)
        initialMana = Mathf.Min(initialMana + 1, limitMana);

        //使用可能なマナをリセットする
        RemainMana = initialMana;
    }

}

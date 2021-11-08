using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// ヒーローのステータス
/// カードは見た目とデータでクラスを分けたが、ヒーローの場合は同じクラス内で処理
/// </summary>
public class Status_Hero : Status
{
    /// <summary>
    /// シングルトンパターンを、インスタンスが2つの場合に拡張
    /// </summary>
    #region MonoPair

    private static Dictionary<bool, Status_Hero> iMap;

    public static Status_Hero I(bool isPlayer)
    {
        if (iMap == null)
        {
            Status_Hero[] i = FindObjectsOfType<Status_Hero>();
            if (i.Length == 2)
            {
                iMap = new Dictionary<bool, Status_Hero>()
                { { i[0].IsPlayer, i[0] }, { i[1].IsPlayer, i[1] } };
            }
            else
            {
                Debug.LogError(typeof(Status_Hero).ToString() + "のインスタンスが" + i.Length + "個存在します。2個に修正してください");
                return null;
            }
        }
        return iMap[isPlayer];

       
    }

    private void OnDestroy()
    {
        iMap = null;
    }
    #endregion


    public Subject<bool> HeroDead { get; } = new Subject<bool>();


    [SerializeField]private Image charaImage;

    [SerializeField] private Sprite charaSprite1;
    [SerializeField] private Sprite charaSprite2;


   // [SerializeField] private SceneInitializer sceneInitializer;




    [SerializeField] private TextMeshProUGUI hpText_field;
    public override CardType Type => CardType.Hero;

    public string CardName { get; private set; }

    protected override string ObjectName => CardName;

    public Data_Hero HeroData => (Data_Hero)CardData;

    public void Initialize(bool isPlayer)
    {
        IsPlayer = isPlayer;

        if (ReactivePropertyList.I.FirstAttack)
        {
            charaImage.sprite = IsPlayer? charaSprite1:charaSprite2;
        }
        else
        {
            charaImage.sprite = IsPlayer ? charaSprite2 : charaSprite1;
        }

        int maxHp = ReactivePropertyList.I.Tutorial ? 4 : 20;
        CardData = new Data_Hero(IsPlayer,this.transform, maxHp, maxHp, new TextMeshProUGUI[1] { hpText_field }, Dead);
        UpdateHpUI.OnNext(maxHp);
        CardName = IsPlayer?"player_hero":"rival_hero";

    }

    /// <summary>
    /// ヒーローが死亡した際に行われる処理
    /// 主にアニメーションの作成を行う
    /// </summary>
    protected override void Dead()
    {
        HeroDead.OnNext(IsPlayer);
    }




    public class Data_Hero : Data
    {
        public Data_Hero(bool isPlayer,Transform trn,int currentHp, int maxHp, TextMeshProUGUI[] hpTexts, Action onDead) : base(isPlayer,trn,currentHp, maxHp, onDead)
        {
        }
    }
}

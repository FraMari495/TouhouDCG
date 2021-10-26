using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カード図鑑(共通部分)
/// </summary>
public abstract class CardBook : ScriptableObject
{
    #region 図鑑に載せたい項目(共通部分)
    [SerializeField] private int id;
    [SerializeField] private string cardName;
    [SerializeField] private Sprite cardImage;
    [SerializeField,TextArea(3,7)] private string description;
    [SerializeField] private int cost;
    [SerializeField] private OnPlayAbility onPlayAbility;
    [SerializeField] private float imageSizeRatio = 1;

    public int Id { get => id;}
    public string CardName { get => cardName; }
    public Sprite CardImage { get => cardImage; }
    public string Description { get => description; }
    public int Cost { get => cost;}
    public OnPlayAbility OnPlaySkill => onPlayAbility;
    public float ImageSizeRatio => imageSizeRatio;
    #endregion

    /// <summary>
    /// プレハブへのパス
    /// </summary>
    protected abstract string PrefabPath { get; }

    /// <summary>
    /// カードのオブジェクトを生成する
    /// 生成したカードのStatusを返す
    /// </summary>
    /// <param name="isPlayer">プレイヤーのカードか否か</param>
    private IPlayable MakeCard(bool isPlayer, int? playableId = null)
    {

        //カードのプレハブをインスタンス化
        //カードの種類によって、用いるプレハブが異なる
        GameObject cardObj = Instantiate(Resources.Load<GameObject>(PrefabPath), GameObject.Find("Canvas").transform, false);
        cardObj.GetComponentInChildren<CardForDeckMaking>().gameObject.SetActive(false);

        //様々な初期化
        cardObj.GetComponentsInChildren<ICardViewInitializer>().ForEach(init => init.Initialize(isPlayer, this));

        //PlayableIdを設定 (カードのインスタンスごとに異なるIdを与える。　オンライン対戦のとき、"同じインスタンス"には同じIdが設定される)
        IPlayable playable = cardObj.GetComponent<IPlayable>();
        playable.RequirePlayableId(playableId);

        //分かりやすいように、オブジェクト名としてカードの名前を用いる
        cardObj.name = CardName;

        return playable;
    }




    /// <summary>
    /// カードのオブジェクトを生成する
    /// 生成したカードのStatusを返す
    /// </summary>
    /// <param name="isPlayer">プレイヤーのカードか否か</param>
    private IPlayable MakeCard()
    {

        //カードのプレハブをインスタンス化
        //カードの種類によって、用いるプレハブが異なる
        GameObject cardObj = Instantiate(Resources.Load<GameObject>(PrefabPath), GameObject.Find("Canvas").transform, false);
        cardObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
        Destroy(cardObj.GetComponent<StatusBase>());

        //様々な初期化
        cardObj.GetComponent<CardInputHandler>().Initialize(true, this);
        cardObj.GetComponent<CardVisualController>().Initialize(true,this);
        //cardObj.GetComponentsInChildren<ICardViewInitializer>().ForEach(init => init.Initialize(true, this));

        //PlayableIdを設定 (カードのインスタンスごとに異なるIdを与える。　オンライン対戦のとき、"同じインスタンス"には同じIdが設定される)
        IPlayable playable = cardObj.GetComponent<IPlayable>();
        // playable.RequirePlayableId(-1);

        //分かりやすいように、オブジェクト名としてカードの名前を用いる
        cardObj.name = CardName;

        return playable;
    }



    #region カードを作成し、position(Deck、Hand、Field、Discard)に渡す  (public methodsh)


    public CardForDeckMaking MakeCardForDeckMaker()
    {
        //カードのプレハブをインスタンス化
        //カードの種類によって、用いるプレハブが異なる
        //GameObject cardObj = Instantiate(Resources.Load<GameObject>(PrefabPath), GameObject.Find("Canvas").transform, false);
        GameObject cardObj =MakeCard().GameObject;
        CardForDeckMaking card = cardObj.GetComponentInChildren<CardForDeckMaking>();
        cardObj.name = CardName;
        return card;
    }

    public IPlayable MakeCardToDeck(bool isPlayer,int pos,int? playableId =null)
    {
        return MakeCard(isPlayer, playableId);
        //c.GameObject.GetComponent<CardInputHandler>().ToDeck(pos);
        //return c;
    }

    public IPlayable MakeCardToField(bool isPlayer)
    {

       return MakeCard(isPlayer);

        //return c.GameObject.GetComponent<CardInputHandler_Chara>().ToField(pos);
    }

    public IPlayable MakeCardToHand(bool isPlayer)
    {
        return MakeCard(isPlayer);
    }

    public IPlayable MakeCardToDiscard(bool isPlayer)
    {
        return MakeCard(isPlayer);
    }
    #endregion
}

/// <summary>
/// 初期化が必要な(カードの)クラスが実装するインターフェース
/// </summary>
public interface ICardViewInitializer
{
    void Initialize(bool isPlayer, CardBook book);
}

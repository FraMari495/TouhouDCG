//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public enum PosEnum
//{
//    Deck = 0,
//    Hand = 1,
//    Field = 2,
//    Discard = 3
//}

//public enum CardType2
//{
//    Chara,
//    Spell,
//    Hero
//}

//public abstract class CardMovement : MonoBehaviour
//{
//    public abstract CardType CardType { get; }
//    private Dictionary<PosEnum, GameObject> viewMap = new Dictionary<PosEnum, GameObject>();

//    public bool IsPlayer { get; private set; }
//    public string CardName { get; private set; }

//    private void Awake()
//    {
//        //xxView と ポジションを対応付けたい
//        viewMap.Add(PosEnum.Deck, this.transform.Find("_DeckView").gameObject);
//        viewMap.Add(PosEnum.Hand, this.transform.Find("_HandView").gameObject);
//        viewMap.Add(PosEnum.Field, this.transform.Find("_FieldView").gameObject);
//        viewMap.Add(PosEnum.Discard, this.transform.Find("_DiscardView").gameObject);
//    }

//    /// <summary>
//    /// カードの移動が起こったときに呼ぶ
//    /// カードの見た目を移動先に対応させる
//    /// </summary>
//    /// <param name="pos">移動先</param>
//    public void ChangePos(PosEnum pos)
//    {
//        foreach (var pos_obj in viewMap)
//        {
//            //posに対応したオブジェクトのみ表示
//            pos_obj.Value.SetActive(pos_obj.Key == pos);
//        }
//    }

//    public override string ToString()
//    {
//        return CardName;
//    }

//    public virtual void Initialize(bool isPlayer, CardBook cardBook)
//    {
//        IsPlayer = isPlayer;
//        CardName = cardBook.CardName;

//        //cost = new Cost(new Text[1] { costText }, cardBook.Cost, (int)1e8);

//        //onPlaySkill = cardBook.OnPlaySkill;
//    }

//    //private OnPlaySkill onPlaySkill;

//    //private StatusParameter cost;
//    //public int Cost => cost;

//    //public IEnumerator RunOnPlaySkill( )
//    //{
//    //    if (onPlaySkill != null)
//    //    {
//    //        yield return onPlaySkill.Run(this);
//    //    }
//    //}
//}




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayableIdを管理する
/// 
/// 通信対戦の時に本領を発揮する
/// カードのインスタンスとIdが1対1対応している
/// 通信相手の盤面と自分の盤面を見比べたとき、同じカードには同じIdが割り振られるように処理を行う
/// 
/// 自分の端末では自分のカードのIdを"生成"
/// 対戦相手のIdは通信により"受け取る"
/// </summary>
public class PlayableIdManager : MonoSingleton<PlayableIdManager>
{
    private int nextPlayerId = 1;
    private int nextRivalId = -1;

    /// <summary>
    /// PlayableIdとカードを紐づける辞書
    /// </summary>
    Dictionary<int, IPlayable> playableMap = new Dictionary<int, IPlayable>();

    /// <summary>
    /// 通信相手の盤面と自分の盤面を見比べたとき、同じカードには同じIdが割り振られるように処理を行う
    /// 
    /// 自分の端末では自分のカードのIdを"生成"
    /// 対戦相手のIdは通信により"受け取る"
    /// </summary>
    /// <param name="playable">PlayableIdが必要なカード</param>
    /// <param name="playableId">対戦相手のカードにIdを割り振る際にnon null</param>
    /// <returns></returns>
    public PlayableId GetId(IPlayable playable,bool firstAttack,int? playableId)
    {
        sbyte modif = (sbyte)(firstAttack ? 1 : -1);

        //playableIdがnullでない = 通信相手のカードのIdの情報を受け取った
        if (playableId is int id)
        {
            //受け取った情報を、素直に辞書に追加
            playableMap.Add(id, playable);

            //インクリメント
            if (playable.IsPlayer) nextPlayerId++;
            else nextRivalId--;

            return new PlayableId(id);
        }

        //playableIdがnull かつ 自分のカード = Idを割り振る必要がある
        if (playable.IsPlayer)
        {
            //PlayableIdは、nextPlayerId(1,2,3,...)を用いて生成
            //後攻なら-1を掛けることでつじつまを合わせる
            PlayableId ans = new PlayableId(nextPlayerId * modif);

            //辞書に登録
            playableMap.Add((int)ans, playable);

            //インクリメント
            nextPlayerId++;
            return ans;
        }

        //playableIdがnull かつ 相手のカード = オフライン対戦で、相手のPlayableIdを割り振る
        else
        {
            //PlayableIdは、nextRivalId(-1,-2,-3,...)を用いて生成
            //後攻なら-1を掛けることでつじつまを合わせる
            PlayableId ans = new PlayableId(nextRivalId * modif);

            //辞書に登録
            playableMap.Add((int)ans, playable);

            //インクリメント
            nextRivalId--;
            return ans;
        }

    }

    /// <summary>
    /// Idからカードを検索
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IPlayable GetPlayableById(int id)
    {
        if (playableMap.ContainsKey(id))
        {
            return playableMap[id];
        }
        else
        {
            Debug.LogError($"id{id}のカードは存在しません");
            Debug.LogError($"存在するのは{string.Join(",", playableMap.Keys)}です");
            return null;
        }
    }
}


/// <summary>
/// プリミティブをラップするクラス
/// カードのインスタンスId
/// </summary>
[System.Serializable]
public class PlayableId
{
    public PlayableId(int id)
    {
        this.id = id;
    }

    /// <summary>
    /// デフォルト値(id = 0)
    /// ※ id = 0が割り振られるカードは存在しない
    /// </summary>
    public static PlayableId Default => new PlayableId(0);


    [SerializeField]private int id;
    private int Id => id;


    #region override operator など (==演算やEqualに関して、インスタンスが異なっていてもidが同じならtrueとなるように変更)
    public static explicit operator int(PlayableId playableId) => playableId.Id;
    public static bool operator ==(PlayableId a, PlayableId b) => a.Id == b.Id;
    public static bool operator !=(PlayableId a, PlayableId b) => a.Id != b.Id;
    public override bool Equals(object obj)=> (obj is PlayableId id) && Id == id.Id;
    public override int GetHashCode()=> base.GetHashCode();
    public override string ToString()=> Id.ToString();
    #endregion
}
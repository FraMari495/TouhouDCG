using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

/// <summary>
/// 状態異常
/// </summary>
public enum StatusEffect
{
    Dead = 0,
    Freeze = 1,
    Sealed=2,
}

public class Status_Chara : Status, ICardViewInitializer, IPlayable
{
    /// <summary>
    /// カードの初期化
    /// </summary>
    /// <param name="isPlayer">プレイヤーのカードか否か</param>
    /// <param name="cardBook">カードのデータベース</param>
    public void Initialize(bool isPlayer, CardBook cardBook)
    {
        if (cardBook is CardBook_Chara charaBook)
        {
            CardBook = cardBook;
            IsPlayer = isPlayer;
            CardData = new Data_Chara(isPlayer, this.transform, charaBook, Dead);
            CardName = charaBook.CardName;
        }
        else
        {
            Debug.LogError("キャラカードのCardBookが必要です");
        }
    }

    #region UniRx.Subject

    /// <summary>
    /// コストの表示を変更するように通知を送る
    /// </summary>
    public Subject<int> UpdateCostUI { get; } = new Subject<int>();

    /// <summary>
    /// Hpの表示を変更するように通知を送る
    /// </summary>
    public Subject<int> UpdateAtkUI { get; } = new Subject<int>();

    /// <summary>
    /// 位置が変更されたことを通知(アニメーションを起こさせる)
    /// </summary>
    public Subject<(PosEnum from, PosEnum to, int index)> UpdatePosition { get; } = new Subject<(PosEnum from, PosEnum to, int index)>();

    public Subject<int> UpdateBombUI { get; } = new Subject<int>();
    #endregion


    #region カードのステータス public get

    /// <summary>
    /// カードのオブジェクト
    /// </summary>
    public GameObject GameObject => this.gameObject;


    /// <summary>
    /// 攻撃力やHpなど、様々な状態を示す変数が含まれている
    /// </summary>
    public Data_Chara CharaData => (Data_Chara)CardData;

    /// <summary>
    /// カードの名前
    /// </summary>
    public string CardName { get; private set; }

    /// <summary>
    /// カードのインスタンスと1対1に紐づくId
    /// </summary>
    public PlayableId PlayableId => CharaData.PlayableId;

    /// <summary>
    /// カードの種類と1対1に紐づくId
    /// </summary>
    public int CardBookId => CharaData.CardbookId;

    /// <summary>
    /// プレイ時に発動するアビリティー
    /// </summary>
    public OnPlayAbility OnPlayAbility => CharaData.OnPlaySkill;

    /// <summary>
    /// カードのデータベース
    /// </summary>
    public CardBook CardBook { get; private set; }

    /// <summary>
    /// カードの種類(キャラカードなので Charaを返す)
    /// </summary>
    public override CardType Type => CardType.Chara;
    protected override string ObjectName => CardName;

    /// <summary>
    /// 攻撃力
    /// </summary>
    public int Atk => CharaData.AtkData;

    /// <summary>
    /// プレイ可能か否か
    /// </summary>
    public bool IsPlayable{ get => CharaData.playable; set => CharaData.playable = value; }

    #endregion


    #region public methods
    /// <summary>
    /// カードのインスタンスIdを取得する
    /// </summary>
    /// <param name="playableId"></param>
    public void RequirePlayableId(int? playableId)
    {
        if (PlayableId != PlayableId.Default)
        {
            Debug.LogError("既にIDが設定されています");
            return;
        }
        CharaData.PlayableId = PlayableIdManager.I.GetId(this, playableId);
    }
    /// <summary>
    /// (通常)ターン開始時に呼ばれる
    /// 攻撃可能回数をリセットする
    /// </summary>
    public void ResetAttackNum()=> CharaData.AttackNum = RemoveStatusEfect(StatusEffect.Freeze) ? 0 : CharaData.initialAttackNum;

    /// <summary>
    /// ターン終了時のスキルを発動
    /// </summary>
    public void RunOnTurnEndSkill()=> CharaData.MyAbilities.OnTurnEndSkill?.Run(this,null);

    /// <summary>
    /// 死亡時のスキルを発動
    /// </summary>
    public void RunOnDefeatedSkill() => CharaData.MyAbilities.OnDefeatedSkill?.Run(this,null);

    /// <summary>
    /// 攻撃力の減少
    /// </summary>
    /// <param name="delta"></param>
    public void DownAtk(int delta)
    {
        CharaData.AtkData.Down(delta);
        AnimationManager.I.AddSequence(() => AnimationMaker.AtkDownAnimation(delta, Atk, this),"攻撃力減少");
    }

    /// <summary>
    /// 攻撃力の増加
    /// </summary>
    /// <param name="delta"></param>
    public void AddAtk(int delta)
    {
        CharaData.AtkData.Add(delta);
        AnimationManager.I.AddSequence(() => DOTween.Sequence().AppendCallback(()=> UpdateAtkUI.OnNext(CharaData.AtkData)), "攻撃力増加");
    }

    /// <summary>
    /// コストを取得
    /// </summary>
    /// <returns></returns>
    public int GetCost() => CharaData.Cost;

    /// <summary>
    /// 状態異常を付与
    /// </summary>
    /// <param name="effect"></param>
    public void AddEffect(StatusEffect effect)
    {
        CharaData.AddStatusEffect(effect);
        UpdateEffect.OnNext(effect);
        UpdateBombUI.OnNext(CharaData.MyAbilities.BombCost);
    }

    public int[] AddBomb(int addNum,int[] indices)
    {
        if (CharaData.MyAbilities.BombCost > 0)
        {
            bool run = CharaData.MyAbilities.AddBombCost(addNum);
            AnimationManager.I.AddSequence(() => DOTween.Sequence().AppendCallback(() => UpdateBombUI.OnNext(CharaData.MyAbilities.BombCost)), "ボム増加");

            if (run)
            {
                return CharaData.MyAbilities.BombAbility.Run(this, indices);
            }
            else
            {
                return null;
            }
        }

        return null;
    }

    #endregion

    protected override void Dead()=> AddEffect(StatusEffect.Dead);


    #region classes

    /// <summary>
    /// キャラカードの状態
    /// </summary>
    public class Data_Chara:Data
    {
        /// <summary>
        /// 攻撃力
        /// </summary>
        public Atk AtkData { get; set; }

        /// <summary>
        /// コスト
        /// </summary>
        public Cost Cost { get; private set; }



        /// <summary>
        /// カードのインスタンスのId
        /// (通信対戦時にカードを指定する際に用いる)
        /// </summary>
        public PlayableId PlayableId { get; set; } = PlayableId.Default;

        /// <summary>
        /// 現在発動している特殊能力
        /// </summary>
        private RunningAbilities myAbilities;

        /// <summary>
        /// 封印されていなければ myAbilities を返し、封印されている場合は空のRunningAbilitiesを返す
        /// </summary>
        public RunningAbilities MyAbilities => StatusEffects.Contains(StatusEffect.Sealed) ? new RunningAbilities(null) : myAbilities;

        /// <summary>
        /// 1ターンに何回攻撃できるか (通常は1回)
        /// </summary>
        public int initialAttackNum = 1;

        /// <summary>
        /// 残りの攻撃可能回数
        /// </summary>
        public int AttackNum { get; set; }

        /// <summary>
        /// 行動可能か否か
        /// </summary>
        public bool playable { get; set; }
        public int CardbookId { get; }

        public Race Race { get; }

        public Data_Chara(bool isPlayer,Transform trn,CardBook_Chara charaBook,Action onDead)
            :base(isPlayer, trn, charaBook.Hp,charaBook.Hp, onDead)
        {
            CardbookId = charaBook.Id; //DB Id 
            AtkData = new Atk(charaBook.Atk);//攻撃力の初期化
            Cost = new Cost(charaBook.Cost);//コストの初期化
            this.myAbilities = new RunningAbilities(charaBook);//特殊能力の初期化
            this.initialAttackNum = 1;//攻撃可能回数は1(後々複数攻撃可能なカードを作るときは変更)
            AttackNum = 0;//召喚したターンは攻撃不能
            OnPlaySkill = charaBook.OnPlaySkill;//プレイ時スキル(封印の影響を受けないので、このクラス内で定義)
            Race = charaBook.Race;
        }

        /// <summary>
        /// AIが使用するコンストラクタ
        /// ディープコピー
        /// </summary>
        /// <param name="charaData"></param>
        private Data_Chara(Data_Chara charaData):base(charaData.IsPlayer,null,charaData.HpData,charaData.HpData.Max,null)
        {
            CardbookId = charaData.CardbookId;
            AtkData = new Atk( charaData.AtkData);
            Cost = new Cost(charaData.Cost);
            this.myAbilities = RunningAbilities.AbiliyiesForAI(charaData.MyAbilities);
            this.initialAttackNum = charaData.initialAttackNum;
            AttackNum = 0;
            //   this.playable = playable;
            OnPlaySkill = charaData.OnPlaySkill;
            playable = charaData.playable;
            PlayableId = charaData.PlayableId;
            HpData.OnDead = () => StatusEffects.Add(StatusEffect.Dead);
            Race = charaData.Race;
        }

        /// <summary>
        /// プレイ時スキル
        /// これは封印の影響を受けないためこのクラスに記述
        /// </summary>
        public OnPlayAbility OnPlaySkill { get; private set; }

        /// <summary>
        /// AIの思考に使用
        /// charaDataをディープコピー(もっといい方法は無いのか)
        /// </summary>
        /// <param name="charaData"></param>
        /// <returns></returns>
        public static Data_Chara DataCharaForAI(Data_Chara charaData)
        {
            return new Data_Chara(charaData);
        }

        /// <summary>
        /// AIの思考に使用
        /// カードの評価を返す
        /// </summary>
        /// <returns></returns>
        public float GetScore()
        {
            float ans = AtkData*3;
            ans += RunningAbilities.GetAbilityScore(this);
            return ans;
        }
    }

    /// <summary>
    /// キャラカードが発動している特殊能力
    /// (封印により消える能力)
    /// </summary>
    public class RunningAbilities
    {
        public RunningAbilities(CardBook_Chara charaBook)
        {
            if (charaBook == null) //封印されている場合
            {
                SpecialStatuses = new List<SpecialStatus>();
                BombCost = -1;
            }
            else  //されていない場合
            {
                SpecialStatuses = charaBook.SpecialStatuses.ToList();
                OnDefeatedSkill = charaBook.OnDefeatedSkill;
                OnTurnEndSkill = charaBook.OnTurnEndSkill;
                BombAbility = charaBook.BombAbility;
                BombCost = charaBook.BombCost;
            }
        }

        /// <summary>
        /// AIの思考に用いる
        /// 準ディープコピー
        /// </summary>
        /// <param name="abilities"></param>
        private RunningAbilities(RunningAbilities abilities)
        {
            SpecialStatuses = abilities.SpecialStatuses;
            OnDefeatedSkill = abilities.OnDefeatedSkill;
            OnTurnEndSkill = abilities.OnTurnEndSkill;
            BombAbility = abilities.BombAbility;
            BombCost = abilities.BombCost;
        }

        /// <summary>
        /// 特殊ステータス(貫通、必殺、守護、鉄壁 = APやATKの表示が変化する)
        /// </summary>
        public List<SpecialStatus> SpecialStatuses { get; private set; }

        /// <summary>
        /// 死亡時スキル
        /// </summary>
        public OnDefeatedAbility OnDefeatedSkill { get;　private set; } = null;

        /// <summary>
        /// ターン終了時スキル
        /// </summary>
        public OnTurnEndSkill OnTurnEndSkill { get; private set; } = null;

        public BombAbilityBase BombAbility { get; private set; } = null;

        private int bombCost = -1;
        public int BombCost
        {
            get => bombCost; 
            private set
            {
                bombCost = value;
            }
        }

        public bool AddBombCost(int cost)
        {
            if (BombCost > 0)
            {
                BombCost = Math.Max(BombCost - cost, 0);
                return BombCost == 0;
            }
            else return false;
        }


        /// <summary>
        /// 鉄壁の数値は、SpecialStatusesリストに含まれる「SpecialStatus.Diffense」の数で決定する
        /// (多くて2を想定)
        /// </summary>
        public int Diffense => SpecialStatuses.Where(s => s == SpecialStatus.Diffense).Count();

        /// <summary>
        /// 引数の特殊ステータスを持っているか否か
        /// </summary>
        /// <param name="specialStatus"></param>
        /// <returns></returns>
        public bool HaveSpecialStatus(SpecialStatus specialStatus) => SpecialStatuses.Contains(specialStatus);
 
        /// <summary>
        /// AIの思考に使用
        /// このインスタンスが持つアビリティーの合計スコア
        /// </summary>
        /// <param name="chara"></param>
        /// <returns></returns>
        public static float GetAbilityScore(Data_Chara chara)
        {
            RunningAbilities ability = chara.MyAbilities;
            float ans = -4;
            if (ability.SpecialStatuses.Contains(SpecialStatus.Gardian)) ans += chara.HpData*3+5;
            if (ability.SpecialStatuses.Contains(SpecialStatus.Killer)) ans += 6;
            if (ability.SpecialStatuses.Contains(SpecialStatus.Surprise)) ans += chara.AtkData;
            ans += ability.SpecialStatuses.Where(s => s == SpecialStatus.Diffense).Count() * chara.HpData / 2f;
            if (ability.SpecialStatuses.Contains(SpecialStatus.Freeze)) ans += 4;
            if (ability.SpecialStatuses.Contains(SpecialStatus.Penetrate)) ans += chara.AtkData;
            if (ability.OnDefeatedSkill) ans += 3;
            if (ability.OnTurnEndSkill) ans += 5;
            return ans;
        }

        /// <summary>
        /// AIの思考に使用
        /// このインスタンスが持つアビリティーの合計スコア
        /// </summary>
        /// <param name="chara"></param>
        /// <returns></returns>
        public static float GetAbilityScore(Status_Chara chara)=> GetAbilityScore(chara.CharaData);

        /// <summary>
        /// AIの思考(シミュレーション)に使用
        /// このインスタンスをディープコピー
        /// もっと他にいい方法が無いのか?
        /// </summary>
        /// <param name="abilities"></param>
        /// <returns></returns>
        public static RunningAbilities AbiliyiesForAI(RunningAbilities abilities)=> new RunningAbilities(abilities);
    }
    #endregion

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Command;
using System;
using System.Linq;
using UniRx;
using System.Collections.ObjectModel;

public class AI : IOfflineRival//MonoSingleton<AI>
{
    //[SerializeField] private BombCardButton bombCardButton;
    private ReadOnlyCollection<IPlayable> FieldCards(bool isPlayer) => CommandManager.I.GetCards(isPlayer, PosEnum.Field);
    private ReadOnlyCollection<IPlayable> HandCards => CommandManager.I.GetCards(false, PosEnum.Hand);
    private bool ExistGardianOnRivalField => CommandManager.I.ExistGardian(true);
    private int RemainedMana => CommandManager.I.GetMana(false);



    private int LastPosition => FieldCards(false).Count;

    private void Start()
    {
        //TurnManager.I.StartAI.Subscribe(setting => StartAI(setting.Item1, setting.Item2));
    }

    //private void StartAI(bool canDraw,bool tutorial)
    //{
    // //   if (canDraw) Deck.I(false).Draw();
    //    StartCoroutine(Think(tutorial));
    //}

    public IEnumerator Run()//(bool tutorial)
    {
        yield return new WaitForSeconds(2);

        //if (tutorial)
        //{
        //    CommandManager.I.Run(new Command_TurnEnd(false));
        //    yield break;
        //}

        yield return PlayCard();

        if (RemainedMana > 0 && HandCards.Count<6)
        {
            //bombCardButton.Run();
            CommandManager.I.Run(new Command_UseBombCard(false));
            Debug.Log("ボムカード使用");
        }

        List<Status_Chara> allPlayerCharas = FieldCards(true).Where(c => c is Status_Chara).ToList().ConvertAll(c => c as Status_Chara);
        List<Status_Chara> allPlayerGardianCharas = allPlayerCharas.Where(c => c.CharaData.MyAbilities.SpecialStatuses.Contains(SpecialStatus.Gardian)).ToList();

        //守護が存在するか
        if (allPlayerGardianCharas.Count > 0)
        {

            //存在したら、守護のみを攻撃
            yield return Attack(allPlayerGardianCharas);

            //守護を倒しきれなかったら、攻撃終了
            if(ExistGardianOnRivalField)
            {
                CommandManager.I.Run(new Command_TurnEnd(false));
                yield break;
            }

        }

        //守護が存在しない or 倒しきった場合
        yield return Attack(FieldCards(true).ConvertType<Status_Chara>().NonNull().ToList());
        yield return Attack(FieldCards(true).ConvertType<Status_Chara>().NonNull().ToList());


        yield return new WaitForSeconds(2);
        CommandManager.I.Run(new Command_TurnEnd(false));
    }

    /// <summary>
    /// カードプレイに関するAI
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayCard()
    {
        //カードプレイに関するAI
        while (HandCards.Count > 0)
        {
            Command.Command command = null;
            float maxScore = 0;
            int handNumber = HandCards.Count;
            if (handNumber == 1) maxScore = 6;
            else if (handNumber == 2) maxScore = 3;
            else if (handNumber == 3) maxScore = 2;


            bool fieldFulled = FieldCards(false).Count > 7;
            var options = fieldFulled? HandCards.Where(c=>c is Status_Spell).ToList() : HandCards.ToList();


            foreach (var item in options)
            {
                //コストが支払えるか
                if (item.GetCost() > RemainedMana) continue;

                //スキルの評価
                PredictedScore predAbility = null;
                if (item.OnPlayAbility != null)
                {
                    predAbility = ((OnPlayAbility)item.OnPlayAbility).Predict((StatusBase)item,FieldCards(true).ConvertType<StatusBase>().NonNull().ToList(), FieldCards(false).ConvertType<StatusBase>().NonNull().ToList());
                }

                //総合的な評価(ステータス & アビリティー)
                float charaScore = (item is Status_Chara chara) ? CardScore(chara) : 0;
                float abilityScore = (predAbility != null) ? predAbility.Score : 0;
                float scoreTemp = charaScore + abilityScore;

                Debug.Log(item.GameObject + $"の評価 : キャラスコア = {charaScore}  アビリティースコア = {abilityScore}");

                //最大評価なら更新
                if (scoreTemp > maxScore)
                {
                    //コマンドの作成
                    command = new Command_CardPlay(false, item.PlayableCardId, LastPosition, (predAbility == null) ? null : predAbility.Target);

                    //最大スコアを塗り替える
                    maxScore = scoreTemp;
                }

            }

            //コマンドが決定したら実行
            if (command != null)
            {
                Debug.Log($"コマンドの点数は{maxScore}です");
                CommandManager.I.Run(command);
                ConnectionManager.Instance.Judge();
                //TurnManager.I.StartJudge(false);
            }
            else
            {
                break;
            }
            yield return new WaitForSeconds(0.03f);
        }

    }


    private IEnumerator Attack(List<Status_Chara> allPlayerCharas)
    {
        //COMのフィールド上の全キャラカード
        List<Status_Chara> allAICharas = FieldCards(false).ConvertType<Status_Chara>().NonNull().Where(c=>c.IsPlayable).ToList();

        if (allAICharas.Count > 5)
        {
            for (int i = 5; i < allAICharas.Count; i++)
            {
                allAICharas.RemoveAt(i);
            }
        }

        //フィールド上のキャラカード数
        int aiCharaNum = allAICharas.Count;
        int playerCharaNum = allPlayerCharas.Count();

        //すべての順列(攻撃順の決定に用いる)
        int[] aiCharaInd = new int[aiCharaNum];
        for (int i = 0; i < aiCharaNum; i++) aiCharaInd[i] = i;
        var list_permutation = AllPermutation(aiCharaInd);

        //求まった解をここに代入する
        float maxScore = 0;
        Queue<Command.Command> maxCommands = new Queue<Command.Command>();

        if (playerCharaNum > 0)
        {

            //ビット全探索
            for (int i = 0; i < Mathf.Pow(2, playerCharaNum); i++) // N<=2^5
            {
                //攻撃対象が決定
                List<int> targetOrderInd = new List<int>();

                for (int j = 0; j < playerCharaNum; j++)
                {
                    if ((i >> j & 1) == 1)
                    {
                        targetOrderInd.Add(j);

                    }
                }


                foreach (var order in list_permutation) // N<=5!
                {
                    //ターゲットの順番
                    PriorityQueue<CharacterForAI> targets = new PriorityQueue<CharacterForAI>((a, b) => a.Atk - b.Atk);

                    //アタッカーの順番
                    Queue<CharacterForAI> attackers = new Queue<CharacterForAI>();

                    //生成したコマンド
                    Queue<Command.Command> commands = new Queue<Command.Command>();


                    foreach (var attackerIndex in order)
                    {
                        attackers.Enqueue(new CharacterForAI(allAICharas[attackerIndex].CharaData));
                    }

                    foreach (var item in targetOrderInd)
                    {
                        targets.Enqueue(new CharacterForAI(allPlayerCharas[item].CharaData));
                    }


                    float score = 0;
                    //攻撃開始
                    while (attackers.Count > 0)
                    {
                        if (targets.Count == 0)
                        {
                            break;
                        }

                        var attacker = attackers.Dequeue();
                        var target = targets.Peek();

                        CommandManager.I.AttackForAISimulation(attacker.CharaData, target.CharaData);


                        commands.Enqueue(new Command.Command_Attack(false, (int)attacker.CharaData.PlayableId, (int)target.CharaData.PlayableId));

                        if (target.Dead)
                        {
                            score += target.CharaData.GetScore();
                            targets.Dequeue();
                        }

                        if (attacker.Dead)
                        {
                            score -= Mathf.Max(attacker.CharaData.GetScore(),1) / 2f;
                        }


                    }


                    while (attackers.Count > 0 && !ExistGardianOnRivalField)
                    {
                        var attacker = attackers.Dequeue();
                        commands.Enqueue(new Command_Attack(false, (int)attacker.CharaData.PlayableId));
                        score += attacker.Atk;
                    }


                    //この攻撃方法は可能
                    if (targets.Count == 0 && score > maxScore)
                    {
                        maxScore = score;
                        maxCommands = commands;
                    }

                    yield return new WaitForSeconds(0.03f);
                }

            }

        }


        while (maxCommands.Count > 0)
        {
            Command.CommandManager.I.Run(maxCommands.Dequeue());
            ConnectionManager.Instance.Judge();
        }




        allAICharas = FieldCards(false).ConvertType<Status_Chara>().NonNull().Where(c => c.IsPlayable).ToList();

        if (!ExistGardianOnRivalField)
        {
            foreach (var attacker in allAICharas)
            {
                CommandManager.I.Run(new Command_Attack(false, (int)attacker.CharaData.PlayableId));
                ConnectionManager.Instance.Judge();
            }
        }

        yield return null;


    }








    public List<T[]> AllPermutation<T>(params T[] array) where T : IComparable
    {
        var a = new List<T>(array).ToArray();
        var res = new List<T[]>();
        res.Add(new List<T>(a).ToArray());
        var n = a.Length;
        var next = true;
        while (next)
        {
            next = false;

            // 1
            int i;
            for (i = n - 2; i >= 0; i--)
            {
                if (a[i].CompareTo(a[i + 1]) < 0) break;
            }
            // 2
            if (i < 0) break;

            // 3
            var j = n;
            do
            {
                j--;
            } while (a[i].CompareTo(a[j]) > 0);

            if (a[i].CompareTo(a[j]) < 0)
            {
                // 4
                var tmp = a[i];
                a[i] = a[j];
                a[j] = tmp;
                Array.Reverse(a, i + 1, n - i - 1);
                res.Add(new List<T>(a).ToArray());
                next = true;
            }
        }
        return res;
    }

    public float CardScore(Status_Chara chara)
    {
        Debug.LogWarning("必殺等の特殊攻撃を考慮しいない");
        return chara.Atk * 2 + chara.Hp;
    }

}




public class PriorityQueue<T> : IEnumerable<T>
{
    private readonly List<T> _data = new List<T>();
    private readonly IComparer<T> _comparer;
    private readonly bool _isDescending;

    public PriorityQueue(IComparer<T> comparer, bool isDescending = true)
    {
        _comparer = comparer;
        _isDescending = isDescending;
    }

    public PriorityQueue(Comparison<T> comparison, bool isDescending = true)
        : this(Comparer<T>.Create(comparison), isDescending)
    {
    }

    public PriorityQueue(bool isDescending = true)
        : this(Comparer<T>.Default, isDescending)
    {
    }

    public void Enqueue(T item)
    {
        _data.Add(item);
        var childIndex = _data.Count - 1;
        while (childIndex > 0)
        {
            var parentIndex = (childIndex - 1) / 2;
            if (Compare(_data[childIndex], _data[parentIndex]) >= 0)
                break;
            Swap(childIndex, parentIndex);
            childIndex = parentIndex;
        }
    }

    public T Dequeue()
    {
        var lastIndex = _data.Count - 1;
        var firstItem = _data[0];
        _data[0] = _data[lastIndex];
        _data.RemoveAt(lastIndex--);
        var parentIndex = 0;
        while (true)
        {
            var childIndex = parentIndex * 2 + 1;
            if (childIndex > lastIndex)
                break;
            var rightChild = childIndex + 1;
            if (rightChild <= lastIndex && Compare(_data[rightChild], _data[childIndex]) < 0)
                childIndex = rightChild;
            if (Compare(_data[parentIndex], _data[childIndex]) <= 0)
                break;
            Swap(parentIndex, childIndex);
            parentIndex = childIndex;
        }
        return firstItem;
    }

    public T Peek()
    {
        return _data[0];
    }

    private void Swap(int a, int b)
    {
        var tmp = _data[a];
        _data[a] = _data[b];
        _data[b] = tmp;
    }

    private int Compare(T a, T b)
    {
        return _isDescending ? _comparer.Compare(b, a) : _comparer.Compare(a, b);
    }

    public int Count => _data.Count;

    public IEnumerator<T> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
public class PriorityQueue<TKey, TValue> : IEnumerable<TValue>
{
    private readonly List<KeyValuePair<TKey, TValue>> _data = new List<KeyValuePair<TKey, TValue>>();
    private readonly bool _isDescending;
    private readonly Func<TValue, TKey> _keySelector;
    private readonly IComparer<TKey> _keyComparer;

    public PriorityQueue(Func<TValue, TKey> keySelector, bool isDescending = true)
        : this(keySelector, Comparer<TKey>.Default, isDescending)
    {
    }

    public PriorityQueue(Func<TValue, TKey> keySelector, IComparer<TKey> keyComparer, bool isDescending = true)
    {
        _keySelector = keySelector;
        _keyComparer = keyComparer;
        _isDescending = isDescending;
    }

    public void Enqueue(TValue item)
    {
        _data.Add(new KeyValuePair<TKey, TValue>(_keySelector(item), item));
        var childIndex = _data.Count - 1;
        while (childIndex > 0)
        {
            var parentIndex = (childIndex - 1) / 2;
            if (Compare(_data[childIndex].Key, _data[parentIndex].Key) >= 0)
                break;
            Swap(childIndex, parentIndex);
            childIndex = parentIndex;
        }
    }

    public TValue Dequeue()
    {
        var lastIndex = _data.Count - 1;
        var firstItem = _data[0];
        _data[0] = _data[lastIndex];
        _data.RemoveAt(lastIndex--);
        var parentIndex = 0;
        while (true)
        {
            var childIndex = parentIndex * 2 + 1;
            if (childIndex > lastIndex)
                break;
            var rightChild = childIndex + 1;
            if (rightChild <= lastIndex && Compare(_data[rightChild].Key, _data[childIndex].Key) < 0)
                childIndex = rightChild;
            if (Compare(_data[parentIndex].Key, _data[childIndex].Key) <= 0)
                break;
            Swap(parentIndex, childIndex);
            parentIndex = childIndex;
        }
        return firstItem.Value;
    }

    public TValue Peek()
    {
        return _data[0].Value;
    }

    private void Swap(int a, int b)
    {
        var tmp = _data[a];
        _data[a] = _data[b];
        _data[b] = tmp;
    }

    private int Compare(TKey a, TKey b)
    {
        return _isDescending ? _keyComparer.Compare(b, a) : _keyComparer.Compare(a, b);
    }

    public int Count => _data.Count;

    public IEnumerator<TValue> GetEnumerator()
    {
        return _data.Select(r => r.Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

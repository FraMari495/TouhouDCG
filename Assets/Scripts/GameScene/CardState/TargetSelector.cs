using Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class TargetSelector
{

    /// <summary>
    /// プレイヤーに選択肢を与える
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal IEnumerator StartSelection(StatusBase owner, CardType[] targetType, Condition condition, Action<StatusBase> onTargetDecided)
    {
        //プレイヤーに選択肢を与えている間はターン終了ボタンを無効にする
        ReactivePropertyList.I.Wait(true);

        if (!CheckConsistency(targetType)) yield break;

        //選択状態の間は、カードを操作できないようにする
        CommandManager.I.Wait();

        var playerCards = CommandManager.I.GetCards(true, PosEnum.Field).Where(c => targetType.Contains(c.Type)).ConvertType<StatusBase>().ToList();
        var rivalCards = CommandManager.I.GetCards(false, PosEnum.Field).Where(c => targetType.Contains(c.Type)).ConvertType<StatusBase>().ToList();



        //条件に合ったカードのみに絞る
        List<StatusBase> targetsOption = condition.DecideOption(owner, playerCards, rivalCards);

        //選択結果を保持する変数
        StatusBase decided = null;
        
        if (targetsOption.Count > 1)
        {
            //選択肢が2以上なら、プレイヤーに選択させる

            //選択されたカードをこのリストに入れる
            List<StatusBase> SelectedTarget = new List<StatusBase>();
            targetsOption.ConvertAll(target => target.StartCoroutine(target.GetComponentInChildren<Target>().SetTarget(SelectedTarget)));

            //選択肢の中から一つが選ばれるまで待機
            yield return new WaitWhile(() => SelectedTarget.Count==0);
            decided = SelectedTarget[0];
        }
        else if (targetsOption.Count == 1)
        {
            //選択肢が1つなら自動的にそれを選択
            decided = targetsOption[0];
        }

        //選択完了のコールバックメソッドを実行
        onTargetDecided(decided);

        //選択が終了したため、ターン終了ボタンを再度有効にする
        ReactivePropertyList.I.Wait(false);
    }


    private bool CheckConsistency(CardType[] targetType)
    {
        if (targetType == null)
        {
            Debug.LogError("ターゲットが設定されていません");
            Debug.LogError("OnPlayAbilityのTargetがNoTargetではないとき、targetTypeは必ず設定してください");
            return false;
        }
        return true;
    }
}

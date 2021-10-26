using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Position;
using Player;



public class TargetSelector
{

    /// <summary>
    /// プレイヤーに選択肢を与える
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public IEnumerator StartSelection(StatusBase owner, CardType[] targetType, Condition condition, Action<StatusBase> onTargetDecided)
    {
        //プレイヤーに選択肢を与えている間はターン終了ボタンを無効にする
        TurnEndButton.I.Show(false);

        if (!CheckConsistency(targetType)) yield break;

        //選択状態の間は、カードを操作できないようにする
        Field.Wait();
        Hand.Wait();

        //targetTypeに含まれている全カード(ヒーロー)を探し、、、
        var playerCards = Field.I(true).GetStatus(targetType);
        var rivalCards = Field.I(false).GetStatus(targetType);

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
        TurnEndButton.I.Show(TurnManager.I.Turn);
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

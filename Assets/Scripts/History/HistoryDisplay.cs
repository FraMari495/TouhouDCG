//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class HistoryDisplay : MonoSingleton<HistoryDisplay>
//{
//    [SerializeField] private GameObject historyText;
//    [SerializeField] private Transform historyTrn;

//    //History‚ð•Û‘¶‚·‚é
//    private List<HistoryBase> histories = new List<HistoryBase>();
//    public void AddHistory(HistoryBase history)
//    {
//        histories.Add(history);
//        Transform trn = Instantiate(historyText, historyTrn).transform;
//        trn.GetComponent<Text>().text = history.GetLog();
//        trn.SetAsFirstSibling();
//    }
//}

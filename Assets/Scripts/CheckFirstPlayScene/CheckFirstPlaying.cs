using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFirstPlaying : MonoBehaviour
{
    [SerializeField] private bool ShowLog;
    [SerializeField] private int[] tutorialCard_player;
    [SerializeField] private int[] tutorialCard_rival;
    

    private void Start()
    {
        DG.Tweening.DOTween.SetTweensCapacity(tweenersCapacity: 200, sequencesCapacity: 200);
        Debug.unityLogger.logEnabled = ShowLog;
        //WBTransition.SceneManager.LoadScene("Lobby");

        if (!SaveSystem.Instance.UserData.DoneTutorial)
        {
            VsCPUTutorial();
            SaveSystem.Instance.UserData.DoneTutorial = true;
            SaveSystem.Instance.Save();
        }
        else
        {
            WBTransition.SceneManager.LoadScene("Lobby");
        }
    }

    private void VsCPUTutorial()
    {
        bool tutorial = true;

        List<int> myDeck = new List<int>();

        foreach (var item in SaveSystem.Instance.UserData.Deck)
        {
            for (int i = 0; i < item.Number; i++)
            {
                myDeck.Add(item.CardId);
            }
        }


        List<int> rivalDeck = new List<int>();


        foreach (var item in SaveSystem.Instance.UserData.Deck)
        {
            for (int i = 0; i < item.Number; i++)
            {
                rivalDeck.Add(item.CardId);
            }
        }
        Debug.Log("シーンチェンジ直前");

        if (tutorial)
        {
            WBTransition.SceneManager.LoadScene("GameScene",
               new Dictionary<string, object>() { { "offline", true }, { "myDeck", tutorialCard_player }, { "rivalDeck", tutorialCard_rival }, { "tutorial", true } },
               new Dictionary<string, object>() { { "offline", true }, { "tutorial", true } }
               );
        }
        else
        {
            WBTransition.SceneManager.LoadScene("GameScene",
               new Dictionary<string, object>() { { "offline", true }, { "myDeck", myDeck.ToArray() }, { "rivalDeck", rivalDeck.ToArray() }, { "tutorial", false } },
               new Dictionary<string, object>() { { "offline", true }, { "tutorial", false } }
               );
        }
    }

}

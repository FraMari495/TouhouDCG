using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace WBTutorial
{
    public class TutorialObject : MonoBehaviour
    {
        [SerializeField] private Parts_Obj[] unmasks;
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI upperMessageText, lowerMessageText;

        public void Next(OpenParts[] open, string message, bool upperMessage)
        {
            AnimationManager.I.AddSequence(() => DOTween.Sequence().AppendCallback(() => {
            unmasks.ForEach(m => m.Open(open.Contains(m.Parts)));
            ShowMessage(message,upperMessage);
            }),"tutorial");
        }

        public void End()
        {
            Destroy(this.gameObject);
        }

        private void ShowMessage(string message,bool upper)
        {
            if (upper)
            {
                upperMessageText.text = message;
                lowerMessageText.text = "";
            }
            else
            {
                lowerMessageText.text = message;
                upperMessageText.text = "";
            }
        }
    }

    [Serializable]
    public class Parts_Obj
    {
        [SerializeField] private OpenParts parts;
        [SerializeField] private GameObject obj;

        public OpenParts Parts { get => parts; }

        public void Open(bool open)
        {
            obj.SetActive(open);
        }
    }
}


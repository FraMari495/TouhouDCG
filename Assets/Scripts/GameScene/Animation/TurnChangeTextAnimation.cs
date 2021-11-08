using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Animation
{

    /// <summary>
    /// ターン変更時に表示するテキストアニメーション
    /// </summary>
    internal class TurnChangeTextAnimation : MonoBehaviour
    {
        [SerializeField] private Animator textAnimator;
        [SerializeField] private Animator panelAlphaAnimator;
        [SerializeField] private TextMeshProUGUI turnChangeText;
        [SerializeField] private Image line1;
        [SerializeField] private Image line2;
        [SerializeField] private Color playerColor;
        [SerializeField] private Color rivalColor;

        internal void Play(bool isPlayer)
        {
            Color c = isPlayer ? playerColor : rivalColor;
            turnChangeText.color = c;
            line1.color = c;
            line2.color = c;

            turnChangeText.text = isPlayer ? "Your Turn" : "Rival Turn";

            textAnimator.Play("TurnChange");
            panelAlphaAnimator.Play("TurnChange_Panel");
        }

    }
}


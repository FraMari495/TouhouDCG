using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Animation
{
    internal class DamageAnimation : MonoBehaviour
    {
        [SerializeField] private Image damageStarImage;
        [SerializeField] private Sprite damageStarKiller;


        internal void Play(int damage, bool killer)
        {
            if (killer) damageStarImage.sprite = damageStarKiller;
            Text t = this.GetComponentInChildren<Text>();
            t.text = damage.ToString();
        }
    }
}

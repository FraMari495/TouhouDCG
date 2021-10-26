using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private Image damageStarImage;
    [SerializeField] private Sprite damageStarKiller;


    public void Initialize(int damage,bool killer)
    {
        if(killer)damageStarImage.sprite = damageStarKiller;
        Text t = this.GetComponentInChildren<Text>();
        t.text = damage.ToString();
    }
}

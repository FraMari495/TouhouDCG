using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;

internal class HeroVisualController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private bool isPlayer;

    public bool IsPlayer => isPlayer;
    private Status_Hero Status { get; set; }

    // Start is called before the first frame update

    private void Awake()
    {
        Status = this.GetComponent<Status_Hero>();

        Status.UpdateHpUI.Subscribe(hp => hpText.text = hp.ToString());
    }

    void Start()
    {
        Status.Initialize(IsPlayer);

       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using Command;

public class BombCardButton : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private bool isPlayer;



    [SerializeField] private GameObject clickable;


    private bool show = true;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!show || !isPlayer || !TurnManager.I.Turn) return;

        Run();
    }

    public void Run()
    {

        if (CommandManager.I.GetMana(isPlayer)<1) throw new System.Exception("ƒ}ƒi‚ª‘«‚è‚Ü‚¹‚ñ");
        clickable.SetActive(false);
        show = false;
     

        //int rand = Random.Range(0, 2);

        CommandManager.I.Run(new Command_UseBombCard(isPlayer));
        //ReactivePropertyList.I.StartJudge(false);
        ConnectionManager.Instance.Judge();
        //GameSetting.I.StartJudge(false);
    }

    private void Start()
    {
        Debug.Log(this.GetType());

        clickable.SetActive(false);
        ReactivePropertyList.I.O_NewTurnNotif.Subscribe(turn => {
            if (turn == isPlayer)
            {
                show = true;
            }
        });

        ReactivePropertyList.I.O_Judge.Subscribe(_ => {

            bool mana = CommandManager.I.GetMana(isPlayer) > 0;
            bool turn = TurnManager.I.Turn == isPlayer;

            clickable.SetActive(mana && turn && show);

        });

        Debug.Log(this.GetType()+"end");

    }

}

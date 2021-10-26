using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using Position;
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

        if (!Hand.I(isPlayer).UseMana(1)) throw new System.Exception("ƒ}ƒi‚ª‘«‚è‚Ü‚¹‚ñ");
        clickable.SetActive(false);
        show = false;
     

        int rand = Random.Range(0, 2);

        CommandManager.I.Run(new Command_UseBombCard(rand,isPlayer));
        TurnManager.I.StartJudge(false);
    }

    private void Start()
    {
        Debug.Log(this.GetType());

        clickable.SetActive(false);
        TurnManager.I.NewTurnNotif.Subscribe(turn => {
            if (turn == isPlayer)
            {
                show = true;
            }
        });


        TurnManager.I.Judge.Subscribe(_ => {

            bool mana = Hand.I(isPlayer).RemainedMana > 0;
            bool turn = TurnManager.I.Turn == isPlayer;

            clickable.SetActive(mana && turn && show);

        });

        Debug.Log(this.GetType()+"end");

    }

}

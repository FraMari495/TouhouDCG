using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.EventSystems;

namespace WBTutorial
{
    public enum OpenParts
    {
        All,
        Hand,
        PlayerField,
        RivalField,
        RivalHero,
        TurnEnd,
        Bomb,
        Hand2,
        Close,
        PlayerHp,
        PlayerMana,
        RivalHp
    }

    public class TutorialManager : MonoBehaviour,IPointerClickHandler
    {
        [SerializeField] private TutorialParts[] parts;
        public bool Tutorial { get;private set; }


        private Queue<TutorialParts> TutorialQueue { get; set; }
        private TutorialObject tutorialObj;
        private IDisposable disposable, disposable2;
        private Subject<Unit> clicked = new Subject<Unit>();
        public void OnPointerClick(PointerEventData eventData)
        {
            clicked.OnNext(Unit.Default);
        }

        public void StartTutorial()
        {
            Tutorial = true;
            TutorialQueue = new Queue<TutorialParts>();
            parts.ForEach(p => TutorialQueue.Enqueue(p));
            tutorialObj = GetComponent<TutorialObject>();
            var part = TutorialQueue.Peek();
            tutorialObj.Next(part.Open, part.Message, part.Upper);

            disposable = Command.CommandManager.I.ranCommand.Where(c => c.IsPlayer).Subscribe(CommandRan);
            disposable2 = clicked.Subscribe(ScreenClicked);
        }

        private void OpenParts(Command.Command rancommand, Command.Command requiredCommand)
        {
            if (!rancommand.Same(requiredCommand))
            {
                Debug.LogError("ˆÙ‚È‚éƒRƒ}ƒ“ƒh‚Å‚·");
            }
            var part = TutorialQueue.Peek();
            tutorialObj.Next(part.Open, part.Message, part.Upper);

        }

        private void CommandRan(Command.Command command)
        {
            Debug.Log(command.IsPlayer+":"+command.GetType());
            if (TutorialQueue.Count > 1)
            {
                var c = TutorialQueue.Dequeue();
                OpenParts(command,c.Command);
            }

            else tutorialObj.End();

        }

        private void ScreenClicked(Unit _)
        {
            if (TutorialQueue.Count > 1)
            {
                if (TutorialQueue.Peek().Command == null)
                {
                    TutorialQueue.Dequeue();
                    var part = TutorialQueue.Peek();
                    tutorialObj.Next(part.Open, part.Message, part.Upper);
                }
            }

            else tutorialObj.End();
        }

        public void RunRivalCommand()
        {
            while (TutorialQueue.Count > 0)
            {
                if (!TutorialQueue.Peek().Command?.IsPlayer ?? false)
                {
                    var command = TutorialQueue.Peek();
                    Command.CommandManager.I.Run(command.Command);
                    CommandRan(command.Command);
                }
                else
                {
                    break;
                }
            }
        }

        private void OnDestroy()
        {
            disposable?.Dispose();
            disposable2?.Dispose();
        }
    }

    [Serializable]
    public class TutorialParts
    {
        [SerializeField] private Command.Command command = null;
        [SerializeField] private OpenParts[] open;
        [SerializeField, TextArea(3, 5)] private string message;
        [SerializeField] private bool upper;

        public Command.Command Command { get => command; }
        public OpenParts[] Open { get => open;  }
        public string Message { get => message;  }
        public bool Upper => upper;
    }
}

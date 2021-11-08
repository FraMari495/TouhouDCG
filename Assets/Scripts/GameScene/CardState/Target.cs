//using Position;
using Command;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;



public abstract class Target : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    [SerializeField] private GameObject targetAura;
    [SerializeField] private CanvasGroup cg;
    protected StatusBase Status { get; set; }
    public abstract bool IsPlayer { get; protected set; }

    /// <summary>
    /// フィールドに守護がいないことが、選択可能する条件
    /// </summary>
    protected bool Condition => CommandManager.I.CanBeTarget(IsPlayer, Status);

    private Subject<Unit> onClicked = new Subject<Unit>();

    protected virtual void Start()
    {
        targetAura.SetActive(false);
        ReactivePropertyList.I.O_ShowAttackTarget.Where(b=>b.isPlayer == IsPlayer && Condition).Subscribe(b=>ShowAura(b.show));

    }

    //選択肢となっている場合はtrue
    private bool isOption = false;
    public bool IsOption
    {
        get => isOption;
        set
        {
            isOption = value;
            // cg.blocksRaycasts = value;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isOption)
        {
            onClicked.OnNext(Unit.Default);
        }
        else
        {
            var newPointerEvent = new PointerEventData(EventSystem.current) { position = eventData.position };
            var objectsHit = new List<RaycastResult>();
            EventSystem.current.RaycastAll(newPointerEvent, objectsHit);

            var c = objectsHit.FirstOrDefault(o => o.gameObject.GetComponentInChildren<IPointerClickHandler>() != null);
            if (c.gameObject != null)
            {
                var handler = c.gameObject.GetComponentInChildren<IPointerClickHandler>();
                if (handler != this as IPointerClickHandler)
                {
                    handler.OnPointerClick(eventData);
                }

            }
        }
    }

    /// <summary>
    /// このカードを選択肢とする
    /// 選択状態が終了した場合は、オーラを消す
    /// </summary>
    public IEnumerator SetTarget(List<StatusBase> statusBase)
    {
        //オーラを表示
        targetAura.SetActive(true);
        IsOption = true;

        var disposable = onClicked.Subscribe(_ => statusBase.Add(Status));
        yield return new WaitWhile(() => statusBase.Count == 0);

        //オーラを非表示
        targetAura.SetActive(false);
        IsOption = false;
        disposable.Dispose();
    }


    /// <summary>
    /// オーラ(選択可能を示す)を表示
    /// </summary>
    /// <param name="show"></param>
    private void ShowAura(bool show)
    {

        if (!show)
        {
            //縮小する
            if (targetAura.activeInHierarchy) this.transform.localScale /= 1.1f;

            //オーラを非表示にする
            targetAura.SetActive(false);
        }
        else
        {
            //もし選択可能なら
            if (Condition)
            {
                //拡大
                this.transform.localScale *= 1.1f;

                //オーラを表示
                targetAura.SetActive(true);
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        //攻撃対象が決まる3つの条件

        //1. ポインターがオブジェクトを持っている
        GameObject obj = eventData.pointerDrag;
        if (obj == null) return;

        //2. そのオブジェクトはFieldView_Charaを持っている
        if (CommandManager.I.GetCards(!IsPlayer, PosEnum.Field).Contains(obj.GetComponent<IPlayable>()))
        {

            //3. 体力が残っている(アニメーションの関係上、体力が0でも盤面に残っている可能性がある)
            if (!Condition) return;

            SelectedAsAttackTarget(obj.GetComponent<IPlayable>());
        }
    }

    protected abstract void SelectedAsAttackTarget(IPlayable attacker);
}


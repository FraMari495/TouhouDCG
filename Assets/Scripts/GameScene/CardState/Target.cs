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
    /// �t�B�[���h�Ɏ�삪���Ȃ����Ƃ��A�I���\�������
    /// </summary>
    protected bool Condition => CommandManager.I.CanBeTarget(IsPlayer, Status);

    private Subject<Unit> onClicked = new Subject<Unit>();

    protected virtual void Start()
    {
        targetAura.SetActive(false);
        ReactivePropertyList.I.O_ShowAttackTarget.Where(b=>b.isPlayer == IsPlayer && Condition).Subscribe(b=>ShowAura(b.show));

    }

    //�I�����ƂȂ��Ă���ꍇ��true
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
    /// ���̃J�[�h��I�����Ƃ���
    /// �I����Ԃ��I�������ꍇ�́A�I�[��������
    /// </summary>
    public IEnumerator SetTarget(List<StatusBase> statusBase)
    {
        //�I�[����\��
        targetAura.SetActive(true);
        IsOption = true;

        var disposable = onClicked.Subscribe(_ => statusBase.Add(Status));
        yield return new WaitWhile(() => statusBase.Count == 0);

        //�I�[�����\��
        targetAura.SetActive(false);
        IsOption = false;
        disposable.Dispose();
    }


    /// <summary>
    /// �I�[��(�I���\������)��\��
    /// </summary>
    /// <param name="show"></param>
    private void ShowAura(bool show)
    {

        if (!show)
        {
            //�k������
            if (targetAura.activeInHierarchy) this.transform.localScale /= 1.1f;

            //�I�[�����\���ɂ���
            targetAura.SetActive(false);
        }
        else
        {
            //�����I���\�Ȃ�
            if (Condition)
            {
                //�g��
                this.transform.localScale *= 1.1f;

                //�I�[����\��
                targetAura.SetActive(true);
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        //�U���Ώۂ����܂�3�̏���

        //1. �|�C���^�[���I�u�W�F�N�g�������Ă���
        GameObject obj = eventData.pointerDrag;
        if (obj == null) return;

        //2. ���̃I�u�W�F�N�g��FieldView_Chara�������Ă���
        if (CommandManager.I.GetCards(!IsPlayer, PosEnum.Field).Contains(obj.GetComponent<IPlayable>()))
        {

            //3. �̗͂��c���Ă���(�A�j���[�V�����̊֌W��A�̗͂�0�ł��ՖʂɎc���Ă���\��������)
            if (!Condition) return;

            SelectedAsAttackTarget(obj.GetComponent<IPlayable>());
        }
    }

    protected abstract void SelectedAsAttackTarget(IPlayable attacker);
}


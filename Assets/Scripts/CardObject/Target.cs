using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{

    public abstract class Target : MonoBehaviour, IPointerClickHandler, IDropHandler
    {
        [SerializeField] private GameObject targetAura;
        private Status status;
        public abstract bool IsPlayer { get; protected set; }

        /// <summary>
        /// �t�B�[���h�Ɏ�삪���Ȃ����Ƃ��A�I���\�������
        /// </summary>
        protected abstract bool Condition { get; }

        private Subject<Unit> onClicked = new Subject<Unit>();

        private void Start()
        {
            targetAura.SetActive(false);
            status = GetComponentInParent<Status>();
        }

        //�I�����ƂȂ��Ă���ꍇ��true
        private bool isOption = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            //���̃J�[�h���I�����Ƃ��Ă������Ă��Ȃ��ꍇ�͒ʏ�̃N���b�N���Ă�
            if (!isOption)
            {
                this.GetComponentInParent<CardInputHandler>().OnPointerClick(eventData);
            }

            onClicked.OnNext(Unit.Default);
        }

        /// <summary>
        /// ���̃J�[�h��I�����Ƃ���
        /// �I����Ԃ��I�������ꍇ�́A�I�[��������
        /// </summary>
        public IEnumerator SetTarget(List<StatusBase> statusBase)
        {
            //�I�[����\��
            targetAura.SetActive(true);
            isOption = true;

            var disposable = onClicked.Subscribe(_ => statusBase.Add(status));
            yield return new WaitWhile(() => statusBase.Count == 0);

            //�I�[�����\��
            targetAura.SetActive(false);
            isOption = false;
            disposable.Dispose();
        }


        /// <summary>
        /// �I�[��(�I���\������)��\��
        /// </summary>
        /// <param name="show"></param>
        public void ShowAura(bool show)
        {
            if (!show)
            {
                //�k������
                if(targetAura.activeInHierarchy)this.transform.localScale /= 1.1f;

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
            if (obj.GetComponent<CardInputHandler>().CurrentState is CardState_Field field)
            {
                //3. FieldView_Chara���ΐ푊��̂��̂ł���
                if (field.IsPlayer == IsPlayer) return;

                //4. �̗͂��c���Ă���(�A�j���[�V�����̊֌W��A�̗͂�0�ł��ՖʂɎc���Ă���\��������)
                if (!Condition) return;

                SelectedAsAttackTarget(field);
            }
        }

        protected abstract void SelectedAsAttackTarget(CardState_Field field);
    }

}

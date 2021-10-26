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
        /// フィールドに守護がいないことが、選択可能する条件
        /// </summary>
        protected abstract bool Condition { get; }

        private Subject<Unit> onClicked = new Subject<Unit>();

        private void Start()
        {
            targetAura.SetActive(false);
            status = GetComponentInParent<Status>();
        }

        //選択肢となっている場合はtrue
        private bool isOption = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            //このカードが選択肢としてあがっていない場合は通常のクリックを呼ぶ
            if (!isOption)
            {
                this.GetComponentInParent<CardInputHandler>().OnPointerClick(eventData);
            }

            onClicked.OnNext(Unit.Default);
        }

        /// <summary>
        /// このカードを選択肢とする
        /// 選択状態が終了した場合は、オーラを消す
        /// </summary>
        public IEnumerator SetTarget(List<StatusBase> statusBase)
        {
            //オーラを表示
            targetAura.SetActive(true);
            isOption = true;

            var disposable = onClicked.Subscribe(_ => statusBase.Add(status));
            yield return new WaitWhile(() => statusBase.Count == 0);

            //オーラを非表示
            targetAura.SetActive(false);
            isOption = false;
            disposable.Dispose();
        }


        /// <summary>
        /// オーラ(選択可能を示す)を表示
        /// </summary>
        /// <param name="show"></param>
        public void ShowAura(bool show)
        {
            if (!show)
            {
                //縮小する
                if(targetAura.activeInHierarchy)this.transform.localScale /= 1.1f;

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
            if (obj.GetComponent<CardInputHandler>().CurrentState is CardState_Field field)
            {
                //3. FieldView_Charaが対戦相手のものである
                if (field.IsPlayer == IsPlayer) return;

                //4. 体力が残っている(アニメーションの関係上、体力が0でも盤面に残っている可能性がある)
                if (!Condition) return;

                SelectedAsAttackTarget(field);
            }
        }

        protected abstract void SelectedAsAttackTarget(CardState_Field field);
    }

}

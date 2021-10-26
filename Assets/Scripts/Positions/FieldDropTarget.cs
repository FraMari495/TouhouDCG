using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    /// <summary>
    /// ��D����t�B�[���h�ɃJ�[�h��D&D���ꂽ�Ƃ��̏��� (�J�[�h���v���C�g�p�Ƃ����Ƃ��́A�t�B�[���h���̏���)
    ///     �h���b�v�ʒu�� CardState_Hand_Chara �ɕԂ�
    /// </summary>
    public class FieldDropTarget : MonoBehaviour, IDropHandler
    {
        [SerializeField] private bool isPlayer;

        public void OnDrop(PointerEventData eventData)
        {
            //�v���C�ʒu�����܂�3�̏���

            //1. �|�C���^�[���I�u�W�F�N�g�������Ă���
            GameObject obj = eventData.pointerDrag;
            if (obj == null) return;

            //2. ���̃J�[�h�̃X�e�[�g�� CardState_Hand_Chara �ł���
            if (obj.GetComponent<CardInputHandler>().CurrentState is CardState_Hand hand)
            {

                //3. HandView���ΐ푊��̂��̂ł͂Ȃ�
                if (hand.IsPlayer != isPlayer) return;

                //�v���C�ʒu�̓|�C���^�[�̈ʒu��p���Č��肷��

                //�ŏ��͓�����0�Ƃ��Ă���
                int answer = 0;

                //�t�B�[���h��̃J�[�h�����ԂɊm�F����
                foreach (Transform trn in this.transform)
                {
                    //�|�C���^�[���E�ɂ��邩
                    if (trn.position.x > eventData.position.x)
                    {
                        //Yes
                        break;
                    }
                    else
                    {
                        //No
                        answer++;
                    }
                }

                //CardState_Hand_Chara �Ƀv���C�ʒu��`����
                hand.PlayPos = answer;
            }
        }
    }
}

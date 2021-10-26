using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �U�����ɕ\��������
/// </summary>
[RequireComponent(typeof(CanvasGroup),typeof(Image))]
public class ArrowController : MonoBehaviour
{
    private RectTransform rect;
    private Transform trn;
    
    /// <summary>
    /// ���I�u�W�F�N�g�̐���
    /// </summary>
    /// <param name="startTrn"></param>
    public static void CreateArrow(PointerEventData eventData,Transform trn)
    {
        //ArrowController�����I�u�W�F�N�g�̐���(�����I��CanvasGroup,Image���A�^�b�`�����)
        ArrowController ctrl = (new GameObject("Arrow",typeof(ArrowController))).GetComponent<ArrowController>();
        ctrl.transform.SetParent(trn);
       
        //���̉摜��ݒ�
        ctrl.GetComponent<Image>().sprite = Resources.Load<Sprite>("Arrow");
        ctrl.GetComponent<Image>().maskable = false;

        //���I�u�W�F�N�g�̐������I��������߁A�����ݒ���s��
        ctrl.Initialize(eventData);
    }

    private void Initialize(PointerEventData eventData)
    {
        //Raycast���u���b�N���Ȃ�
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;

        //�I�u�W�F�N�g�̈ʒu���A���̎n�_�ɑ�����
        rect = this.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0, 0.5f);

        //�N���b�N����Ă��Ȃ��ꍇ�A���̃I�u�W�F�N�g���폜����
        if (!Input.GetMouseButton(0))
        {
            Debug.Log("OnBeginDrag���\�b�h����CreateArrow���Ă�ł�������");
            Destroy(this.gameObject);
            return;
        }

        //this.transform�͒x���ł��B���̂��߂�������[�v��(Scaling��while���[�v)�ɂ͎g���ׂ��ł͂���܂���
        //����āA���炩����this.transform�̒��g��trn�ɓ���Ă����܂�
        trn = this.transform;

        //�n�_��CreateArrow()�̈����̃|�W�V�����ɐݒ�
        trn.position = eventData.pointerDrag.transform.position;

        //�}�E�X�̓����ɍ��킹�ĐL�яk�݂�����
        StartCoroutine(Scaling(eventData));
    }

    private IEnumerator Scaling(PointerEventData eventData)
    {
        ShowTargetAura(true);
        while (true)
        {
            //�}�E�X��������Ă�����A���[�v�𔲂���( = ���̃I�u�W�F�N�g���폜����)
            if (eventData.pointerDrag==null) break;

            //�I�_(�}�E�X�̃|�W�V����)-�n�_(���̃I�u�W�F�N�g�̈ʒu)
            var diff = eventData.position - (Vector2)trn.position;

            //�Ίp(�x)
            var theta = Mathf.Atan2(diff.y , diff.x) * 180 / Mathf.PI;

            //���̒������Adiff�̑傫���ɍ��킹��
            rect.sizeDelta = new Vector3(diff.magnitude, rect.sizeDelta.y);

            //������diff�ɍ��킹��
            trn.rotation = Quaternion.Euler(0, 0, theta);

            //0.1�b�҂�
            yield return new WaitForSeconds(0.1f);
        }
        ShowTargetAura(false);

        //�h���b�O���I��������߁A�����폜����
        Destroy(this.gameObject);
    }

    /// <summary>
    /// �U���\�ȃJ�[�h�̃I�[����\������
    /// </summary>
    public void ShowTargetAura(bool show)
    {
        Position.Field.I(false).GetComponentsInChildren<Player.Target>().ForEach(c => c.ShowAura(show));
        Status_Hero.I(false).GetComponentInChildren<Player.Target>().ShowAura(show);
    }

}

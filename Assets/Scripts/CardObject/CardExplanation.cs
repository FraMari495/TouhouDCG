using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �Q�[�����A�J�[�h���N���b�N����Əڍׂ��\�������B
/// ���̐����p�̉�ʂ��i��
/// </summary>
public class CardExplanation : MonoSingleton<CardExplanation>,IPointerClickHandler
{
    /// <summary>
    /// �\���A��\����؂�ւ���ۂɗp����
    /// </summary>
    private CanvasGroup cg;

    /// <summary>
    /// �\������J�[�h�I�u�W�F�N�g
    /// </summary>
    private GameObject cardObj;

    /// <summary>
    /// �\���ʒu
    /// </summary>
    [SerializeField]private Transform center;

  
    private void Awake()
    {
        Debug.Log(this.GetType());

        cg = this.GetComponent<CanvasGroup>();

        //�ʏ�͔�\��
        Show(false);
        Debug.Log(this.GetType()+"end");

    }

    public void Initialize(GameObject cardObj)
    {
        this.cardObj = cardObj;
        cardObj.transform.SetParent(this.transform);

        //�J�[�h�̓N���b�N�s�ɂ���
        cardObj.GetComponent<CanvasGroup>().blocksRaycasts = false;

        //�J�[�h�̈ʒu�𒲐�
        cardObj.transform.position = center.position;

        //�g��
        cardObj.GetComponent<RectTransform>().localScale= new Vector3(3,3,3);

        //�\��
        Show(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //�N���b�N���ꂽ�Ƃ��ɂ��̉�ʂ����
        Destroy(cardObj);
        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }

    /// <summary>
    /// ��\���ɂ���
    /// </summary>
    private void Show(bool show)
    {
        cg.alpha = show ? 1 : 0; ;
        cg.blocksRaycasts = show;
    }
}

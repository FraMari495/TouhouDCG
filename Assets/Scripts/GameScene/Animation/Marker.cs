using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{

    /// <summary>
    /// �J�[�h�̃A�j���[�V�������s���ۂɁA�ړ���̖ڈ�Ƃ��Ďg�p
    /// (���̃Q�[���I�u�W�F�N�g�߂����ăA�j���[�V��������)
    /// </summary>
    internal class Marker : MonoBehaviour
    {
        /// <summary>
        /// ���̃I�u�W�F�N�g��Transform���i�[
        /// �v�Z�ʍ팸�̂���
        /// </summary>
        private Transform Trn { get; set; }

        /// <summary>
        /// �����̃I�u�W�F�N�g���A���̃I�u�W�F�N�g�̎q�I�u�W�F�N�g�Ƃ���
        /// (�����̃I�u�W�F�N�g��localPosition�� zero vector�Ƃ���ƁA���҂̈ʒu���d�Ȃ�)
        /// </summary>
        /// <param name="obj"></param>
        internal void Initialize(GameObject obj)
        {
            //���̃I�u�W�F�N�g��Transform���i�[
            Trn = this.transform;

            //�ړ��I�u�W�F�N�g���A���̃I�u�W�F�N�g�̎q�Ƃ���
            obj.transform.SetParent(Trn, true);

            //marker�̃T�C�Y���A�ړ����������I�u�W�F�N�g�̉摜�T�C�Y�ɂ��킹��
            this.GetComponent<RectTransform>().sizeDelta = obj.GetComponent<RectTransform>().sizeDelta;

            StartCoroutine(CheckChild());
        }

        /// <summary>
        /// �ړ��I�u�W�F�N�g�����̃I�u�W�F�N�g�̎q����O�ꂽ�ꍇ�A���̃I�u�W�F�N�g���폜����
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckChild()
        {
            yield return new WaitWhile(() => Trn.childCount > 0);
            Destroy(this.gameObject);
        }
    }
}

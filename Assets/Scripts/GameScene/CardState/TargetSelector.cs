using Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class TargetSelector
{

    /// <summary>
    /// �v���C���[�ɑI������^����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal IEnumerator StartSelection(StatusBase owner, CardType[] targetType, Condition condition, Action<StatusBase> onTargetDecided)
    {
        //�v���C���[�ɑI������^���Ă���Ԃ̓^�[���I���{�^���𖳌��ɂ���
        ReactivePropertyList.I.Wait(true);

        if (!CheckConsistency(targetType)) yield break;

        //�I����Ԃ̊Ԃ́A�J�[�h�𑀍�ł��Ȃ��悤�ɂ���
        CommandManager.I.Wait();

        var playerCards = CommandManager.I.GetCards(true, PosEnum.Field).Where(c => targetType.Contains(c.Type)).ConvertType<StatusBase>().ToList();
        var rivalCards = CommandManager.I.GetCards(false, PosEnum.Field).Where(c => targetType.Contains(c.Type)).ConvertType<StatusBase>().ToList();



        //�����ɍ������J�[�h�݂̂ɍi��
        List<StatusBase> targetsOption = condition.DecideOption(owner, playerCards, rivalCards);

        //�I�����ʂ�ێ�����ϐ�
        StatusBase decided = null;
        
        if (targetsOption.Count > 1)
        {
            //�I������2�ȏ�Ȃ�A�v���C���[�ɑI��������

            //�I�����ꂽ�J�[�h�����̃��X�g�ɓ����
            List<StatusBase> SelectedTarget = new List<StatusBase>();
            targetsOption.ConvertAll(target => target.StartCoroutine(target.GetComponentInChildren<Target>().SetTarget(SelectedTarget)));

            //�I�����̒��������I�΂��܂őҋ@
            yield return new WaitWhile(() => SelectedTarget.Count==0);
            decided = SelectedTarget[0];
        }
        else if (targetsOption.Count == 1)
        {
            //�I������1�Ȃ玩���I�ɂ����I��
            decided = targetsOption[0];
        }

        //�I�������̃R�[���o�b�N���\�b�h�����s
        onTargetDecided(decided);

        //�I�����I���������߁A�^�[���I���{�^�����ēx�L���ɂ���
        ReactivePropertyList.I.Wait(false);
    }


    private bool CheckConsistency(CardType[] targetType)
    {
        if (targetType == null)
        {
            Debug.LogError("�^�[�Q�b�g���ݒ肳��Ă��܂���");
            Debug.LogError("OnPlayAbility��Target��NoTarget�ł͂Ȃ��Ƃ��AtargetType�͕K���ݒ肵�Ă�������");
            return false;
        }
        return true;
    }
}

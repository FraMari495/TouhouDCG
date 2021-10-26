using Position;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class OnPlayAbility: AbilityBase
{
    /// <summary>
    /// �A�r���e�B�[�̃^�[�Q�b�g��I���ł��邩�ۂ�
    /// </summary>
    public bool TargetRequired => Condition.Target != TargetEnum.NoTarget;

    /// <summary>
    /// �v���C���A�r���e�B�[�̏ꍇ�A�v���C���[���Ώ̂��w�肷�邱�Ƃ��ł���
    /// </summary>
    [SerializeField] private Condition condition;

    /// <summary>
    /// �Ώ̂ƂȂ肤��J�[�h�̏���
    /// </summary>
    public Condition Condition => condition;

    /// <summary>
    /// ���̃A�r���e�B�[�̓v���C���ɔ�������
    /// </summary>
    public override AbilityType AbilityType => AbilityType.OnPlay;

    /// <summary>
    /// Skill()�̈����ɓ���J�[�h�̃^�C�v
    /// </summary>
    public abstract CardType[] TargetType { get; }

    /// <summary>
    /// �A�r���e�B�[�𔭓�����O�ɁA�Ώ̂�I������
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    public int[] Run(StatusBase owner,StatusBase Target, int[] indices) =>RunAbility(owner, Target,indices);

    /// <summary>
    /// ��̓I�ȃA�r���e�B�[�̒��g
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="targets"></param>
    protected abstract int[] RunAbility(StatusBase owner, StatusBase targets, int[] indices);

    /// <summary>
    /// for AI
    /// ���̃A�r���e�B�[���g�p���čő�X�R�A�𓾂���@
    /// (�^�[�Q�b�g��N�ɂ��ׂ��� & �X�R�A�̎擾)
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    public PredictedScore Predict(StatusBase owner)
    {

        if (condition.Target != TargetEnum.NoTarget)
        {
            //�I�������^������悤�ȃA�r���e�B�[�̏ꍇ

            #region �I���\�ȃJ�[�h�����肷��
            //�t�B�[���h��́A�v���C���[�̃J�[�h�S��
            var playerCards = Field.I(true).GetStatus(TargetType);

            //�t�B�[���h��́A����̃J�[�h�S��
            var rivalCards = Field.I(false).GetStatus(TargetType);

            //�I���\�ȃJ�[�h�S��
            var options = condition.DecideOption(owner, playerCards, rivalCards);
            #endregion

            #region �X�R�A�̍ő剻

            float maxScore = -(int)1e8;
            StatusBase target = null;

            //���ׂĂ̑I����������
            foreach (var item in options)
            {
                //�X�R�A���v�Z��
                float score = CalculateScore(owner, item);

                //�ő�l���X�V
                if (maxScore < score)
                {
                    target = item;
                    maxScore = score;
                }
            }
            #endregion

            return new PredictedScore(target, maxScore);
        }
        else
        {
            //�I�����̂Ȃ��A�r���e�B�[�̏ꍇ
            return new PredictedScore(null, CalculateScore(owner, null));
        }
    }

    /// <summary>
    /// for AI
    /// �A�r���e�B�[�̕]�����v�Z
    /// </summary>
    /// <param name="owner">�A�r���e�B�[�̎g�p��</param>
    /// <param name="target">�A�r�e�B�[�^�[�Q�b�g</param>
    /// <returns></returns>
    protected abstract float CalculateScore(StatusBase owner,StatusBase target);


}


/// <summary>
/// AI�̕]���̂܂Ƃ�
/// ���̃X�L���𔭓������ꍇ�A�l������X�R�A���ő�Ƃ���悤�ȃ^�[�Q�b�g�Ƃ��̃X�R�A
/// </summary>
public class PredictedScore
{
    public PredictedScore(StatusBase target,float score)
    {
        Target = target;
        Score = score;
    }

    /// <summary>
    /// �X�R�A���ő剻�����^�[�Q�b�g
    /// </summary>
    public StatusBase Target { get; }

    /// <summary>
    /// �ő剻���ꂽ�X�R�A
    /// </summary>
    public float Score { get; } = 0;

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

namespace Position
{
    public class Field : PositionBase<Field>
    {
        protected override PosEnum Pos => PosEnum.Field;

        /// <summary>
        /// �t�B�[���h�̃J�[�h�̂����ACardType�^�̃J�[�h��Ԃ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<StatusBase> GetStatus(CardType[] type)
        {
            //type�^�̃J�[�h�𔲂��o��
            List<StatusBase> list = Cards.ConvertType<StatusBase>().Where(x => type.Contains(x.Type)).ToList();

            //�q�[���[���I���\�Ȃ��list�ɒǉ�����
            if (type.Contains(CardType.Hero)) list.Add(Status_Hero.I(IsPlayer));

            return list;
        }


        /// <summary>
        /// �����̃J�[�h���AField�̉��Ԗڂɔz�u����Ă��邩
        /// </summary>
        /// <param name="statusBase"></param>
        /// <returns></returns>
        public int GetIndex(StatusBase statusBase)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                if (Cards[i] as StatusBase == statusBase)
                {
                    return i;
                }
            }
            Debug.LogError($"{statusBase}��Field�Ɍ�����܂���ł���");
            return -1;
        }

        /// <summary>
        /// �J�[�h���J�[�h���U��
        /// </summary>
        /// <param name="attackerId">�A�^�b�J�[�̈ʒu</param>
        /// <param name="targetId">�^�[�Q�b�g�̈ʒu</param>
        public bool Attack(int attackerId, int targetId)
        {
            //PlayableId����J�[�h�����
            var attacker = (Status_Chara)PlayableIdManager.I.GetPlayableById(attackerId);
            var target = (Status_Chara)PlayableIdManager.I.GetPlayableById(targetId);

            var allFieldCards = new List<IPlayable>();
            allFieldCards.AddRange(Field.I(true).Cards);
            allFieldCards.AddRange(Field.I(false).Cards);

            //�f�o�b�O(�A�^�b�J�[��IsPlayable�łȂ���΂Ȃ�Ȃ��B���҂��t�B�[���h�ɑ��݂��邩�m�F)
            if (!allFieldCards.Contains(attacker) || !allFieldCards.Contains(target))
            {
                return false;
            }

            //�J�[�h����Data_Chara�N���X(�J�[�h�̏�Ԃ�\������N���X)�𒊏o
            Status_Chara.Data_Chara attackerData = attacker.CharaData;
            Status_Chara.Data_Chara targetData = target.CharaData;

           �@
            //�U���A�j���[�V����(�s�ӑł��̏ꍇ�̓A�j���[�V�������قȂ�)
            bool isSurpriseAttack = attackerData.MyAbilities.HaveSpecialStatus(SpecialStatus.Surprise);
            if (!isSurpriseAttack) AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.AttackAnimation(attacker), "�U���A�j���[�V����");
            else AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.LockOnAnimation(attacker, target), "�U���A�j���[�V����");

            ChangeStatus(attackerData, targetData, Status_Hero.I(!attackerData.IsPlayer));

            return true;
        }

        private void ChangeStatus(Status_Chara.Data_Chara attackerData, Status_Chara.Data_Chara targetData,Status_Hero rival_Hero)
        {

            //�A�^�b�J�[�̎c��̍U���񐔂����炷
            attackerData.AttackNum--;

            //�U�����A�^�����_���[�W���擾
            int damageT = targetData.DamageHp(attackerData.AtkData, attackerData.MyAbilities.HaveSpecialStatus(SpecialStatus.Killer));
            if (attackerData.MyAbilities.HaveSpecialStatus(SpecialStatus.Killer) && damageT > 0)
            {
                targetData.AddStatusEffect(StatusEffect.Dead);
                //Debug.LogError("��Ԉُ�̃A�j���[�V����");
            }

            //�s�ӑł��ł͂Ȃ��Ȃ甽��
            bool isSurpriseAttack = attackerData.MyAbilities.HaveSpecialStatus(SpecialStatus.Surprise);
            if (!isSurpriseAttack)
            {
                int damageA = attackerData.DamageHp(targetData.AtkData, targetData.MyAbilities.HaveSpecialStatus(SpecialStatus.Killer));
                if (targetData.MyAbilities.HaveSpecialStatus(SpecialStatus.Killer) && damageA > 0)
                {
                    attackerData.AddStatusEffect(StatusEffect.Dead);
                   // Debug.LogError("��Ԉُ�̃A�j���[�V����");
                }
            }

            //����
            if (attackerData.MyAbilities.HaveSpecialStatus(SpecialStatus.Freeze))
            {
                targetData.AddStatusEffect(StatusEffect.Freeze);
                //Debug.LogError("��Ԉُ�̃A�j���[�V����");
            }

            //�ђ�
            if (attackerData.MyAbilities.HaveSpecialStatus(SpecialStatus.Penetrate))
            {
                //Ai��rival_Hero�Ƃ���null��������B����C�� ( = ����AAi�͊ђʍU�����l���Ȃ�)
                if (rival_Hero!=null) rival_Hero.CardData.DamageHp(damageT / 2);
            }
        }

        /// <summary>
        /// AI�̍s������̂��߁A�U���̃V�~�����[�V�������s��
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="target"></param>
        public void AttackForAISimulation(Status_Chara.Data_Chara attacker, Status_Chara.Data_Chara target)
        {
            //����AI�͊ђʍU�����l���Ȃ����߁A��3������null
            ChangeStatus(attacker, target,null);
        }

        /// <summary>
        /// �q�[���[���U������
        /// </summary>
        /// <param name="attackerPos"></param>
        /// <returns></returns>
        public bool AttackHero(int attackerPos)
        {

            if ((Status_Chara)PlayableIdManager.I.GetPlayableById(attackerPos) is Status_Chara attacker)
            {
                //�f�o�b�O(�A�^�b�J�[��IsPlayable�łȂ���΂Ȃ�Ȃ��B���҂��t�B�[���h�ɑ��݂��邩�m�F)
                if (!attacker.IsPlayable || !Field.I(attacker.IsPlayer).Cards.Contains(attacker))
                {
                    return false;
                }

                attacker.CharaData.AttackNum--;
                Status_Hero hero = Status_Hero.I(!IsPlayer);

                //�U���̃A�j���[�V����(�s�ӑł��Ƃ��̑��ŃA�j���[�V�������قȂ�)
                bool isSurpriseAttack = attacker.CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Surprise);
                if (!isSurpriseAttack) AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.AttackAnimation(attacker), "�U���A�j���[�V����");
                else AnimationManager.I.AddSequence(() => AnimationManager.I.AnimationMaker.LockOnAnimation(attacker, hero), "�U���A�j���[�V����");

                //�_���[�W��^����
                hero.CardData.DamageHp(attacker.Atk);
                return true;
            }
            else
            {
                Debug.LogError($"{(Status_Chara)PlayableIdManager.I.GetPlayableById(attackerPos)}�̓L�����N�^�[�J�[�h�ł͂���܂���");
                return false;
            }
        }

        /// <summary>
        /// ���S���� & �s���\������
        /// �A�^�b�J�[�̑I����
        /// </summary>
        protected override void Judge()
        {
            //���S����
            foreach (var card in new List<IPlayable>(Cards))
            {

                //�L�����N�^�[�̃X�e�[�^�X���Q��
                Status_Chara chara = card.GameObject.GetComponent<Status_Chara>();

                //���S����
                if (chara != null && chara.CharaData.StatusEffects.Contains(StatusEffect.Dead))
                {
                    //���S������Judge����Ȃ�
                    //�������Ֆʂɂ�(�A�j���[�V��������)�c�邽�߁A���ʒ��O��Playable = false;������
                    chara.IsPlayable = false;
                    DeadCard(chara);
                }


            }

            //�s���\������
            foreach (var card in Cards)
            {
                if (card is Status_Chara chara)
                {
                    //�U���\����
                    bool condition1 = TurnManager.I.Turn == IsPlayer; //�����̃^�[��
                    bool condition2 = chara.CharaData.AttackNum > 0; //�U���\�񐔂��c���Ă��邩
                    chara.IsPlayable = condition1 && condition2;
                }
            }

            // Cards.ForEach(c => c.GameObject.GetComponent<CardInputHandler>().UpdatePlayableAura());

            ReactivePropertyList.I.UpdatePlayableAura(IsPlayer);

        }

        /// <summary>
        /// �J�[�h�����S�����Ƃ��̏���
        /// </summary>
        /// <param name="status"></param>
        private void DeadCard(Status_Chara status)
        {
            status.RunOnDefeatedSkill();
            Move(status, Discard.I(status.IsPlayer), Discard.I(status.IsPlayer).Cards.Count);
        }

        public bool SpecialSummon(int posIndex,IPlayable playable)
        {
            //�f�[�^��Ńt�B�[���h�ɒǉ�
            if (AddCard(posIndex, playable))
            {
                //�|�W�V�����ύX��ʒm(�A�j���[�V����)
                playable.UpdatePosition.OnNext(( PosEnum.None, Pos, posIndex));
                return true;
            }

            return false;
        }

        /// <summary>
        /// �^�[���J�n���ɌĂ΂��
        /// </summary>
        protected override void OnBeginTurn()
        {
            //Status_Chara�^�̃J�[�h���ׂĂ�T���AResetAttackNum�����s
            Cards.ToList().ConvertAll(c => c.GameObject.GetComponent<Status_Chara>()).NonNull().ForEach(x => x.ResetAttackNum());
        }

        public void OnTurnEnd()
            => Cards.ConvertType<Status_Chara>().NonNull().ForEach(chara => chara.RunOnTurnEndSkill());
        

        public bool ExistGardian()
            => Cards.ConvertType<Status_Chara>().NonNull().Any(c => c.CharaData.MyAbilities.HaveSpecialStatus(SpecialStatus.Gardian));



    }
}

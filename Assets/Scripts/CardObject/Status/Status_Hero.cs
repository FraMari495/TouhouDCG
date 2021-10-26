using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �q�[���[�̃X�e�[�^�X
/// �J�[�h�͌����ڂƃf�[�^�ŃN���X�𕪂������A�q�[���[�̏ꍇ�͓����N���X���ŏ���
/// </summary>
public class Status_Hero : Status
{
    /// <summary>
    /// �V���O���g���p�^�[�����A�C���X�^���X��2�̏ꍇ�Ɋg��
    /// </summary>
    #region MonoPair

    private static Dictionary<bool, Status_Hero> iMap;

    public static Status_Hero I(bool isPlayer)
    {
        if (iMap == null)
        {
            Status_Hero[] i = FindObjectsOfType<Status_Hero>();
            if (i.Length == 2)
            {
                iMap = new Dictionary<bool, Status_Hero>()
                { { i[0].IsPlayer, i[0] }, { i[1].IsPlayer, i[1] } };
            }
            else
            {
                Debug.LogError(typeof(Status_Hero).ToString() + "�̃C���X�^���X��" + i.Length + "���݂��܂��B2�ɏC�����Ă�������");
                return null;
            }
        }
        return iMap[isPlayer];

       
    }

    private void OnDestroy()
    {
        iMap = null;
    }
    #endregion


    [SerializeField]private Image charaImage;

    [SerializeField] private Sprite charaSprite1;
    [SerializeField] private Sprite charaSprite2;

    [SerializeField] private Sprite charaSprite1win;
    [SerializeField] private Sprite charaSprite2win;

    [SerializeField] private Sprite charaSprite1lose;
    [SerializeField] private Sprite charaSprite2lose;

    [SerializeField] private GameObject resultPrefab;
    [SerializeField] private SceneInitializer sceneInitializer;




    [SerializeField] private TextMeshProUGUI hpText_field;
    public override CardType Type => CardType.Hero;

    public string CardName { get; private set; }

    protected override string ObjectName => CardName;

    public Data_Hero HeroData => (Data_Hero)CardData;

    public void Initialize(bool isPlayer)
    {
        IsPlayer = isPlayer;

        if (ConnectionManager.Instance.IsFirstAttack)
        {
            charaImage.sprite = IsPlayer? charaSprite1:charaSprite2;
        }
        else
        {
            charaImage.sprite = IsPlayer ? charaSprite2 : charaSprite1;
        }

        int maxHp = sceneInitializer.IsTutorial ? 4 : 20;
        CardData = new Data_Hero(IsPlayer,this.transform, maxHp, maxHp, new TextMeshProUGUI[1] { hpText_field }, Dead);
        UpdateHpUI.OnNext(maxHp);
        CardName = IsPlayer?"player_hero":"rival_hero";

    }

    /// <summary>
    /// �q�[���[�����S�����ۂɍs���鏈��
    /// ��ɃA�j���[�V�����̍쐬���s��
    /// </summary>
    protected override void Dead()
    {
        AudioClip winClip = Resources.Load<AudioClip>("WinClip");
        AudioClip loseClip = Resources.Load<AudioClip>("LoseClip");

        AnimationManager.I.AddSequence(() => DOTween.Sequence()
        .AppendCallback(()=>SoundManager.I.FadeOut())
        .AppendInterval(1)
        .AppendCallback(()=>SoundManager.I.PlaySE(IsPlayer? loseClip : winClip))
        .AppendCallback(() =>
        {
            AnimationManager.I.GameOver = true;

            ResultManager result = Instantiate(resultPrefab, GameObject.Find("Canvas").transform).GetComponent<ResultManager>();

            if (!IsPlayer)
            {
                Sprite myChara = ConnectionManager.Instance.IsFirstAttack ? charaSprite1win : charaSprite2win;
                Sprite rivalChara = ConnectionManager.Instance.IsFirstAttack ? charaSprite2lose : charaSprite1lose;

                result.GameSet(true, myChara, rivalChara);

            }
            else
            {
                Sprite myChara = ConnectionManager.Instance.IsFirstAttack ? charaSprite1lose : charaSprite2lose;
                Sprite rivalChara = ConnectionManager.Instance.IsFirstAttack ? charaSprite2win : charaSprite1win;

                result.GameSet(false, myChara, rivalChara);

            }
        }), "�������");
    }




    public class Data_Hero : Data
    {
        public Data_Hero(bool isPlayer,Transform trn,int currentHp, int maxHp, TextMeshProUGUI[] hpTexts, Action onDead) : base(isPlayer,trn,currentHp, maxHp, onDead)
        {
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Position;
using DG.Tweening;
using UniRx;

public class TurnManager : MonoSingleton<TurnManager>
{
    [SerializeField] private AudioClip intro;
    [SerializeField] private AudioClip main;
    [SerializeField] private TurnEndButton buttonImage;
    [SerializeField] private SceneInitializer sceneInitializer;
    [SerializeField] private ChoicingPanel choicingPanel;    

    private bool FirstAttack => ConnectionManager.Instance.IsFirstAttack;
    private bool firstTurn = true;

    /// <summary>
    /// �^�[���`�F���W��ʒm
    /// </summary>
    public Subject<bool> NewTurnNotif { get; } = new Subject<bool>();

    /// <summary>
    /// �W���b�W�J�n
    /// </summary>
    public Subject<Unit> Judge { get; } = new Subject<Unit>();


    /// <summary>
    /// �v���C���[�̃^�[�����ۂ�
    /// </summary>
    private bool turn;

    /// <summary>
    /// �^�[��
    /// </summary>
    public bool Turn
    {
        get => turn;
        set
        {
            turn = value;

            //�^�[���`�F���W�̃e�L�X�g�A�j���[�V����
            AnimationManager.I.AddSequence(()=>AnimationMaker.TurnEndAnimation(buttonImage,value), "�^�[���`�F���W");

            //��U��1�^�[���ڂ̓h���[�ł��Ȃ�
            bool canDraw = true;
            if (firstTurn)
            {
                firstTurn = false;
                canDraw = false;
            }

            //�^�[���`�F���W��ʒm
            NewTurnNotif.OnNext(value);

            //�I�t���C�����[�h�ő���̃^�[���̏ꍇ�AAI�𓮂���
            if (ConnectionManager.Instance.OfflineMode && !value)
            {
                if(sceneInitializer.Tutorial==null) AI.I.StartAI(canDraw,sceneInitializer.IsTutorial);
                else sceneInitializer.Tutorial.RunRivalCommand();
            }

            //���[�U�[�̓��͂̒��O
            StartJudge(false);
        }
    }

    /// <summary>
    /// �^�[���`�F���W�̃e�L�X�g�A�j���[�V���� & �^�[���`�F���W
    /// </summary>
    /// <param name="isPlayer"></param>
    private void TurnChangeAnimation(bool isPlayer)
    {
        AnimationManager.I.AddSequence(() => DOTween.Sequence().AppendCallback(() => Turn = isPlayer),"�^�[���`�F���W");
    }

    /// <summary>
    /// �^�[���`�F���W
    /// </summary>
    /// <returns></returns>
    public bool ChangeTurn()
    {
        //�^�[���I���̃X�L���𔭓�
        Field.I(Turn).OnTurnEnd();

        //�^�[���`�F���W�̃A�j���[�V���� & �^�[���`�F���W
        TurnChangeAnimation(!Turn);

        return true;
    }

    /// <summary>
    /// �Q�[���J�n
    /// </summary>
    /// <param name="myTurn"></param>
    public void GameStart(bool myTurn)
    {
        //�^�[���`�F���W�̃A�j���[�V���� & �^�[���`�F���W
        TurnChangeAnimation(myTurn);

        //BGM�̍Đ�
        SoundManager.I.Play(intro,main);
    }

    /// <summary>
    /// �f�b�L���\����������
    /// 6���I��ŕ\�����A3����I��
    /// </summary>
    /// <param name="myTurn"></param>
    /// <param name="selectedCards"></param>
    /// <returns></returns>
    public IEnumerator Mulligan(List<IPlayable> options)
    {
        //�}���K���̊J�n
        //�I�����ɁADecidedFiestHand()���\�b�h�����s�����
        choicingPanel.gameObject.SetActive(true);
        yield return choicingPanel.StartSelecting(options, 3, Deck.I(true).DecidedFiestHand);
    }

    /// <summary>
    /// !!!���[�U�[�̓��͒��O�ɕK���Ă�!!!
    /// ���S����A�v���C�\�ȃJ�[�h�̔���A�A�^�b�J�[�̑I������
    /// </summary>
    public void StartJudge(bool rpc)
    {
        if(!rpc)ConnectionManager.Instance.Judge();
        Judge.OnNext(Unit.Default);
    }

}

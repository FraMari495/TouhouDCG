using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// ゲームセットの画面のデザイン
/// </summary>
public class ResultManager : MonoBehaviour
{
    [SerializeField] private Status_Hero[] heroes;

    [SerializeField] private AudioClip enterClip;
    [SerializeField] private GameObject youwinText;
    [SerializeField] private GameObject youloseText;
    [SerializeField] private Image myChara;
    [SerializeField] private Image rivalChara;

    [SerializeField] private Sprite charaSprite1win;
    [SerializeField] private Sprite charaSprite2win;

    [SerializeField] private Sprite charaSprite1lose;
    [SerializeField] private Sprite charaSprite2lose;

    [SerializeField] private GameObject resultPrefab;

    [SerializeField] private GameObject animationObject;


    private void Start()
    {
        Array.ForEach(heroes, h => h.HeroDead.Subscribe(isPlayer=> {

            AnimationManager.I.AddSequence(() => DOTween.Sequence().AppendCallback(() => HeroDead(isPlayer)),"ゲーム終了");
        
        }));
        animationObject.SetActive(false);
    }

    public void GameSet(bool win,Sprite myChara,Sprite rivalChara)
    {
        this.myChara.sprite = myChara;
        this.rivalChara.sprite = rivalChara;

        (win ? youloseText : youwinText).SetActive(false);
    }

    /// <summary>
    /// ホームへ戻るボタンがクリックされた際に呼ばれる
    /// </summary>
    public void GoTitleButtonClicked()
    {
        SoundManager.I.PlaySE(enterClip);

        ConnectionManager.Instance.CloseRoom();

        WBTransition.SceneManager.LoadScene("Lobby");
    }

    private void HeroDead(bool isPlayerDead)
    {
        animationObject.SetActive(true);

        AudioClip winClip = Resources.Load<AudioClip>("WinClip");
        AudioClip loseClip = Resources.Load<AudioClip>("LoseClip");

        AnimationManager.I.AddSequence(() => DOTween.Sequence()
        .AppendCallback(() => SoundManager.I.FadeOut())
        .AppendInterval(1)
        .AppendCallback(() => SoundManager.I.PlaySE(isPlayerDead ? loseClip : winClip))
        .AppendCallback(() =>
        {
            AnimationManager.I.GameOver = true;

            ResultManager result = this;// Instantiate(resultPrefab, GameObject.Find("Canvas").transform).GetComponent<ResultManager>();

            if (!isPlayerDead)
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
        }), "勝利画面");
    }
}

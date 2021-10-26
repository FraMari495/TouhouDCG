using Player;
using DG.Tweening;
using Position;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アニメーションを作成するクラス
/// </summary>
public static class AnimationMaker
{
    private static AudioClip GetClip(string name)
    {
        return Resources.Load<AudioClip>(name);
    }

    public static Sequence TurnEndAnimation(TurnEndButton turnEndButton,bool show)
    {


            GameObject turnChangeObj = GameObject.Instantiate(Resources.Load<GameObject>("TurnChangePanel"), GameObject.Find("Canvas").transform);
        TurnChangeObject turnChange = turnChangeObj.GetComponent<TurnChangeObject>();


        Sequence sequence = DOTween.Sequence()
            .AppendCallback(()=>SoundManager.I.PlaySE(GetClip("TurnChange_nc168168")))
            .AppendInterval(0.1f)
            .AppendCallback(() => turnChange.Play(show))
            .AppendInterval(1f)
            .AppendCallback(() => {
                turnEndButton.Show(show);
                GameObject.Destroy(turnChangeObj);
        });

        return sequence;
    }

    public static Sequence DamageAnimation(int damage,int newHpValue, Transform trn,bool killer)
    {
        

        GameObject damageObj = GameObject.Instantiate(Resources.Load<GameObject>("DamageEffect"), trn);

        return DOTween.Sequence()
            .AppendCallback(() =>
            {
                damageObj.GetComponent<DamageEffect>().Initialize(damage, killer);
                SoundManager.I.PlaySE(GetClip("Damage"));
               // hpTexts.ForEach(hpText=>hpText.text = newHpValue.ToString());
                trn.GetComponent<Status>().UpdateHpUI.OnNext(newHpValue);
            })
            .AppendInterval(1)
            .AppendCallback(() => GameObject.Destroy(damageObj));
            
    }

    public static Sequence AtkDownAnimation(int damage, int newAtkValue, Status_Chara chara)
    {
        // GameObject damageObj = GameObject.Instantiate(Resources.Load<GameObject>("DamageEffect"), status.transform);

        return DOTween.Sequence()
            .AppendCallback(() =>
            {
                //   damageObj.GetComponent<DamageEffect>().Initialize(damage);
                //atkTexts.ForEach(atkText => atkText.text = newAtkValue.ToString());
                chara.UpdateAtkUI.OnNext(newAtkValue);
            })
            .AppendInterval(1);
           // .AppendCallback(() => GameObject.Destroy(damageObj));

    }

    public static Sequence CostDownAnimation(int down, int newCostValue, IPlayable chara, TextMeshProUGUI[] costTexts)
    {
        // GameObject damageObj = GameObject.Instantiate(Resources.Load<GameObject>("DamageEffect"), status.transform);

        return DOTween.Sequence()
            .AppendCallback(() =>
            {
                //   damageObj.GetComponent<DamageEffect>().Initialize(damage);
                costTexts.ForEach(costText => costText.text = newCostValue.ToString());
                chara.UpdateCostUI.OnNext(newCostValue);
            })
            .AppendInterval(1);
        // .AppendCallback(() => GameObject.Destroy(damageObj));

    }

    public static Sequence AttackAnimation(Status status)
    {
        Vector3 punch = new Vector3(0, 100, 0) * (status.IsPlayer ? 1 : -1);
        Sequence s = DOTween.Sequence()
            .AppendCallback(()=> status.GetComponent<CardInputHandler>().UpdatePlayableAura())
            .Append(status.transform.DOPunchPosition(punch, 0.4f,1,snapping:true));
        return s;
    }

    public static Sequence DrawAnimation(IPlayable playable)
    {


        CardInputHandler drawingCard = playable.GameObject.GetComponent<CardInputHandler>();

        PosEnum from = drawingCard.Pos;


        //フィールドの子オブジェクトとしてマーカーを配置
        Transform marker = GameObject.Instantiate(Resources.Load<GameObject>("Marker"), Hand.I(drawingCard.IsPlayer).transform).transform;
        marker.localPosition = Vector3.zero;
        //カードのTransform
        Transform trn = drawingCard.transform;


        marker.GetComponent<Marker>().Initialize(trn.gameObject);

        SoundManager.I.PlaySE(GetClip("Draw"));
        if (drawingCard.IsPlayer && from == PosEnum.Deck)
        {
            Sequence s = DOTween.Sequence()
                .Append(trn.DOLocalRotate(new Vector3(0, 90, 0), 0.3f))
                .AppendCallback(() => drawingCard.ChanegCardView( PosEnum.Hand))
                .Append(trn.DOLocalRotate(new Vector3(0, 0, 0), 0.3f));

            return DOTween.Sequence()
                .Append(trn.DOLocalMove(Vector3.zero, 0.6f)).SetEase(Ease.OutQuad)
                .Join(trn.DOScale(new Vector3(1,1,1),0.6f))
                .Join(s);
        }
        else
        {
            return DOTween.Sequence()
                .AppendCallback(() => drawingCard.ChanegCardView(PosEnum.Hand))
                .Join(trn.DOScale(new Vector3(1, 1, 1), 0.6f))
                .Join(trn.DOLocalMove(Vector3.zero, 0.6f)).SetEase(Ease.OutQuad);
        }
    }

    public static Sequence PlayAnimation(IPlayable playable,int pos,Transform center)
    {
        GameObject markerPrefab = Resources.Load<GameObject>("Marker");

        if (playable.IsPlayer)
        {
            //フィールドの子オブジェクトとしてマーカーを配置
            Transform marker = GameObject.Instantiate(markerPrefab, Field.I(playable.IsPlayer).transform).transform;
            marker.SetSiblingIndex(pos);

            //カードのTransform
            Transform trn = playable.GameObject.transform;
            trn.SetParent(center);


            //アニメーションを作成
            Sequence s1 = DOTween.Sequence()
                .Append(trn.DOLocalRotate(new Vector3(0, 90, 0), 0.15f))
                .Join(trn.DOMoveX(center.position.x + 200, 0.15f))
                .AppendCallback(() => playable.GameObject.GetComponent<CardInputHandler>().ChanegCardView(PosEnum.Field))
                .Append(trn.DOLocalRotate(new Vector3(0, 0, 0), 0.15f))
                .Join(trn.DOMoveX(center.position.x, 0.15f));

            return DOTween.Sequence()
            .AppendCallback(() =>
            {
                SoundManager.I.PlaySE(GetClip("Pull"));
            })
            .Append(trn.DOMoveY(center.position.y, 0.3f))
            .Join(s1)
            .Join(trn.DOScale(2, 0.3f))
            .AppendInterval(0.5f)
            .AppendCallback(() => marker.GetComponent<Marker>().Initialize(trn.gameObject))
            .Append(trn.DOScale(1, 0.3f))
            .Join(trn.DOLocalMove(Vector3.zero, 0.3f))
            .AppendCallback(() => SoundManager.I.PlaySE(GetClip("Put")));
        }
        else
        {
            //フィールドの子オブジェクトとしてマーカーを配置
            Transform marker = GameObject.Instantiate(markerPrefab, Field.I(playable.IsPlayer).transform).transform;
            marker.SetSiblingIndex(pos);



            //カードのTransform
            Transform trn = playable.GameObject.transform;

            playable.GameObject.GetComponent<CardInputHandler>().ChanegCardView( PosEnum.Field);
            marker.GetComponent<Marker>().Initialize(trn.gameObject);

            //アニメーションを作成
            return DOTween.Sequence()
                .AppendCallback(() => {
                    playable.GameObject.GetComponent<CardInputHandler>().UpdatePlayableAura();
                    SoundManager.I.PlaySE(GetClip("Pull"));
                })
                .Append(trn.DOLocalMove(Vector3.zero, 0.5f));
                //.AppendCallback(()=> CardInfo.I.Show(playable.CardBook, trn.GetComponent<CardVisualController>().GetExampleCard()));
                
        }
    }

    public static Sequence SpellAnimation(IPlayable playable, Transform center)
    {

            //カードのTransform
            Transform trn = playable.GameObject.transform;
            trn.SetParent(center);


            //アニメーションを作成
            Sequence s1 = DOTween.Sequence()
                .Append(trn.DOLocalRotate(new Vector3(0, 90, 0), 0.15f))
                .Join(trn.DOMoveX(center.position.x + 200, 0.15f))
                .AppendCallback(() => playable.GameObject.GetComponent<CardInputHandler>().ChanegCardView(PosEnum.Field))
                .Append(trn.DOLocalRotate(new Vector3(0, 0, 0), 0.15f))
                .Join(trn.DOMoveX(center.position.x, 0.15f));

            return DOTween.Sequence()
            .AppendCallback(() =>
            {
                SoundManager.I.PlaySE(GetClip("Pull"));
            })
            .Append(trn.DOMoveY(center.position.y, 0.3f))
            .Join(s1)
            .Join(trn.DOScale(2, 0.3f))
            .AppendInterval(0.5f);

    }

    public static Sequence DeadAnimation(IPlayable playable,bool makeSound = true)
    {


        Transform trn = playable.GameObject.transform;
        CanvasGroup cg = trn.GetComponent<CanvasGroup>();

        return DOTween.Sequence()
            .AppendCallback(()=> { if (makeSound) SoundManager.I.PlaySE(GetClip("Dead")); })
            .Append(cg.DOFade(0, 0.3f))
            .OnComplete(() => {

                //見た目を捨て札のViewに変更する
                playable.GameObject.GetComponent<CardInputHandler>().ChanegCardView( PosEnum.Discard);

                //カードオブジェクトの移動(親はDiscardにする)
                playable.GameObject.GetComponent<CardInputHandler>().transform.SetParent(Discard.I(playable.IsPlayer).transform, false);
            });

    }

    public static Sequence ToDeckAnimation(IPlayable playable)
    {
        return DOTween.Sequence()
            .AppendCallback(() => {


                //見た目をデッキのViewに変更する
                playable.GameObject.GetComponent<CardInputHandler>().ChanegCardView( PosEnum.Deck);

                //カードオブジェクトの移動(親はDiscardにする)
                playable.GameObject.GetComponent<CardInputHandler>().transform.SetParent(Deck.I(playable.IsPlayer).transform, false);
            });

    }

    public static Sequence LockOnAnimation(Status attacker,Status target)
    {
        float moveSize = 100;
        Transform targetCircle = GameObject.Instantiate(Resources.Load<GameObject>("TargetCircle")).transform;
        targetCircle.SetParent(target.transform);
        targetCircle.localPosition = new Vector3(-moveSize/2,0,0);

        return DOTween.Sequence()
            .AppendCallback(()=>SoundManager.I.PlaySE(GetClip("Lockon_nc154263")))
            .Append(targetCircle.DOLocalMove(new Vector3(-moveSize / 2, -moveSize / 2, 0), 0.3f))
            .Append(targetCircle.DOLocalMove(new Vector3(moveSize / 2, -moveSize / 2, 0), 0.3f))
            .Append(targetCircle.DOLocalMove(Vector3.zero, 0.3f))
            .OnComplete(()=> {
                attacker.GetComponent<CardInputHandler>().UpdatePlayableAura();
                GameObject.Destroy(targetCircle.gameObject);
                });


    }


    public static Sequence SpecialSummon(IPlayable playable, int pos)
    {
        playable.GameObject.GetComponent<CanvasGroup>().alpha = 0;
        return SpecialSummonAnimation(playable, pos);
    }

    private static Sequence SpecialSummonAnimation(IPlayable playable, int pos)
    {
        //フィールドの子オブジェクトとしてマーカーを配置
        Transform marker = GameObject.Instantiate(Resources.Load<GameObject>("Marker"), Field.I(playable.IsPlayer).transform, false).transform;
        marker.SetSiblingIndex(pos);

        Transform trn = playable.GameObject.transform;
        marker.GetComponent<Marker>().Initialize(trn.gameObject);


        GameObject circle = GameObject.Instantiate(Resources.Load<GameObject>("MagicCircle"), marker, false);
        Transform playableObj = playable.GameObject.transform;
        playable.GameObject.GetComponent<CardInputHandler>().ChanegCardView(PosEnum.Field);

        CanvasGroup playableCg = playableObj.GetComponent<CanvasGroup>();
        playableObj.SetParent(marker, false);
        playableObj.transform.localPosition = Vector3.zero;
        playableCg.alpha = 0;

        CanvasGroup circleCg = circle.GetComponent<CanvasGroup>();
        circleCg.alpha = 0;

        Sequence playableAnimation = DOTween.Sequence().Append(playableCg.DOFade(1, 0.3f));

        Sequence anim = DOTween.Sequence()
            .AppendCallback(() => SoundManager.I.PlaySE(GetClip("SummonSE")))
            .Append(circleCg.DOFade(1, 0.3f))
            .AppendInterval(0.5f)
            .Join(playableAnimation)
            .Append(circleCg.DOFade(0, 0.3f))
            .AppendCallback(() => GameObject.Destroy(circle));

        return anim;
    }

    public static Sequence SpecialSummonAnimation_Hand(IPlayable playable)
    {
        Transform initPos = Deck.I(playable.IsPlayer).transform;
        playable.GameObject.GetComponent<CanvasGroup>().alpha = 1;
        playable.GameObject.transform.position = initPos.position;

        return DrawAnimation(playable);
    }
}

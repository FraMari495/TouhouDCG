using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// SEやBGMの再生を担当するクラス
/// </summary>
public class SoundManager : MonoSingleton<SoundManager>
{

    [SerializeField]private AudioSource BGM_Source;
    [SerializeField] private AudioSource BGM_Source_intro;
    [SerializeField] private Transform SeSourcesTrn;
    private Queue<AudioSource> seSourceQueue = new Queue<AudioSource>();

    private float BGM_Volume = 0.5f;
    private float SE_Volume = 0.5f;

    public bool isPlaying => BGM_Source.isPlaying;

    private bool main = false;

    private void Awake()
    {
        SoundManager[] soundManagers = FindObjectsOfType<SoundManager>();
        if(soundManagers.Length == 2)
        {
            Destroy(soundManagers.Where(s=>!s.main).ElementAt(0));
        }

        DontDestroyOnLoad(this.gameObject);
        BGM_Source.volume = BGM_Volume;

        foreach (var item in SeSourcesTrn.GetComponents<AudioSource>())
        {
            seSourceQueue.Enqueue(item);
        }
        main = true;
    }

    /// <summary>
    /// BGMをフェードアウト指せる
    /// </summary>
    public void FadeOut()
    {
        DOTween.To(()=>BGM_Source.volume,(f)=>BGM_Source.volume=f, 0, 0.5f).OnComplete(()=>BGM_Source.Stop());
    }

    /// <summary>
    /// BGMを再生
    /// </summary>
    /// <param name="clip"></param>
    public void Play(AudioClip clip)
    {
        BGM_Source.loop = true;
        BGM_Source.volume = BGM_Volume;
        BGM_Source.clip = clip;
        BGM_Source.Play();
    }

    public void Play(AudioClip intro,AudioClip main)
    {
        Play(main);


        //BGM_Source.volume = BGM_Volume;
        //BGM_Source_intro.volume = BGM_Volume;


        //BGM_Source_intro.clip = intro;
        //BGM_Source.clip = main;

        ////イントロ部分の再生開始
        //BGM_Source_intro.PlayScheduled(AudioSettings.dspTime);

        ////イントロ終了後にループ部分の再生を開始
        //BGM_Source.PlayScheduled(AudioSettings.dspTime + ((float)BGM_Source_intro.clip.samples / (float)BGM_Source_intro.clip.frequency));

        ////StartCoroutine(WaitMain(intro, main));
    }

    private IEnumerator WaitMain(AudioClip intro, AudioClip main)
    {
        BGM_Source.loop = false;
        BGM_Source.volume = BGM_Volume;
        BGM_Source.clip = intro;
        BGM_Source.Play();

        yield return new WaitWhile(() => BGM_Source.isPlaying);
        BGM_Source.loop = true;
        BGM_Source.clip = main;
        BGM_Source.Play();
    }

    /// <summary>
    /// SEを再生
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySE(AudioClip clip)
    {
        var source = seSourceQueue.Dequeue();
        source.volume = SE_Volume;
        source.clip = clip;
        source.Play();
        seSourceQueue.Enqueue(source);
    }
}

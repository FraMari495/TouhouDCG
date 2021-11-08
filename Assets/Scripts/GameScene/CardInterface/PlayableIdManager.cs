using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayableId���Ǘ�����
/// 
/// �ʐM�ΐ�̎��ɖ{�̂𔭊�����
/// �J�[�h�̃C���X�^���X��Id��1��1�Ή����Ă���
/// �ʐM����̔ՖʂƎ����̔Ֆʂ�����ׂ��Ƃ��A�����J�[�h�ɂ͓���Id������U����悤�ɏ������s��
/// 
/// �����̒[���ł͎����̃J�[�h��Id��"����"
/// �ΐ푊���Id�͒ʐM�ɂ��"�󂯎��"
/// </summary>
public class PlayableIdManager : MonoSingleton<PlayableIdManager>
{
    private int nextPlayerId = 1;
    private int nextRivalId = -1;

    /// <summary>
    /// PlayableId�ƃJ�[�h��R�Â��鎫��
    /// </summary>
    Dictionary<int, IPlayable> playableMap = new Dictionary<int, IPlayable>();

    /// <summary>
    /// �ʐM����̔ՖʂƎ����̔Ֆʂ�����ׂ��Ƃ��A�����J�[�h�ɂ͓���Id������U����悤�ɏ������s��
    /// 
    /// �����̒[���ł͎����̃J�[�h��Id��"����"
    /// �ΐ푊���Id�͒ʐM�ɂ��"�󂯎��"
    /// </summary>
    /// <param name="playable">PlayableId���K�v�ȃJ�[�h</param>
    /// <param name="playableId">�ΐ푊��̃J�[�h��Id������U��ۂ�non null</param>
    /// <returns></returns>
    public PlayableId GetId(IPlayable playable,bool firstAttack,int? playableId)
    {
        sbyte modif = (sbyte)(firstAttack ? 1 : -1);

        //playableId��null�łȂ� = �ʐM����̃J�[�h��Id�̏����󂯎����
        if (playableId is int id)
        {
            //�󂯎���������A�f���Ɏ����ɒǉ�
            playableMap.Add(id, playable);

            //�C���N�������g
            if (playable.IsPlayer) nextPlayerId++;
            else nextRivalId--;

            return new PlayableId(id);
        }

        //playableId��null ���� �����̃J�[�h = Id������U��K�v������
        if (playable.IsPlayer)
        {
            //PlayableId�́AnextPlayerId(1,2,3,...)��p���Đ���
            //��U�Ȃ�-1���|���邱�Ƃł��܂����킹��
            PlayableId ans = new PlayableId(nextPlayerId * modif);

            //�����ɓo�^
            playableMap.Add((int)ans, playable);

            //�C���N�������g
            nextPlayerId++;
            return ans;
        }

        //playableId��null ���� ����̃J�[�h = �I�t���C���ΐ�ŁA�����PlayableId������U��
        else
        {
            //PlayableId�́AnextRivalId(-1,-2,-3,...)��p���Đ���
            //��U�Ȃ�-1���|���邱�Ƃł��܂����킹��
            PlayableId ans = new PlayableId(nextRivalId * modif);

            //�����ɓo�^
            playableMap.Add((int)ans, playable);

            //�C���N�������g
            nextRivalId--;
            return ans;
        }

    }

    /// <summary>
    /// Id����J�[�h������
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IPlayable GetPlayableById(int id)
    {
        if (playableMap.ContainsKey(id))
        {
            return playableMap[id];
        }
        else
        {
            Debug.LogError($"id{id}�̃J�[�h�͑��݂��܂���");
            Debug.LogError($"���݂���̂�{string.Join(",", playableMap.Keys)}�ł�");
            return null;
        }
    }
}


/// <summary>
/// �v���~�e�B�u�����b�v����N���X
/// �J�[�h�̃C���X�^���XId
/// </summary>
[System.Serializable]
public class PlayableId
{
    public PlayableId(int id)
    {
        this.id = id;
    }

    /// <summary>
    /// �f�t�H���g�l(id = 0)
    /// �� id = 0������U����J�[�h�͑��݂��Ȃ�
    /// </summary>
    public static PlayableId Default => new PlayableId(0);


    [SerializeField]private int id;
    private int Id => id;


    #region override operator �Ȃ� (==���Z��Equal�Ɋւ��āA�C���X�^���X���قȂ��Ă��Ă�id�������Ȃ�true�ƂȂ�悤�ɕύX)
    public static explicit operator int(PlayableId playableId) => playableId.Id;
    public static bool operator ==(PlayableId a, PlayableId b) => a.Id == b.Id;
    public static bool operator !=(PlayableId a, PlayableId b) => a.Id != b.Id;
    public override bool Equals(object obj)=> (obj is PlayableId id) && Id == id.Id;
    public override int GetHashCode()=> base.GetHashCode();
    public override string ToString()=> Id.ToString();
    #endregion
}
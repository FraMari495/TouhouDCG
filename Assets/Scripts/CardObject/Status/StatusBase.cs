using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class StatusBase : MonoBehaviour
{


    protected abstract string ObjectName { get; }

    public abstract CardType Type { get; }

    public bool IsPlayer { get; protected set; }

    public override string ToString()
    {
        return ObjectName;
    }


}

public interface IPlayable
{
    int GetCost();
    OnPlayAbility OnPlayAbility { get; }
    bool IsPlayable { get; set; }
    GameObject GameObject { get; }
    bool IsPlayer { get; }
    PlayableId PlayableId { get; }
    void RequirePlayableId(int? playableId);
    int CardBookId { get; }
    public void Initialize(bool isPlayer, CardBook cardBook);
    CardBook CardBook { get; }
    Subject<int> UpdateCostUI { get; }
    Subject<(PosEnum from,PosEnum to,int index)> UpdatePosition { get; }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Status_Chara;

public class CharacterForAI
{
    public Data_Chara CharaData { get; }

    public int Hp => CharaData.HpData;
    public int Atk => CharaData.AtkData;
    public bool Dead => CharaData.StatusEffects.Contains(StatusEffect.Dead);



    public CharacterForAI(Data_Chara charaData)
    {
        this.CharaData =Data_Chara.DataCharaForAI(charaData);
    }

}



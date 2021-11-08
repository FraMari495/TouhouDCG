//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public abstract class HistoryBase
//{
//    public HistoryBase(bool isPlayer)
//    {
//        IsPlayer = isPlayer;
//    }

//    public bool IsPlayer { get; }

//    public abstract string GetLog();
//}

//public class History_Draw:HistoryBase
//{
//    public IPlayable DrawedCard { get; }
//    public History_Draw(bool isPlayer,IPlayable drawedCard) : base(isPlayer)
//    {
//        DrawedCard = drawedCard;
//    }

//    public override string GetLog()
//    {
//        return $"{DrawedCard}���h���[���܂���";
//    }
//}

//public class History_PlayCard : HistoryBase
//{
//    public IPlayable PlayedChara { get; }
//    public History_PlayCard(bool isPlayer, IPlayable playedChara) : base(isPlayer)
//    {
//        PlayedChara = playedChara;
//    }

//    public override string GetLog()
//    {
//        return $"{PlayedChara}���v���C���܂���";
//    }
//}

//public class History_CardBattle : HistoryBase
//{
//    public Status_Chara Attacker { get; }
//    public Status_Chara Target { get; }
//    public History_CardBattle(bool isPlayer, Status_Chara attacker,Status_Chara target) : base(isPlayer)
//    {
//        Attacker = attacker;
//        Target = target;
//    }

//    public override string GetLog()
//    {
//        return $"{Attacker}��{Target}���U��";
//    }
//}

//public class History_AttackHero : HistoryBase
//{
//    public Status_Chara Attacker { get; }
//    public History_AttackHero(bool isPlayer, Status_Chara attacker) : base(isPlayer)
//    {
//        Attacker = attacker;
//    }

//    public override string GetLog()
//    {
//        return $"{Attacker}���q�[���[���U��";
//    }
//}

//public class History_CardDamaged : HistoryBase
//{
//    public Status Target { get; }
//    public int Damage { get; }
//    public History_CardDamaged(bool isPlayer, Status target,int damage) : base(isPlayer)
//    {
//        Damage = damage;
//        Target = target;
//    }

//    public override string GetLog()
//    {
//        return $"{Target}��{Damage}�̃_���[�W���󂯂�";
//    }
//}

//public class History_HeroDamaged : HistoryBase
//{
//    public int Damage { get; }
//    public History_HeroDamaged(bool isPlayer, int damage) : base(isPlayer)
//    {
//        Damage = damage;
//    }

//    public override string GetLog()
//    {
//        return $"�q�[���[��{Damage}�̃_���[�W���󂯂�";
//    }
//}


//public class History_NewTurn : HistoryBase
//{
//    public History_NewTurn(bool isPlayer) : base(isPlayer)
//    {
//    }

//    public override string GetLog()
//    {
//        return (IsPlayer?"�v���C���[":"����")+"�̃^�[��";
//    }
//}

//public class History_CardDead : HistoryBase
//{
//    public Status_Chara DeadCard { get; }
//    public History_CardDead(bool isPlayer, Status_Chara deadCard) : base(isPlayer)
//    {
//        DeadCard = deadCard;
//    }

//    public override string GetLog()
//    {
//        return $"{DeadCard}�����S���܂���";
//    }
//}


//public class History_Ability : HistoryBase
//{
//    public StatusBase Owner { get; }
//    public Ability Ability { get; }
//    public History_Ability(bool isPlayer, StatusBase owner,Ability ability) : base(isPlayer)
//    {
//        Owner = owner;
//        Ability = ability;
//    }

//    public override string GetLog()
//    {
//        string timingText = Ability.AbilityType switch
//        {
//            AbilityType.OnDefeated => "���S��",
//            AbilityType.OnPlay => "�v���C��",
//            AbilityType.OnTurnEnd => "�^�[���I����",
//            AbilityType.Bomb => "�{��������",
//            _ => throw new System.NotImplementedException()
//        };
//        return $"{Owner}��{timingText}�X�L���u{Ability.SkillName}�v";
//    }
//}
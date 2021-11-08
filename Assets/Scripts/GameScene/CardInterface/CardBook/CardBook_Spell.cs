using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//namespace Book
//{

    /// <summary>
    /// カード図鑑(スペルカード用)
    /// </summary>
    [CreateAssetMenu(menuName = "Card/SpellCard", fileName = "new SpellCard")]
    public class CardBook_Spell : CardBook
    {

        public override string PrefabPath => "SpellCard";
    }
//}

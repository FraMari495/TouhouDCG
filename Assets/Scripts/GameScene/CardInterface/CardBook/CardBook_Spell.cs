using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//namespace Book
//{

    /// <summary>
    /// �J�[�h�}��(�X�y���J�[�h�p)
    /// </summary>
    [CreateAssetMenu(menuName = "Card/SpellCard", fileName = "new SpellCard")]
    public class CardBook_Spell : CardBook
    {

        public override string PrefabPath => "SpellCard";
    }
//}

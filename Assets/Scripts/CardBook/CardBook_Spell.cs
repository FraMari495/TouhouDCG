using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �J�[�h�}��(�X�y���J�[�h�p)
/// </summary>
[CreateAssetMenu(menuName = "Card/SpellCard",fileName = "new SpellCard")]
public class CardBook_Spell : CardBook
{

    protected override string PrefabPath => "SpellCard";
}

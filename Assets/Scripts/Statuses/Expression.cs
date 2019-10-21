using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Expression
{
    public enum Effects {
        DAMAGE
    }
    
    public enum Modifier {
        ADD,
        MULTIPLY
    }

    
    [HorizontalGroup("Expression"), HideLabel, ValueDropdown("Modifiers")]
    public Modifier modifier;
    [HorizontalGroup("Expression"), HideLabel]
    public int amount;
    [HorizontalGroup("Expression"), HideLabel]
    public Effects effect;

#if UNITY_EDITOR
    private static IEnumerable Modifiers = new ValueDropdownList<Modifier>()
    {
        { "+", Modifier.ADD },
        { "*", Modifier.MULTIPLY }
    };
#endif
}

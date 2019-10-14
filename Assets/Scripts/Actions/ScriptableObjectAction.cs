using System.Collections;
using UnityEngine;

public abstract class ScriptableObjectAction : ScriptableObject, AbstractAction
{
    public abstract IEnumerator Run();
}

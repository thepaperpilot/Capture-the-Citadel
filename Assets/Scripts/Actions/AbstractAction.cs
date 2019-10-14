using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public interface AbstractAction
{
    IEnumerator Run();
}

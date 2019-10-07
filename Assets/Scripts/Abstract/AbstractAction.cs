using System.Collections;

public class AbstractAction
{
    public virtual IEnumerator Run() {
        yield return null;
    }
}

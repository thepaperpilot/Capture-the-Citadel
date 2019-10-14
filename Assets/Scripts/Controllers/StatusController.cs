using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatusController : MonoBehaviour
{
    readonly public List<AbstractStatus> statuses = new List<AbstractStatus>();

    private void ResetStatuses(Scene scene, LoadSceneMode mode) {
        statuses.RemoveRange(0, statuses.Count);
    }

    public void AddStatus(AbstractStatus status) {
        statuses.Add(status);
    }

    public void RemoveStatus(AbstractStatus status) {
        statuses.Remove(status);
    }

    public int GetStrength(int baseStrength) {
        int strength = baseStrength;
        foreach (AbstractStatus status in statuses.Where(status => status.affectsStrength).OrderBy(status => status.priority)) {
            strength = status.ModifyStrength(strength);
        }
        return strength;
    }

    public int GetDexterity(int baseDexterity) {
        int dexterity = baseDexterity;
        foreach (AbstractStatus status in statuses.Where(status => status.affectsDexterity).OrderBy(status => status.priority)) {
            dexterity = status.ModifyDexterity(dexterity);
        }
        return dexterity;
    }
}

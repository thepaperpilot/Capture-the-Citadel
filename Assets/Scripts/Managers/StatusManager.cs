using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatusManager : MonoBehaviour
{
    public static StatusManager Instance;

    readonly public List<AbstractStatus> statuses = new List<AbstractStatus>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(this);
        }
    }

    private void Start() {
        SceneManager.sceneLoaded += ResetStatuses;
    }

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

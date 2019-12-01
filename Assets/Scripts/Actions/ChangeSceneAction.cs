using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneAction : AbstractAction
{
    public string scene;

    public ChangeSceneAction(string scene) {
        this.scene = scene;
    }

    public IEnumerator Run()
    {
        SceneManager.LoadScene(scene);
        yield return null;
    }
}

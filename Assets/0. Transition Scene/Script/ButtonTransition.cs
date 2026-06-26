using System;
using UnityEngine;

public class ButtonTransition : MonoBehaviour
{
    public SceneTransitioner sceneTransitioner;
    void Start()
    {
        sceneTransitioner = SceneTransitioner.Instance;
    }

    public void OnClickTransition(String sceneName)
    {
        sceneTransitioner.TriggerTransition(sceneName);
    }


}

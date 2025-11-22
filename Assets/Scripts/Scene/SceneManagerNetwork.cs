using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SceneManagerNetwork : NetworkBehaviour
{
    public static SceneManagerNetwork instance;
    private void Awake()
    {
        instance = this;

    }

    [Server]
    public void ChangeScene(string sceneName)
    {
        NetworkManager.singleton.ServerChangeScene(sceneName);
    }
}

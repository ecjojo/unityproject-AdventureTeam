using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MainGameController : NetworkBehaviour
{
    public static MainGameController instance;
    public PlayerScript localPlayerManager;

    public SyncListInt PlayerIDList;
    public List<Transform> PlayerStartPos;
    public List<GameObject> AllPlayerPrefabsObjs;

    int PlayerDeadCount;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

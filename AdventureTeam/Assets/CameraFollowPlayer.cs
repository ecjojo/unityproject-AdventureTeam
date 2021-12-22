using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        if (MainGameController.instance.localPlayerManager != null)
        {
            if (MainGameController.instance.localPlayerManager.gameObject.transform.position.x < 0)
            {
                transform.position = new Vector3(0, 0, -10);
            }
            else
            {
                transform.position = new Vector3(MainGameController.instance.localPlayerManager.gameObject.transform.position.x, 0, -10);
            }

        }

    }
}

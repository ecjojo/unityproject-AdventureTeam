using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CheckpointScript : NetworkBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("ShowCheck");
            GetComponent<Animator>().SetBool("Check", true);
            Invoke("ServerChangeScene", 1f);         
        }
    }

     public void ServerChangeScene()
     {
        NetworkManager.singleton.ServerChangeScene("Level1BossScene");
     }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    Animator anim;

    public GameObject LobbyCanvas;
    public GameObject HowtoplayPanel;
    public GameObject CreditsPanel;

    public GameObject Character;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void _OpenMenuButtonOnClick()
    {
        anim.SetBool("isOpenMeun", true);
        Character.GetComponent<Animator>().SetBool("isOpenMeun", true);
    }

    public void _ShowLobbyButtonOnClick()
    {
        LobbyCanvas.GetComponent<Canvas>().sortingOrder = 50;
    }

    public void _ShowHowtoplayButtonOnClick()
    {
        HowtoplayPanel.SetActive(true);
    }

    public void _ShowCreditsButtonOnClick()
    {
        CreditsPanel.SetActive(true);
    }

    public void _HideAllPanelButtonOnClick()
    {
        HowtoplayPanel.SetActive(false);
        CreditsPanel.SetActive(false);
        LobbyCanvas.GetComponent<Canvas>().sortingOrder = 0;
    }

    public void _QuitButtonOnClick()
    {
        Application.Quit();
    }
}

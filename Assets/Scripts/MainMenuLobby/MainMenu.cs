using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private LobbyManager lobbyManager = null;

    [Header("UI")]
    [SerializeField] private GameObject menuParent1 = null;
    [SerializeField] private GameObject menuParent2 = null;
    [SerializeField] private GameObject menuParent3 = null;

    private void Start()
    {
        menuParent1.SetActive(true);
        menuParent2.SetActive(false);
        menuParent3.SetActive(false);
    }

    private void Update()
    {
        lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    public void HostLobby()
    {
        lobbyManager.StartHost();
        menuParent2.SetActive(false);
        menuParent3.SetActive(false);
    }
}

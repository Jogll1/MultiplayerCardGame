using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private LobbyManager lobbyManager = null;

    [Header("UI")]
    //[SerializeField] private GameObject menuParent = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    private void Update()
    {
        lobbyManager = GameObject.FindWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    private void OnEnable()
    {
        LobbyManager.OnClientConnected += HandleClientConnected;
        LobbyManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        LobbyManager.OnClientConnected += HandleClientConnected;
        LobbyManager.OnClientDisconnected += HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;

        lobbyManager.networkAddress = ipAddress;
        lobbyManager.StartClient();

        joinButton.interactable = false; //so players cant spam it
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        //menuParent.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}

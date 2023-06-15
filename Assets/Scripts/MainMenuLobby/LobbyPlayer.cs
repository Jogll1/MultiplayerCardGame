using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class LobbyPlayer : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[2];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[2];
    [SerializeField] private Button startGameButton = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))] //hook is the nameof a method that is called when this variable is changed
    public string displayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool isReady = false;

    private bool isLeader;

    //setter for isLeader
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    private LobbyManager lobby;
    //getter for lobby
    private LobbyManager Lobby
    {
        get
        {
            if (lobby != null) { return lobby; }
            return lobby = NetworkManager.singleton as LobbyManager;
        }
    }

    public override void OnStartAuthority()
    {
        //base.OnStartAuthority();

        CmdSetDisplayName("Player " + (Lobby.playersInLobby.Count + 1).ToString()); //PlayerNameInput.DisplayName

        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        //base.OnStartClient();

        Lobby.playersInLobby.Add(this); //add to list in LobbyManager

        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        //base.OnStopClient();

        Lobby.playersInLobby.Remove(this); //remove from list when a client leaves

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    public void UpdateDisplay()
    {
        if (!isOwned) //maybe change
        {
            foreach (var player in Lobby.playersInLobby) //basically look for the player we own and update their display
            {
                if (player.isOwned)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting for Player...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Lobby.playersInLobby.Count; i++)
        {
            playerNameTexts[i].text = Lobby.playersInLobby[i].displayName;
            playerReadyTexts[i].text = Lobby.playersInLobby[i].isReady ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>"; //idk why theres some random CSS
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string _displayName)
    {
        displayName = _displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        isReady = !isReady;

        Lobby.NotifyReadyPlayers();
    }

    [Command]

    public void CmdStartGame()
    {
        if (Lobby.playersInLobby[0].connectionToClient != connectionToClient) { return; } //make sure person who called this is the leader

        //Start game code

        Lobby.StartGame();
    }
}

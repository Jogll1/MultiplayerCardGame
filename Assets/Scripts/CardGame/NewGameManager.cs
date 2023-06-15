using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;

public class NewGameManager : NetworkBehaviour
{
    [Header("Bools")]
    [SyncVar] public bool gameStarted = false;

    [Header("Network Vars")]
    [SyncVar] public int turnCount = 1; //start at turn one
    [SyncVar] public int playersConnected;

    [Header("Text GameObjects")]
    //public GameObject waitingText;
    public GameObject turnText;

    //[Header("Players List")]
    //public List<GamePlayer> players = new List<GamePlayer>(); //list of the current players

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

    private void Start()
    {
        //cardsManager = GameObject.FindWithTag("CardsManager").GetComponent<CardsManager>();
    }

    private void Update()
    {
        if (isServer)
        {
            playersConnected = NetworkServer.connections.Count;
        }

        if (gameStarted /*&& playersConnected == 2*/)
        {
            //waitingText.SetActive(false);

            turnText.GetComponent<TextMeshProUGUI>().enabled = true;
            turnText.GetComponent<TextMeshProUGUI>().text = "Turn: " + turnCount.ToString();
        }
        else
        {
            turnText.GetComponent<TextMeshProUGUI>().enabled = false;
            //waitingText.SetActive(true);
        }

        if (gameStarted && playersConnected < 2) //if someone leaves whilst the game is playing
        {
            EndGame();
        }
    }

    [ClientRpc]
    public void RpcStartGame()
    {
        gameStarted = true;

        //not working because both players are server
        //server randomly decide which player goes first
        // if (isServer)
        // {
        //     Debug.Log("Server");
        //     //randomly reverse list
        //     int ran = Random.Range(0, 2);
        //     Debug.Log(ran);
        //     if (ran == 0)
        //     {
        //         Lobby.playersInGame.Reverse();
        //         Debug.Log("Reversed list");
        //     }
        // } 

        //set all playernums
        for (int i = 0; i < Lobby.playersInGame.Count; i++)
        {
            GamePlayer gamePlayer = Lobby.playersInGame[i];

            //assign playerNums
            gamePlayer.playerNum = i + 1;

            //set who's first
            if (gamePlayer.playerNum == 1)
            {
                gamePlayer.isOurTurn = true;
            }
            else
            {
                gamePlayer.isOurTurn = false;
            }

            //gamePlayer.energyUiManager.UpdateEnergyUI();
        }
    }

    #region Turns
    public void UpdateTurnCount()
    {
        turnCount++;

        //increment energy
        for (int i = 0; i < Lobby.playersInGame.Count; i++)
        {
            GamePlayer player = Lobby.playersInGame[i];
            if (!player.isOurTurn && player.maxEnergy < 10)
            {
                player.maxEnergy++;
            }
            player.currentEnergy = player.maxEnergy; //reset the current energy
            player.energyUiManager.UpdateEnergyUI();
        }
    }

    public void NextTurn()
    {
        RpcNextTurn();
    }

    [ClientRpc] //used for each turn
    public void RpcNextTurn()
    {
        Debug.Log("Next turn");

        //increment turn count
        UpdateTurnCount();

        //change each player's isOurTurn
        for (int i = 0; i < Lobby.playersInGame.Count; i++)
        {
            Lobby.playersInGame[i].isOurTurn = !Lobby.playersInGame[i].isOurTurn;
        }

        //player who's turn it is draws a card
        for (int i = 0; i < Lobby.playersInGame.Count; i++)
        {
            GamePlayer gp = Lobby.playersInGame[i];
            //if the player is owned, is their turn, and they have less than 9 cards in their hand, draw a card
            if (gp.isOwned && gp.isOurTurn && gp.cardsInHand < 9) Lobby.playersInGame[i].CmdDrawCard();
        }

        //allow all cards to attack again
        Transform playerPlayArea = GameObject.FindWithTag("PlayerPlayArea").transform;
        Transform opponentPlayArea = GameObject.FindWithTag("OpponentPlayArea").transform;

        foreach (Transform child in playerPlayArea)
        {
            child.GetComponent<CardAttack>().hasAttackedThisTurn = false;
        }

        foreach (Transform child in opponentPlayArea)
        {
            child.GetComponent<CardAttack>().hasAttackedThisTurn = false;
        }
    }
    #endregion

    public void EndGame()
    {
        //when a player disconnects, stop the host and take them back to the main menu
        lobby.StopHost();
        // lobby.StopServer(); //hm
        // lobby.StopClient();
        // lobby.OnStopClient();
        // lobby.OnStopServer();
        lobby.OnStopHost();
        Destroy(lobby.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}

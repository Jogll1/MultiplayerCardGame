using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class LobbyManager : NetworkManager
{
    [Header("Custom variables")]

    [Header("Min Players")]
    [SerializeField] private int minPlayers = 2;

    [Header("Scene")]
    [Scene][SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private LobbyPlayer lobbyPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private GamePlayer gamePlayerPrefab = null;
    public string gameSceneName = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    [Header("Player Lists")]
    //list of all the players in the lobby
    [SerializeField] public List<LobbyPlayer> playersInLobby = new List<LobbyPlayer>();
    [SerializeField] public List<GamePlayer> playersInGame = new List<GamePlayer>(); //{ get; } 

    #region On Server methods
    public override void OnStartServer()
    {
        //base.OnStartServer();

        //set prefabs to spawn on the network?
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList(); //maybe
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        //base.OnServerConnect(conn);

        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<LobbyPlayer>();

            playersInLobby.Remove(player);

            NotifyReadyPlayers();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //base.OnServerAddPlayer(conn);

        if (SceneManager.GetActiveScene().path == menuScene)
        {
            bool isLeader = playersInLobby.Count == 0; //if host basically, true if first player

            LobbyPlayer lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab);
            lobbyPlayerInstance.gameObject.name = $"{lobbyPlayerPrefab.name} [connId={conn.connectionId}]";
            lobbyPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);
            //Debug.Log(lobbyPlayerInstance);
        }
    }

    public override void OnStopServer()
    {
        //base.OnStopServer();

        playersInLobby.Clear(); //clear list when server stopped
    }
    #endregion

    #region On Client methods
    public override void OnStartClient()
    {
        //base.OnStartClient();

        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();
    }
    #endregion

    #region Readying
    public void NotifyReadyPlayers()
    {
        foreach (var player in playersInLobby)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in playersInLobby)
        {
            if (!player.isReady) { return false; }
        }

        return true;
    }
    #endregion

    #region Joining Game
    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            if (!IsReadyToStart()) { return; }

            ServerChangeScene(gameSceneName); //change to game scene
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        //from menu to game
        if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith(gameSceneName)) //if going to game scene
        {
            for (int i = playersInLobby.Count - 1; i >= 0; i--)
            {
                var conn = playersInLobby[i].connectionToClient;
                var gamePlayerInstance = Instantiate(gamePlayerPrefab); //instantiate player prefab
                gamePlayerInstance.gameObject.name = $"Player [connId={conn.connectionId}]";
                gamePlayerInstance.SetDisplayName(playersInLobby[i].displayName); //set players name

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject); //replace player

                //set the leader (host) player
                bool isLeader = playersInLobby.Count == 0;
                gamePlayerInstance.isLeader = isLeader;
            }
        }

        base.ServerChangeScene(newSceneName);
    }
    #endregion
}

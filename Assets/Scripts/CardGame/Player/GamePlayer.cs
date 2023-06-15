using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class GamePlayer : NetworkBehaviour
{
    [Header("Game Manager")]
    public NewGameManager newGameManager;

    [Header("Card")]
    public GameObject cardPrefab; //update this
    [SyncVar] public int cardsInHand = 0;

    [Header("Play Area Transforms")]
    public Transform playerHand;
    public Transform opponentHand;
    public Transform playerPlayArea;
    public Transform opponentPlayArea;

    [Header("Texts")]
    public GameObject playerText;
    public GameObject yourTurnText;
    public GameObject healthText; //health test

    [Header("Network Vars")]
    [SyncVar] public int playerNum; //determine if they go first
    [SyncVar] public bool isOurTurn = false; //check if it is our turn
    [SyncVar] public bool isLeader;
    [SyncVar] public bool gameStarted;
    public int playerHealth = 100; //health test
    [SyncVar] private string displayName = "Loading...";

    [Header("Energy")]
    [SyncVar] public int maxEnergy = 1;
    [SyncVar] public int currentEnergy = 1;
    public EnergyUIManager energyUiManager;

    [Header("End Button")]
    public GameObject endTurnButton;

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

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == Lobby.gameSceneName /*&& isOwned*/) //maybe find a way to optimise this
        {
            if (GameObject.FindWithTag("GameManager") != null) newGameManager = GameObject.FindWithTag("GameManager").GetComponent<NewGameManager>();
            if (GameObject.FindWithTag("EnergyUIManager") != null) energyUiManager = GameObject.FindWithTag("EnergyUIManager").GetComponent<EnergyUIManager>();

            if (GameObject.FindWithTag("PlayerHand") != null) playerHand = GameObject.FindWithTag("PlayerHand").transform;
            if (GameObject.FindWithTag("OpponentHand") != null) opponentHand = GameObject.FindWithTag("OpponentHand").transform;
            if (GameObject.FindWithTag("PlayerPlayArea") != null) playerPlayArea = GameObject.FindWithTag("PlayerPlayArea").transform;
            if (GameObject.FindWithTag("OpponentPlayArea") != null) opponentPlayArea = GameObject.FindWithTag("OpponentPlayArea").transform;
            if (GameObject.FindWithTag("EndTurnButton") != null) endTurnButton = GameObject.FindWithTag("EndTurnButton");

            if (GameObject.FindWithTag("PlayerText") != null) playerText = GameObject.FindWithTag("PlayerText");
            if (GameObject.FindWithTag("YourTurnText") != null) yourTurnText = GameObject.FindWithTag("YourTurnText");
            if (GameObject.FindWithTag("HealthText") != null) healthText = GameObject.FindWithTag("HealthText");

            if (newGameManager != null && isOwned)
            {
                if (newGameManager.gameStarted)
                {
                    //player num text
                    playerText.GetComponent<TextMeshProUGUI>().enabled = true;
                    playerText.GetComponent<TextMeshProUGUI>().text = "P" + playerNum.ToString();

                    //turn text
                    yourTurnText.GetComponent<TextMeshProUGUI>().enabled = true;
                    if (isOurTurn)
                    {
                        yourTurnText.GetComponent<TextMeshProUGUI>().text = "It's your turn!";
                    }
                    else
                    {
                        yourTurnText.GetComponent<TextMeshProUGUI>().text = "It's not your turn.";
                    }

                    //health text
                    healthText.GetComponent<TextMeshProUGUI>().enabled = true;
                    healthText.GetComponent<TextMeshProUGUI>().text = "Health: " + playerHealth.ToString();
                }
                else
                {
                    playerText.GetComponent<TextMeshProUGUI>().enabled = false;
                    yourTurnText.GetComponent<TextMeshProUGUI>().enabled = false;
                    healthText.GetComponent<TextMeshProUGUI>().enabled = false;
                }


                //enable endTurnButton if its our turn
                endTurnButton.transform.GetChild(0).gameObject.SetActive(isOurTurn);
            }

            if (NetworkClient.ready && isOwned)
            {
                if (!gameStarted)
                {
                    if (isLeader)
                    {
                        CmdStartGame();
                    }

                    gameStarted = true;
                    Debug.Log("Started");
                    //if (energyUiManager != null) energyUiManager.UpdateEnergyUI();
                }
            }
        }
    }

    #region NetworkBehaviour overrides
    public override void OnStartClient()
    {
        //base.OnStartClient();

        //dont destroy this player when changing scene
        DontDestroyOnLoad(gameObject);

        //add to list in LobbyManager
        Lobby.playersInGame.Add(this);

        //CmdStartGame();
    }

    public override void OnStopClient()
    {
        //base.OnStopClient();

        //remove from list when a client leaves
        Lobby.playersInGame.Remove(this);
    }
    #endregion

    #region Server
    [Server]
    public void SetDisplayName(string _displayName)
    {
        displayName = _displayName;
    }
    #endregion

    #region Drawing and Playing cards
    [Command] //commands start with Cmd - commands are requests by the client to the server
    public void CmdDrawCard()
    {
        //GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity); //instantiate new gameobject
        GameObject card = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.identity); //instantiate new gameobject

        NetworkServer.Spawn(card, connectionToClient); //spawn card on the server
        RpcShowCard(card, "Dealt");

        cardsInHand++;
    }

    public void PlayCard(GameObject cardGameObject)
    {
        CmdPlayCard(cardGameObject);
    }

    [Command]
    void CmdPlayCard(GameObject cardGameObject) //could this be public? 
    {
        RpcShowCard(cardGameObject, "Played"); //play the card on the network
    }

    [ClientRpc] //rpcs start with Rpc - client rpcs are something the server requests of all clients (a target rpc makes a single client do something)
    void RpcShowCard(GameObject card, string type)
    {
        if (type == "Dealt")
        {
            //test ----------------- this might only work when both players are on the same computer, should load on host then sent to clients
            List<CardData> cardDataList = SaveCardData.ReadFromJson<CardData>("deckDataFile.json");
            int ran = Random.Range(0, cardDataList.Count);
            if (isServer) card.GetComponent<CardPlayData>().RpcSetCardDesign(cardDataList[ran]);
            //----------------------

            //when dealt a card, set its position to the correct hand transform
            if (isOwned)
            {
                card.GetComponent<NewCardDrag>().SetCardParent(playerHand.transform);
                //card.transform.SetParent(playerHand.transform);
            }
            else
            {
                card.GetComponent<NewCardDrag>().SetCardParent(opponentHand.transform);
                //card.transform.SetParent(opponentHand.transform);
                card.GetComponent<FlipCard>().Flip(); //flip the card to hide it
            }

            //get which index each child is
            for (int i = 0; i < card.GetComponent<NewCardDrag>().cardParent.childCount; i++)
            {
                if (card.transform.parent.GetChild(i) == card.transform) card.GetComponent<NewCardDrag>().placeInHand = i;
            }
        }
        else if (type == "Played")
        {
            //disable energy symbol
            card.GetComponent<NewCardDrag>().energySymbol.SetActive(false);

            card.transform.rotation = Quaternion.identity;

            //when a card is played, set its position to the correct play area transform
            if (isOwned)
            {
                card.GetComponent<NewCardDrag>().SetCardParent(playerPlayArea.transform);
            }
            else
            {
                card.GetComponent<NewCardDrag>().SetCardParent(opponentPlayArea.transform);
                card.GetComponent<FlipCard>().Flip(); //flip the card to show the opponent what it is
            }

            cardsInHand--; //decrement card counter

            //get which index each child is
            for (int i = 0; i < playerHand.childCount; i++)
            {
                playerHand.GetChild(i).GetComponent<NewCardDrag>().placeInHand = i;
            }

            //----------ABILITIES TEST----------
            AbilityManager abilityManager = GameObject.FindWithTag("GameManager").GetComponent<AbilityManager>();
            AbilitySO ability = abilityManager.abilitiesIndex[card.GetComponent<CardPlayData>().cardData.cardAbilityIndex];
            if (ability.abilityType == "On Play")
            {
                card.GetComponent<CardAbility>().OnPlay();
            }
        }
    }
    #endregion

    #region Card Hover
    //hovering opponent's card they are currently selecting
    [Command]
    public void CmdOpponentCardHover(GameObject cardObject, bool hoverActive)
    {
        RpcHoverCard(cardObject, hoverActive);
    }

    [ClientRpc(includeOwner = false)] //ignore the owner client when sending this out
    void RpcHoverCard(GameObject cardObject, bool hoverActive)
    {
        if (!isOwned)
        {
            //hover the card front of the correct object
            if (hoverActive)
            {
                cardObject.GetComponent<NewCardHover>().cardBack.GetComponent<RectTransform>().localPosition = new Vector2(0, -75);
                //cardObject.GetComponent<NewCardHover>().cardFront.SetActive(false);
            }
            else
            {
                cardObject.GetComponent<NewCardHover>().cardBack.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                //cardObject.GetComponent<NewCardHover>().cardFront.SetActive(false);
            }
        }
    }
    #endregion

    #region Game Logic
    [Command]
    public void CmdStartGame()
    {
        //Debug.Log("Command");
        newGameManager.RpcStartGame();
    }

    [Command]
    public void CmdDrawStartCards()
    {
        RpcDrawStartCards();
    }

    [ClientRpc]
    public void RpcDrawStartCards()
    {
        //draw four cards for each player
        for (int j = 0; j < 4; j++)
        {
            if (isOwned) CmdDrawCard();
        }
    }

    [Command]
    public void CmdEndTurn()
    {
        newGameManager.NextTurn();
    }
    #endregion

    #region Destroy card
    [Command(requiresAuthority = false)] //so player can call it on opponent
    public void CmdDestoryCard(GameObject card)
    {
        RpcDestroyCard(card);
    }

    [ClientRpc]
    public void RpcDestroyCard(GameObject card)
    {
        if (card == null) return;

        NetworkServer.Destroy(card);
        if (card != null) Destroy(card);
    }
    #endregion

    //allow other objects to access this gameplayer's lobby
    public LobbyManager GetLobby()
    {
        return Lobby;
    }
}

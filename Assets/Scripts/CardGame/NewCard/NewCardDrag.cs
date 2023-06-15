using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror; //make this script send over the network

//script that manages dragging cards
public class NewCardDrag : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Game Player and Manager")]
    public GamePlayer gamePlayer; //for networking
    public NewGameManager newGameManager;

    [Header("Bools")]
    public bool isDragging = false;
    public bool isDraggable = false; //if the card can be dragged by this player
    [SyncVar] public bool inPlayArea = false;

    private Transform uiCanvas; //main canvas
    [Header("Card Parent")]
    public Transform cardParent;

    [Header("Card Components")]
    public CanvasGroup canvasGroup; //canvasGroup of the whole card
    public LayoutElement layoutElement; //manage the card's attributes when its part of a layout group
    private NewCardHover newCardHover;
    public GameObject energySymbol;

    [Header("Drop Position")]
    public Vector3 dropPosition; //position the card will go to when it is not being dragged

    public int placeInHand;

    // Start is called before the first frame update
    void Start()
    {
        uiCanvas = GameObject.FindWithTag("UICanvas").transform;
        newGameManager = GameObject.FindWithTag("GameManager").GetComponent<NewGameManager>();

        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        gamePlayer = networkIdentity.GetComponent<GamePlayer>();

        isDraggable = isOwned; //let the card be draggable if you own it

        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        layoutElement = gameObject.GetComponent<LayoutElement>();
        newCardHover = gameObject.GetComponent<NewCardHover>();
    }

    public void SetCardParent(Transform parent)
    {
        //so the card knows its parent
        transform.SetParent(parent, false);
        dropPosition = transform.position; //set the drop position of the card
        cardParent = parent; //set the parent of the card
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanPlayCard()) return; //if cant drag it, don't let it be dragged - maybe make this all one bool

        //make it so cards cant hover or show card info when a card is being dragged
        newCardHover.playerHand.cardHover = false;
        newCardHover.isHovering = false;
        newCardHover.HideCardInfo();
        newCardHover.gamePlayer.CmdOpponentCardHover(gameObject, false);

        isDragging = true;
        canvasGroup.blocksRaycasts = false;
        layoutElement.ignoreLayout = true;

        //Debug.Log("Begin Drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanPlayCard()) return; //if cant drag it, don't let it be dragged

        transform.position = eventData.position;
        transform.SetParent(uiCanvas); //set card as last child of canvas to make it draw over everything
        transform.SetAsLastSibling();
        transform.rotation = Quaternion.identity;

        //Debug.Log("Dragging");
    }

    public void OnEndDrag(PointerEventData pointerEvent)
    {
        if (!CanPlayCard()) return; //if cant drag it, don't let it be dragged

        newCardHover.playerHand.cardHover = true;

        isDragging = false;
        transform.position = dropPosition;
        transform.SetParent(cardParent);
        canvasGroup.blocksRaycasts = true;
        layoutElement.ignoreLayout = false;

        if (inPlayArea) isDraggable = false; //don't let cards be dragged after being put down

        //if not in the play area (so in the hand), set it to the placeInHand positionb of the parent
        if (!inPlayArea) transform.SetSiblingIndex(placeInHand);
    }

    public bool CanPlayCard()
    {
        if (!isDraggable || !isOwned || !gamePlayer.isOurTurn || !newGameManager.gameStarted) return false;

        return true;
    }
}

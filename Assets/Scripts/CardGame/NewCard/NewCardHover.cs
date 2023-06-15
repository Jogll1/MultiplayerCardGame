using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;

public class NewCardHover : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Player Objects")]
    public PlayerHand playerHand;
    public GamePlayer gamePlayer;

    [Header("Card Sides")]
    public GameObject cardFront;
    public GameObject cardBack;

    [Header("Bools")]
    public bool canHover = true; //if this card can hover
    public bool isHovering;

    // Start is called before the first frame update
    void Start()
    {
        playerHand = GameObject.FindWithTag("PlayerHand").GetComponent<PlayerHand>();

        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        gamePlayer = networkIdentity.GetComponent<GamePlayer>();
    }

    void Update()
    {
        if (gameObject.GetComponent<NewCardDrag>().inPlayArea)
        {
            canHover = false;
        }

        if (isHovering && canHover && isOwned)
        {
            cardFront.GetComponent<RectTransform>().localPosition = new Vector2(0, 400);
            cardFront.SetActive(false);
            //gameObject.transform.rotation = Quaternion.identity;
        }
        else
        {
            cardFront.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            cardFront.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanHover()) return; //maybe make this all one bool 

        isHovering = true;

        if (isHovering && canHover)
        {
            ShowCardInfo();
            gamePlayer.CmdOpponentCardHover(gameObject, true); //send the hover through the network
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!CanHover()) return;

        isHovering = false;

        cardFront.SetActive(true);

        HideCardInfo();
        gamePlayer.CmdOpponentCardHover(gameObject, false); //send the hover through the network
    }

    public void ShowCardInfo()
    {
        playerHand.cardInfo.GetComponent<RectTransform>().localPosition = new Vector2(GetComponent<RectTransform>().localPosition.x, -300);
        playerHand.cardInfo.GetComponent<CardInfo>().SetCardDesign(GetComponent<CardPlayData>().cardData);
        playerHand.cardInfo.SetActive(true);
    }

    public void HideCardInfo()
    {
        playerHand.cardInfo.SetActive(false);
        cardFront.SetActive(true);
    }

    public bool CanHover()
    {
        if (!canHover || !playerHand.cardHover || !gamePlayer.isOurTurn || !isOwned || GetComponent<CardAttack>().dragArrow.isActive) return false;

        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror;

public class CardAttack : NetworkBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Game Player")]
    public GamePlayer gamePlayer;

    [Header("Card Components")]
    private NewCardDrag newCardDrag;
    public Outline cardOutline;

    [Header("Bools")]
    public bool isHovered;
    public bool isSelected;
    public bool hasAttackedThisTurn;
    public bool playerIsAttacking;

    [Header("Colour")]
    public Color selected;
    public Color notSelected;

    [Header("UI Drag Arrow")]
    public UIDrawArrow dragArrow; //The graphic that appears when you drag for an attack

    private void Start()
    {
        //always gets this player, not opponent, even if card is not owned
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        gamePlayer = networkIdentity.GetComponent<GamePlayer>();

        dragArrow = GameObject.FindWithTag("ArrowParent").GetComponent<UIDrawArrow>();

        newCardDrag = GetComponent<NewCardDrag>();

        cardOutline.effectColor = notSelected;
        cardOutline.enabled = false;

        hasAttackedThisTurn = false;
    }

    private void Update()
    {
        if (isHovered && !isSelected)
        {
            cardOutline.effectColor = notSelected;
            cardOutline.enabled = true;
        }
        else if (isSelected)
        {
            cardOutline.effectColor = selected;
            cardOutline.enabled = true;
        }
        else if (!isHovered && !isSelected)
        {
            cardOutline.effectColor = notSelected;
            cardOutline.enabled = false;
        }
    }

    #region Dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CanAttack())
        {
            isSelected = true;
            playerIsAttacking = true;

            dragArrow.transform.position = transform.position;
            dragArrow.isActive = true;
        }
        else
        {
            isSelected = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData pointerEvent)
    {
        if (CanAttack())
        {
            dragArrow.isActive = false;
            playerIsAttacking = true;
        }

        isSelected = false;
    }
    #endregion

    #region Pointer over
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CanAttack() || IsBeingAttacked())
        {
            isHovered = true;
        }
        else
        {
            isHovered = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CanAttack() || IsBeingAttacked())
        {
            isHovered = false;
        }
    }
    #endregion

    public bool CanAttack()
    {
        //only attack card if: in play area, this card is owned, its our turn
        if (newCardDrag.inPlayArea && isOwned && gamePlayer.isOurTurn && !hasAttackedThisTurn)
        {
            return true;
        }

        return false;
    }

    public bool IsBeingAttacked()
    {
        bool otherPlayerAttacking = false;
        foreach (GamePlayer _gamePlayer in gamePlayer.GetLobby().playersInGame)
        {
            if (_gamePlayer != gamePlayer)
            {
                otherPlayerAttacking = true;
            }
        }

        if (!isOwned && otherPlayerAttacking && newCardDrag.inPlayArea) //return true if this card is an opponent's card, we are attacking, and their card is in the play area
        {
            return true;
        }

        return false;
    }
}

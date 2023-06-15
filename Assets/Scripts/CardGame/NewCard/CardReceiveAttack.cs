using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class CardReceiveAttack : NetworkBehaviour, IDropHandler
{
    private CardAttack cardAttack;
    private CardPlayData cardPlayData;
    private void Start()
    {
        cardAttack = GetComponent<CardAttack>();
        cardPlayData = GetComponent<CardPlayData>();
    }

    #region Drop
    public void OnDrop(PointerEventData eventData)
    {
        CardAttack card = eventData.pointerDrag.transform.GetComponent<CardAttack>();

        if (card != null && card.CanAttack() && cardPlayData.cardCurrentHP > 0)
        {
            if (!card.hasAttackedThisTurn) //make sure card hasn't attacked already this turn
            {
                if (card.transform.parent != transform.parent) //make sure only can be attacked by opponent's cards by checking if the parents are different
                {
                    if (gameObject.GetComponent<NewCardDrag>().inPlayArea) //make sure this card is in the playarea
                    {
                        //when a card is dragged over this card, perform the attack
                        //Debug.Log("Attack!");
                        CmdTakeHealth(card.GetComponent<CardPlayData>().cardData.cardAttack); //make this card take damage
                        card.gameObject.GetComponent<CardReceiveAttack>().CmdTakeHealth(cardPlayData.cardData.cardAttack); //make attacking card take damage

                        //make sure attack only once per turn
                        card.hasAttackedThisTurn = true;
                        cardAttack.dragArrow.isActive = false;
                        cardAttack.isSelected = false;
                    }
                }
            }
        }
    }
    #endregion

    #region Take health (on cardPlayData)
    [Command(requiresAuthority = false)] //so player can call it on opponent
    public void CmdTakeHealth(int healthToTake)
    {
        RpcTakeHealth(healthToTake);
    }

    [ClientRpc]
    public void RpcTakeHealth(int healthToTake)
    {
        cardPlayData.cardCurrentHP -= healthToTake;
        cardPlayData.healthText.text = cardPlayData.cardCurrentHP.ToString();

        //add if health is 0 then destroy card (+ on network?)
        if (cardPlayData.cardCurrentHP <= 0)
        {
            //----------test---------- subtract health from the card's player
            if (isOwned)
            {
                NetworkIdentity networkIdentity = NetworkClient.connection.identity;
                GamePlayer gamePlayer = networkIdentity.GetComponent<GamePlayer>();
                gamePlayer.playerHealth -= cardPlayData.cardData.cardHP;
            }

            cardAttack.dragArrow.isActive = false;
            if (cardAttack.gamePlayer != null) cardAttack.gamePlayer.CmdDestoryCard(gameObject);
        }
    }
    #endregion
}

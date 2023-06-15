using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror; //for networking

public class NewPlayArea : NetworkBehaviour, IDropHandler
{
    //(attaching this script to a gameobject makes it act as a drop zone)
    public int maxCards;

    public void OnDrop(PointerEventData eventData)
    {
        NewCardDrag card = eventData.pointerDrag.transform.GetComponent<NewCardDrag>();

        //if gameobject has the cardDrag script and is owned by this player and its that player's turn, and there is less than 9 cards in this play area
        if (card != null && card.isOwned && card.gamePlayer.isOurTurn && transform.childCount < maxCards)
        {
            if (card.dropPosition != transform.position) //if card is being dragged over a dropzone (something with this script attached), change its dropPosition
            {
                //check if this card has enough energy to be played
                if (card.gamePlayer.currentEnergy < card.gameObject.GetComponent<CardPlayData>().cardData.cardEnergy) return;
                //subtract some of the player's energy if can be
                card.gamePlayer.currentEnergy -= card.gameObject.GetComponent<CardPlayData>().cardData.cardEnergy;
                card.gamePlayer.energyUiManager.UpdateEnergyUI();

                //networking
                NetworkIdentity networkIdentity = NetworkClient.connection.identity;
                card.gamePlayer = networkIdentity.GetComponent<GamePlayer>();
                card.gamePlayer.PlayCard(card.gameObject); //when you play this card, play it over the network

                //changing the drop position of the card
                card.dropPosition = transform.position;
                card.cardParent = transform;
                card.transform.rotation = Quaternion.identity;

                card.inPlayArea = true;
            }
        }
    }
}

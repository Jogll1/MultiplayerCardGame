using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NewCardsManager : NetworkBehaviour
{
    public GamePlayer gamePlayer; //for networking

    private void Start()
    {
        StartCoroutine(DrawStartCards());
    }

    public void DrawCard()
    {
        if (gamePlayer.cardsInHand < 9) //only draw a card if less than 9 cards in hand
        {
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            gamePlayer = networkIdentity.GetComponent<GamePlayer>();
            gamePlayer.CmdDrawCard();
        }
    }

    public IEnumerator DrawStartCards()
    {
        yield return new WaitForSeconds(0.2f); //this makes it work for some reason 
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        gamePlayer = networkIdentity.GetComponent<GamePlayer>();
        gamePlayer.CmdDrawStartCards();
    }

    public void EndTurn() //idk where else to put it
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        gamePlayer = networkIdentity.GetComponent<GamePlayer>();
        gamePlayer.CmdEndTurn();
        Debug.Log("End Turn, " + gamePlayer);
    }
}

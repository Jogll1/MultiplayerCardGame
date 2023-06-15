using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script to flip the opponent's cards
public class FlipCard : MonoBehaviour
{
    public GameObject cardBack;
    public GameObject cardFront;

    public void Flip()
    {
        //flip the card by setting the reverse of what it currently is
        cardBack.SetActive(!cardBack.activeSelf);
        cardFront.SetActive(!cardFront.activeSelf);
    }
}

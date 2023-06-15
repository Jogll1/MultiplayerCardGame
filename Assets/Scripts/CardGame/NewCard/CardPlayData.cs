using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using UnityEngine.EventSystems;

//changes the card in play's design to match its cardData
public class CardPlayData : NetworkBehaviour
{
    public CardData cardData;

    [Header("Card Design Elements")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI abilityText;
    public Image cardFrontBackground;
    public Image cardBackBackground;
    public RawImage cardArt;

    [Header("Stats")]
    public int cardCurrentHP;

    [Header("Colours")]
    public Color positiveStats;
    public Color neutralStats;
    public Color negativeStats;

    private void Update()
    {
        if (cardCurrentHP == cardData.cardHP)
        {
            healthText.color = neutralStats;
        }
        else if (cardCurrentHP < cardData.cardHP)
        {
            healthText.color = negativeStats;
        }
        else
        {
            healthText.color = positiveStats;
        }

        //also destroy when cardHP = 0
    }

    [Command]
    public void CmdSetCardDesign(CardData _cardData)
    {
        RpcSetCardDesign(_cardData);
    }

    [ClientRpc] //used for initialising the card design
    public void RpcSetCardDesign(CardData _cardData)
    {
        cardData = _cardData; //add a reference to cardData stored in this prefab

        nameText.text = _cardData.cardName;
        healthText.text = _cardData.cardHP.ToString();
        attackText.text = _cardData.cardAttack.ToString();
        energyText.text = _cardData.cardEnergy.ToString();

        //load ability text
        AbilityManager abilityManager = GameObject.FindWithTag("GameManager").GetComponent<AbilityManager>();
        abilityText.text = abilityManager.abilitiesIndex[_cardData.cardAbilityIndex].abilityDescription;
        AbilitySO ability = abilityManager.abilitiesIndex[_cardData.cardAbilityIndex];
        if (ability.abilityType == "None" || ability.abilityType == "During Play")
        {
            abilityText.text = ability.abilityDescription;
        }
        else
        {
            abilityText.text = ability.abilityType + ": " + ability.abilityDescription;
        }

        //set bg colour
        Color bgColour;
        if (ColorUtility.TryParseHtmlString(_cardData.cardHex, out bgColour))
        {
            cardFrontBackground.color = bgColour;
            //cardBackBackground.color = bgColour;
        }

        //load image eventually

        //set stats
        cardCurrentHP = _cardData.cardHP;
    }

    #region Health
    // public void CmdTakeHP(int healthToTake)
    // {
    //     //needs case for if health goes less than 0
    //     // cardHP -= healthToTake;
    //     // cardData.cardHP = cardHP;
    //     Debug.Log("Cmd");
    //     // if (isOwned) CmdUpdateCardHP();

    //     RpcUpdateCardHP(healthToTake);
    // }

    // [ClientRpc]
    // public void RpcUpdateCardHP(int healthToTake)
    // {
    //     cardHP -= healthToTake;
    //     cardData.cardHP = cardHP;
    //     healthText.text = cardHP.ToString();
    //     Debug.Log("Rpc");
    // }
    #endregion
}

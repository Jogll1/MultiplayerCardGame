using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInfo : MonoBehaviour
{
    //this script is for the big card that appears when you hover over a card in your hand
    public CardData cardData;

    [Header("Card Design Elements")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI abilityText;
    public Image cardFrontBackground;
    public RawImage cardArt;


    private void Start()
    {

    }

    public void SetCardDesign(CardData _cardData)
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
        }

        //load image eventually
    }
}

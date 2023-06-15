using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplayPrefab : MonoBehaviour
{
    private DeckCreator deckCreator;
    private AbilityManager abilityManager;
    private CardData cardData;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI abilityText;
    public TextMeshProUGUI energyText;
    public Image cardBackground;
    private string imagePath; //unused
    private string hex; //unused
    public RawImage cardArt;

    double artOffsetY;
    //private float initialImageHeight = 358f;

    private void Start()
    {
        deckCreator = GameObject.FindWithTag("DeckCreatorManager").GetComponent<DeckCreator>();
        abilityManager = GameObject.FindWithTag("DeckCreatorManager").GetComponent<AbilityManager>();
    }

    //set the display thing's values
    public IEnumerator SetValues(FileBrowserUpdate fileBrowserUpdate, CardData _cardData)
    {
        cardData = _cardData; //add a reference to cardData stored in this prefab

        nameText.text = _cardData.cardName;
        healthText.text = _cardData.cardHP.ToString();
        attackText.text = _cardData.cardAttack.ToString();
        energyText.text = _cardData.cardEnergy.ToString();

        //load ability text
        abilityManager = GameObject.FindWithTag("DeckCreatorManager").GetComponent<AbilityManager>();
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
            cardBackground.color = bgColour;
        }

        //load image and calculate offset
        float initialImageHeight = 358f;
        //the yield return means the code below will execute after the coroutine has finished
        yield return StartCoroutine(fileBrowserUpdate.LoadImage(_cardData.cardArtPath, cardArt));

        float height = cardArt.rectTransform.rect.height - initialImageHeight; //calculate height value
        double valueToAdd = (_cardData.cardOffsetY) * (height);
        cardArt.rectTransform.localPosition = new Vector3(0, (float)valueToAdd, 0); //change image height
    }

    public void DeleteCard()
    {
        //remove the card from the cardDataList then destory the gameobject
        deckCreator.cardDataList.Remove(cardData);
        Destroy(gameObject);
    }
}

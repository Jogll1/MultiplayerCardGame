using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardMakerUI : MonoBehaviour
{
    private DeckCreator deckCreator;
    private FileBrowserUpdate fileBrowserUpdate;
    private AbilityManager abilityManager;

    [Header("Card Elements")]
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI attackText;
    [SerializeField] TextMeshProUGUI abilityText;
    [SerializeField] TextMeshProUGUI energyText;
    [SerializeField] Image cardBackground;
    public string imagePath;
    public string hex;
    public RawImage cardArt;

    [Header("Editor Elements")]
    //name
    [SerializeField] TMP_InputField nameInputField;
    //health
    [SerializeField] TMP_InputField healthInputField;
    //attack
    [SerializeField] TMP_InputField attackInputField;
    //ability
    [SerializeField] TMP_Dropdown abilityDropdown;
    //colour selector
    [SerializeField] TMP_InputField hexInputField;
    //art offset
    [SerializeField] Scrollbar artOffsetScrollbar;

    private float initialImageHeight = 358f; //try to dynamically find this i guess

    // Start is called before the first frame update
    void Start()
    {
        deckCreator = GameObject.FindWithTag("DeckCreatorManager").GetComponent<DeckCreator>();
        fileBrowserUpdate = GameObject.FindWithTag("FileManager").GetComponent<FileBrowserUpdate>();

        //ability dropdown
        abilityManager = GameObject.FindWithTag("DeckCreatorManager").GetComponent<AbilityManager>();
        abilityDropdown.ClearOptions();
        for (int i = 0; i < abilityManager.abilitiesIndex.Count; i++)
        {
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            newOption.text = abilityManager.abilitiesIndex[i].abilityName;
            abilityDropdown.options.Add(newOption);
        }

        ResetCardTemplate();
    }

    // Update is called once per frame
    void Update()
    {
        //length and character limits
        healthInputField.characterLimit = 1;
        attackInputField.characterLimit = 1;
        hexInputField.characterLimit = 6;

        healthInputField.characterValidation = TMP_InputField.CharacterValidation.Integer;
        attackInputField.characterValidation = TMP_InputField.CharacterValidation.Integer;

        healthInputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return MyValidate(addedChar); };
        attackInputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return MyValidate(addedChar); };

        //name
        if (nameInputField.text != "")
        {
            nameText.text = nameInputField.text;
        }
        else
        {
            nameText.text = "Name";
        }

        //health
        if (healthInputField.text != "")
        {
            healthText.text = healthInputField.text;
        }
        else
        {
            healthText.text = "0";
        }

        //attack
        if (attackInputField.text != "")
        {
            attackText.text = attackInputField.text;
        }
        else
        {
            attackText.text = "0";
        }

        //ability
        AbilitySO ability = abilityManager.abilitiesIndex[abilityDropdown.value];
        if (ability.abilityType == "None" || ability.abilityType == "During Play")
        {
            abilityText.text = ability.abilityDescription;
        }
        else
        {
            abilityText.text = ability.abilityType + ": " + ability.abilityDescription;
        }

        //energy
        int energy = Mathf.RoundToInt((float.Parse(attackText.text) + float.Parse(healthText.text) + (float)abilityManager.abilitiesIndex[abilityDropdown.value].abilityCost) * 0.51f);
        if (energy > 9) energy = 9;
        energyText.text = energy.ToString();

        //bg colour
        if (hexInputField.text == "")
        {
            hex = "#FFFFFF";
        }
        else
        {
            hex = "#" + hexInputField.text;
        }

        Color bgColour;
        if (ColorUtility.TryParseHtmlString(hex, out bgColour))
        {
            cardBackground.color = bgColour;
        }

        //art height scrollbar
        //calculate height value
        float height = cardArt.rectTransform.rect.height - initialImageHeight;
        double valueToAdd = (artOffsetScrollbar.value - 0.5) * (height);
        //change image height
        cardArt.rectTransform.localPosition = new Vector3(0, (float)valueToAdd, 0);
    }

    public void ResetCardTemplate()
    {
        //reset card creator menu
        hex = "#FFFFFF";
        hexInputField.text = "";

        nameText.text = "";
        healthText.text = "1";
        attackText.text = "1";
        abilityText.text = "";

        nameInputField.text = "";
        healthInputField.text = "1";
        attackInputField.text = "1";
        abilityDropdown.value = 0;
        artOffsetScrollbar.value = 0.5f;

        imagePath = "";

        //reset card texture
        cardArt.texture = null;
    }

    //save the card you are creating
    public void SaveCardDesign()
    {
        //store data in an object and add it to the list of cardData objects
        CardData cardData = new CardData(deckCreator.cardCount + 1, nameText.text,
            int.Parse(healthText.text),
            int.Parse(attackText.text),
            int.Parse(energyText.text),
            abilityDropdown.value,
            imagePath,
            hex,
            artOffsetScrollbar.value - 0.5);

        // Debug.Log("Card Data: Card Index: " + cardData.cardIndex
        //         + ", Card Name: " + cardData.cardName
        //         + ", Card HP: " + cardData.cardHP
        //         + ", Card Attack: " + cardData.cardAttack
        //         + ", Path: " + cardData.cardArtPath
        //         + ", Card Hex: " + cardData.cardHex
        //         + ", Card Offset Y: " + cardData.cardOffsetY);

        //add to list in deckcreator script
        deckCreator.AddToCardDataList(cardData);
        //create a card template thing
        StartCoroutine(deckCreator.CreateCardDisplayPrefab(cardData));
    }

    //specific character validation method
    private char MyValidate(char charToValidate)
    {
        //Cant be -
        if (charToValidate == '-' || charToValidate == '0')
        {
            // ... if it is change it to an empty character.
            charToValidate = '\0';
        }

        return charToValidate;
    }
}

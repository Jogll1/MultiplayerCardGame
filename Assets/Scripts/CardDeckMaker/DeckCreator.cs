using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckCreator : MonoBehaviour
{
    private FileBrowserUpdate fileBrowserUpdate;

    public Canvas deckCreatorMenu;
    public Canvas cardCreatorMenu;
    public GameObject cardDisplayPrefab;
    public Transform cardPrefabsHolder;

    public int cardCount = -1; //temp;

    public Scrollbar scrollbar;

    private string fileName = "deckDataFile.json";

    [Space]
    //list of cards (cardData objects) in deck
    public List<CardData> cardDataList = new List<CardData>();

    // Start is called before the first frame update
    void Start()
    {
        fileBrowserUpdate = GameObject.FindWithTag("FileManager").GetComponent<FileBrowserUpdate>();

        OpenDeckCreatorMenu();
        CloseCardCreatorMenu();

        //on start load saved deck
        cardDataList = SaveCardData.ReadFromJson<CardData>(fileName);
        //create card display prefabs for each saved cardData object
        foreach (CardData _cardData in cardDataList)
        {
            StartCoroutine(CreateCardDisplayPrefab(_cardData));
        }

        scrollbar.value = 1; //make sure scrollbar starts at top
    }

    public void AddToCardDataList(CardData _cardData)
    {
        //add cardData object to list
        cardDataList.Add(_cardData);
    }

    public IEnumerator CreateCardDisplayPrefab(CardData _cardData)
    {
        //create a prefab to display the values of a card
        //instantiate prefab
        GameObject cardTempPrefab = Instantiate(cardDisplayPrefab, Vector3.zero, Quaternion.identity, cardPrefabsHolder);

        //apply card display prefab values
        CardDisplayPrefab cdp = cardTempPrefab.GetComponent<CardDisplayPrefab>();

        //update the prefab's values
        yield return StartCoroutine(cdp.SetValues(fileBrowserUpdate, _cardData));
    }

    public void SaveCardDeck()
    {
        SaveCardData.SaveToJson<CardData>(cardDataList, fileName);
        Debug.Log("Saved!");
    }

    #region Open/Close Menus
    public void OpenDeckCreatorMenu()
    {
        deckCreatorMenu.enabled = true;
    }

    public void CloseDeckCreatorMenu()
    {
        deckCreatorMenu.enabled = false;
    }

    public void OpenCardCreatorMenu()
    {
        cardCreatorMenu.enabled = true;
    }

    public void CloseCardCreatorMenu()
    {
        cardCreatorMenu.enabled = false;
    }
    #endregion
}

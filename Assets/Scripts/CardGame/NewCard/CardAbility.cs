using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility : MonoBehaviour
{
    public AbilitySO ability;
    private NewCardsManager cardsManager;

    private void Start()
    {
        cardsManager = GameObject.FindWithTag("CardsManager").GetComponent<NewCardsManager>();

        AbilityManager abilityManager = GameObject.FindWithTag("GameManager").GetComponent<AbilityManager>();
        AbilitySO ability = abilityManager.abilitiesIndex[GetComponent<CardPlayData>().cardData.cardAbilityIndex];
    }

    //----------ABILITIES TEST----------
    public void OnPlay()
    {
        Debug.Log("On Play");
        // if (ability.abilityEnum == AbilityEnum.DrawXCards)
        // {
        //     DrawXCards(1);
        // }
        // Debug.Log(ability.abilityEnum); //problem with this hmmm
    }

    #region Code of abilities

    //On Play abilities
    public void DrawXCards(int x)
    {
        for (int i = 0; i < x; i++)
        {
            cardsManager.DrawCard();
        }
    }

    #endregion
}

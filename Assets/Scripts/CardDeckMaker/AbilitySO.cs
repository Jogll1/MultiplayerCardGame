using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "AbilitySO", order = 1)]
public class AbilitySO : ScriptableObject
{
    // public AbilityEnum abilityEnum;
    public string abilityName;
    public string abilityDescription;
    public int abilityCost;
    public string abilityType;
}

// public enum AbilityType
// {
//     OnPlay,
//     OnDestroy,
//     DuringPlay,
//     None
// }

//enum for storing all abilities //----------ABILITIES TEST----------
// public enum AbilityEnum
// {
//     None,
//     DrawXCards, //on play
//     TakesXLessDamage,
//     CantBeOneShot
// }
// I probably need an ability class which stores the ability's type ?

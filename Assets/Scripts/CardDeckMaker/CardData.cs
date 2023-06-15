using System;

[Serializable] //to view inspector
public class CardData
{
    //attributes for cardData object
    public int cardIndex; //unused
    public string cardName;
    public int cardHP;
    public int cardAttack;
    public int cardEnergy;
    public int cardAbilityIndex;
    public string cardArtPath;
    public string cardHex;
    public double cardOffsetY;

    //constructor
    public CardData() //to get rid of the error when this object is used in Rpcs, cmds etc
    {

    }

    public CardData(int index, string name, int hp, int attack, int energy, int abilityIndex, string path, string hex, double offsetY)
    {
        cardIndex = index;
        cardName = name;
        cardHP = hp;
        cardAttack = attack;
        cardEnergy = energy;
        cardAbilityIndex = abilityIndex;
        cardArtPath = path;
        cardHex = hex;
        cardOffsetY = offsetY;
    }
}

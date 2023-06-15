using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnergyUIManager : NetworkBehaviour
{
    public List<GameObject> energySymbols;
    public List<GameObject> energySymbolSpaces;

    public GamePlayer gamePlayer;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < energySymbols.Count; i++)
        {
            energySymbols[i].SetActive(false);
            energySymbolSpaces[i].SetActive(false);
        }

        for (int i = 0; i < 1; i++) //fix - change if start with less or more
        {
            energySymbols[i].SetActive(true);
            energySymbolSpaces[i].SetActive(true);
        }
    }

    public void UpdateEnergyUI()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        gamePlayer = networkIdentity.GetComponent<GamePlayer>();

        for (int i = 0; i < energySymbolSpaces.Count; i++)
        {
            if (i < gamePlayer.maxEnergy)
            {
                energySymbolSpaces[i].SetActive(true);
            }
            else
            {
                energySymbolSpaces[i].SetActive(false);
            }
        }

        for (int i = 0; i < energySymbols.Count; i++)
        {
            if (i < gamePlayer.currentEnergy)
            {
                energySymbols[i].SetActive(true);
            }
            else
            {
                energySymbols[i].SetActive(false);
            }
        }
    }
}

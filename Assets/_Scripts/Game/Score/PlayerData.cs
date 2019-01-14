using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : PersistantData
{
    public int amountTimePlayed = 0;

    /// <summary>
    /// reset toute les valeurs à celle d'origine pour le jeu
    /// </summary>
    public void SetDefault()
    {
        amountTimePlayed = 0;
    }

    public override string GetFilePath ()
	{
		return "playerData.dat";
	}
}
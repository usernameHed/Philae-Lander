using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Fonctions utile
/// <summary>
public static class GameData
{
    public enum Event
    {
        GameOver,                   //event game over
    };

    public enum PoolTag
    {
        None,
        Jump,
        Hit,
        IA,
    };

    public enum Tags
    {
        Player,
        Coin,
        Enemy,
        Object,
    }
}

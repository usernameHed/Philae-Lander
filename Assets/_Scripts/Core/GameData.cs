﻿using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Fonctions utile
/// <summary>
public static class GameData
{
    #region core script

    public static String[] keys =
    {
        "None",
        "Backspace",
        "Tab",
        "Clear",
        "Return",
        "Pause",
        "Escape",
        "Space",
        "Exclaim",
        "DoubleQuote",
        "Hash",
        "Dollar",
        "Ampersand",
        "Quote",
        "LeftParen",
        "RightParen",
        "Asterisk",
        "Plus",
        "Comma",
        "Minus",
        "Period",
        "Slash",
        "Alpha0",
        "Alpha1",
        "Alpha2",
        "Alpha3",
        "Alpha4",
        "Alpha5",
        "Alpha6",
        "Alpha7",
        "Alpha8",
        "Alpha9",
        "Colon",
        "Semicolon",
        "Less",
        "Equals",
        "Greater",
        "Question",
        "At",
        "LeftBracket",
        "Backslash",
        "RightBracket",
        "Caret",
        "Underscore",
        "BackQuote",
        "A",
        "B",
        "C",
        "D",
        "E",
        "F",
        "G",
        "H",
        "I",
        "J",
        "K",
        "L",
        "M",
        "N",
        "O",
        "P",
        "Q",
        "R",
        "S",
        "T",
        "U",
        "V",
        "W",
        "X",
        "Y",
        "Z",
        "Delete",
        "Keypad0",
        "Keypad1",
        "Keypad2",
        "Keypad3",
        "Keypad4",
        "Keypad5",
        "Keypad6",
        "Keypad7",
        "Keypad8",
        "Keypad9",
        "KeypadPeriod",
        "KeypadDivide",
        "KeypadMultiply",
        "KeypadMinus",
        "KeypadPlus",
        "KeypadEnter",
        "KeypadEquals",
        "UpArrow",
        "DownArrow",
        "RightArrow",
        "LeftArrow",
        "Insert",
        "Home",
        "End",
        "PageUp",
        "PageDown",
        "F1",
        "F2",
        "F3",
        "F4",
        "F5",
        "F6",
        "F7",
        "F8",
        "F9",
        "F10",
        "F11",
        "F12",
        "F13",
        "F14",
        "F15",
        "Numlock",
        "CapsLock",
        "ScrollLock",
        "RightShift",
        "LeftShift",
        "RightControl",
        "LeftControl",
        "RightAlt",
        "LeftAlt",
        "RightCommand",
        "RightApple",
        "LeftCommand",
        "LeftApple",
        "LeftWindows",
        "RightWindows",
        "AltGr",
        "Help",
        "Print",
        "SysReq",
        "Break",
        "Menu"
    };

    public enum Event
    {
        GameOver,                   //event game over
        GamePadConnectionChange,    //event when gamePad connection change
        SceneLoaded,                //called when a scene is loaded
        AdditiveJustFinishLoad,     //called when a scene is finish loading additivly
    };

    public enum Tags
    {
        None,
        Player,
        Floor,
        Enemy,
    };

    public enum Layers
    {
        Player,
        Obstacle,
        Default,
        Object,
        Colliders,
        Entity,
        OnlyIA,
        Hurt,
        RedEye,
    }

    public enum Sounds
    {
        Bump,
    }
    #endregion
}

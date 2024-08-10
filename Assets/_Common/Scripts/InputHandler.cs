using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerIndex
{
    Player1,
    Player2,

    None,
}

public enum InputKey
{
    //Both players
    ActionZBoth,
    ActionXBoth,
    ActionCBoth,
    UnusedJBoth,
    UnusedKBoth,
    UnusedLBoth,
    //Player1
    ActionZ, 
    ActionX, 
    ActionC, 
    // Player2
    ActionV, 
    ActionB, 
    ActionN, 
    // Unused but defined
    UnusedY, 
    UnusedU, 
    UnusedI, 
    UnusedJ, 
    UnusedK, 
    UnusedL, 
    // System
    Pause, 
    Escape, 
    Confirm, // One of all actions and unused of players
    Player2Start,
    // Debug
    Debug1, 
    Debug2,
}

public static class InputHandler
{
    private static Dictionary<InputKey, List<KeyCode>> KeyBindings = new Dictionary<InputKey, List<KeyCode>>()
    {
       { InputKey.ActionZ,      new List<KeyCode>(){KeyCode.Z,} },
       { InputKey.ActionX,      new List<KeyCode>(){KeyCode.X,} },
       { InputKey.ActionC,      new List<KeyCode>(){KeyCode.C,} },
       { InputKey.ActionV,      new List<KeyCode>(){KeyCode.V,} },
       { InputKey.ActionB,      new List<KeyCode>(){KeyCode.B,} },
       { InputKey.ActionN,      new List<KeyCode>(){KeyCode.N,} },
       { InputKey.UnusedJ,      new List<KeyCode>(){KeyCode.J,} },
       { InputKey.UnusedK,      new List<KeyCode>(){KeyCode.K,} },
       { InputKey.UnusedL,      new List<KeyCode>(){KeyCode.L,} },
       { InputKey.UnusedU,      new List<KeyCode>(){KeyCode.U,} },
       { InputKey.UnusedY,      new List<KeyCode>(){KeyCode.I,} },
       { InputKey.UnusedI,      new List<KeyCode>(){KeyCode.O,} },
       { InputKey.Escape,       new List<KeyCode>(){KeyCode.M,} },
       { InputKey.Pause,        new List<KeyCode>(){KeyCode.P,} },
       { InputKey.Debug1,       new List<KeyCode>(){KeyCode.Q,} },
       { InputKey.Debug2,       new List<KeyCode>(){KeyCode.E,} },
       { InputKey.Confirm,      new List<KeyCode>(){KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.L, KeyCode.J, KeyCode.K, KeyCode.U, KeyCode.O, KeyCode.I} },
       { InputKey.Player2Start, new List<KeyCode>(){KeyCode.V, KeyCode.B, KeyCode.N,KeyCode.U, KeyCode.O, KeyCode.I,}},
       { InputKey.ActionZBoth,  new List<KeyCode>(){KeyCode.Z, KeyCode.V}},
       { InputKey.ActionXBoth,  new List<KeyCode>(){KeyCode.X, KeyCode.B}},
       { InputKey.ActionCBoth,  new List<KeyCode>(){KeyCode.C, KeyCode.N}},
       { InputKey.UnusedJBoth,  new List<KeyCode>(){KeyCode.J, KeyCode.U}},
       { InputKey.UnusedKBoth,  new List<KeyCode>(){KeyCode.K, KeyCode.I}},
       { InputKey.UnusedLBoth,  new List<KeyCode>(){KeyCode.L, KeyCode.O}},
    };

    public static float GetHorizontal(PlayerIndex index = PlayerIndex.None){
        switch(index){
            case PlayerIndex.Player1: return Input.GetAxisRaw("Horizontal");
            case PlayerIndex.Player2: return Input.GetAxisRaw("Horizontal2");
        }

        return Math.Min(Math.Max(-1, Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Horizontal2")), 1);
    }

    public static float GetVertical(PlayerIndex index = PlayerIndex.None){
       switch(index){
            case PlayerIndex.Player1: return Input.GetAxisRaw("Vertical");
            case PlayerIndex.Player2: return Input.GetAxisRaw("Vertical2");
        }

        return Math.Min(Math.Max(-1, Input.GetAxisRaw("Vertical") + Input.GetAxisRaw("Vertical2")), 1);
    }

    public static bool GetKey(InputKey key, bool keyDown = true)
    {
        List<KeyCode> keys = KeyBindings[key];
        for(int i = 0; i < keys.Count; i++){
            KeyCode code = keys[i];
            bool isPressed = keyDown ? Input.GetKeyDown(code) : Input.GetKey(code);
            if(isPressed) return isPressed;
        }

        return false;
    }
}

public static class PlayerList<PlayerType>
{
    private static PlayerType[] _playerList = new PlayerType[(int)PlayerIndex.None];
    public static void Register(PlayerIndex index, PlayerType playerInstance)
    {
        _playerList[(int)index] = playerInstance;
    }

    public static PlayerType Get(PlayerIndex index)
    {
        return _playerList[(int)index];
    }
}

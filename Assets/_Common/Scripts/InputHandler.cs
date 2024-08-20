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
    Action_1_Both,
    Action_2_Both,
    Action_3_Both,
    Action_4_Both,
    Action_5_Both,
    Action_6_Both,
    //Player1
    Action_1_Player1, 
    Action_2_Player1, 
    Action_3_Player1, 
    // Player2
    Action_1_Player2, 
    Action_2_Player2, 
    Action_3_Player2, 
    // Unused but defined
    Action_4_Player1, 
    Action_5_Player1, 
    Action_6_Player1, 
    Action_4_Player2, 
    Action_5_Player2, 
    Action_6_Player2, 
    // System
    Pause, 
    Escape, 
    Confirm, // One of all actions and unused of players
    Action_Any_Player2,
    Action_Any_Player1,
    // Debug
    Debug1, 
    Debug2,
}

public static class InputHandler
{
    private static Dictionary<InputKey, List<KeyCode>> KeyBindings = new Dictionary<InputKey, List<KeyCode>>()
    {
       { InputKey.Action_1_Player1,   new List<KeyCode>(){KeyCode.Z,} },
       { InputKey.Action_2_Player1,   new List<KeyCode>(){KeyCode.X,} },
       { InputKey.Action_3_Player1,   new List<KeyCode>(){KeyCode.C,} },
       { InputKey.Action_1_Player2,   new List<KeyCode>(){KeyCode.V,} },
       { InputKey.Action_2_Player2,   new List<KeyCode>(){KeyCode.B,} },
       { InputKey.Action_3_Player2,   new List<KeyCode>(){KeyCode.N,} },
       { InputKey.Action_4_Player2,   new List<KeyCode>(){KeyCode.J,} },
       { InputKey.Action_5_Player2,   new List<KeyCode>(){KeyCode.K,} },
       { InputKey.Action_6_Player2,   new List<KeyCode>(){KeyCode.L,} },
       { InputKey.Action_5_Player1,   new List<KeyCode>(){KeyCode.U,} },
       { InputKey.Action_4_Player1,   new List<KeyCode>(){KeyCode.I,} },
       { InputKey.Action_6_Player1,   new List<KeyCode>(){KeyCode.O,} },
       { InputKey.Escape,             new List<KeyCode>(){KeyCode.M,} },
       { InputKey.Pause,              new List<KeyCode>(){KeyCode.P,} },
       { InputKey.Debug1,             new List<KeyCode>(){KeyCode.Q,} },
       { InputKey.Debug2,             new List<KeyCode>(){KeyCode.E,} },
       { InputKey.Confirm,            new List<KeyCode>(){KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.L, KeyCode.J, KeyCode.K, KeyCode.U, KeyCode.O, KeyCode.I} },
       { InputKey.Action_Any_Player2, new List<KeyCode>(){KeyCode.V, KeyCode.B, KeyCode.N,KeyCode.U, KeyCode.O, KeyCode.I,}},
       { InputKey.Action_Any_Player1, new List<KeyCode>(){KeyCode.Z, KeyCode.X, KeyCode.C,KeyCode.J, KeyCode.K, KeyCode.L,}},
       { InputKey.Action_1_Both,      new List<KeyCode>(){KeyCode.Z, KeyCode.V}},
       { InputKey.Action_2_Both,      new List<KeyCode>(){KeyCode.X, KeyCode.B}},
       { InputKey.Action_3_Both,      new List<KeyCode>(){KeyCode.C, KeyCode.N}},
       { InputKey.Action_4_Both,      new List<KeyCode>(){KeyCode.J, KeyCode.U}},
       { InputKey.Action_5_Both,      new List<KeyCode>(){KeyCode.K, KeyCode.I}},
       { InputKey.Action_6_Both,      new List<KeyCode>(){KeyCode.L, KeyCode.O}},
    };

    public static float GetHorizontal(PlayerIndex index = PlayerIndex.None){
        switch(index){
            case PlayerIndex.Player1: return Input.GetAxisRaw("Horizontal");
            case PlayerIndex.Player2: return Input.GetAxisRaw("Horizontal2");
        }

        float sum = Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Horizontal2");
        if(Mathf.Abs(sum) < 0.01f){ return 0; }
        if(Mathf.Abs(sum) <= 1f) { return sum; }
        return Math.Sign(sum);
    }

    public static float GetVertical(PlayerIndex index = PlayerIndex.None){
       switch(index){
            case PlayerIndex.Player1: return Input.GetAxisRaw("Vertical");
            case PlayerIndex.Player2: return Input.GetAxisRaw("Vertical2");
        }

        float sum = Input.GetAxisRaw("Vertical") + Input.GetAxisRaw("Vertical2");
        if(Mathf.Abs(sum) < 0.01f){ return 0; }
        if(Mathf.Abs(sum) <= 1f) { return sum; }
        return Math.Sign(sum);
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

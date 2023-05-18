using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneFlowController 
{
    private static Dictionary<string, List<string>> flow = new Dictionary<string, List<string>>();

    static SceneFlowController(){

        flow["Intro"] = new List<string>{
            #if T_SegmentSpawner
                "AsteroidsMain",
            #elif BERZERK_GAME
                "BerzerkMain",
            #elif DIGDUG_GAME
                "DigDug2Main",
            #elif FROGGER_GAME
                "FroggerMain",
            #elif THREE_GAME
                "GameSelect",
            #elif LITTLE_FIGHTER_GAME
                "LittleFighterMain",
            #elif SPACE_BASE_GAME
                "SpaceBaseMain",
            #elif T3_GAMES_2
                "GameSelect2"
            #elif TUNNEL_GAME
                "TunnelMain"
            #endif
        };
        
        flow["Intro1"] = new List<string>{
            #if T_SegmentSpawner
                "AsteroidsMain",
            #elif BERZERK_GAME
                "BerzerkMain",
            #elif DIGDUG_GAME
                "DigDug2Main",
            #elif FROGGER_GAME
                "FroggerMain",
            #elif THREE_GAME
                "GameSelect",
            #elif LITTLE_FIGHTER_GAME
                "LittleFighterMain",
            #elif SPACE_BASE_GAME
                "SpaceBaseMain",
            #elif T3_GAMES_2
                "GameSelect2"
            #elif TUNNEL_GAME
                "TunnelMain"
            #elif ALL_GAMES
                "GameSelect3"
            #endif
        };

        flow["GameSelect"]     = new List<string>{
            "AsteroidsMain", 
            "FroggerMain", 
            "DigDug2Main"
        };
        flow["GameSelect2"]     = new List<string>{
            "SpaceBaseMain", 
            "BerzerkMain", 
            "LittleFighterMain",
            "TunnelMain",
            "FroggerMain",
            "AsteroidsMain",
            "DigDug2Main"
        };

        flow["GameSelect3"]     = new List<string>{
            "SpaceBaseMain", 
            "BerzerkMain", 
            "LittleFighterMain",
            "TunnelMain",
            "FroggerMain",
            "AsteroidsMain",
            "DigDug2Main",
            "DigDugMain"
        };

        flow["Asteroids"]      = new List<string>{"AsteroidsOutro"};
        flow["AsteroidsIntro"] = new List<string>{"Asteroids"};
        flow["AsteroidsMain"]  = new List<string>{"AsteroidsIntro"};
        flow["AsteroidsOutro"] = new List<string>{"EOutro",
            "AsteroidsMain",
            "GameSelect3",};

        flow["DigDug2"]         = new List<string>{
            "DigDug2Outro",
            "Intro1",
            "DigDug2",

        };
        flow["DigDug2Intro"]    = new List<string>{"DigDug2"};
        flow["DigDug2Main"]     = new List<string>{"DigDug2Intro"};
        flow["DigDug2Outro"]    = new List<string>{
            "EOutro",
            "DigDug2Main",
            "GameSelect3",};

        flow["Frogger"]        = new List<string>{"FroggerOutro", "FroggerOutro1", "Intro1"};
        flow["FroggerIntro"]   = new List<string>{"Frogger"};
        flow["FroggerMain"]    = new List<string>{"FroggerIntro"};
        flow["FroggerOutro"]   = new List<string>{"EOutro",
            "FroggerMain",
            "GameSelect3",};
        flow["FroggerOutro1"]  = new List<string>{"EOutro",
            "FroggerMain",
            "GameSelect3",
        };

        flow["Berzerk"]        = new List<string>{
            "BerzerkOutro",
            "Intro1",
            "Berzerk",
        };
        flow["BerzerkIntro"]   = new List<string>{"Berzerk"};
        flow["BerzerkMain"]    = new List<string>{"BerzerkIntro"};
        flow["BerzerkOutro"]   = new List<string>{
            "EOutro",
            "BerzerkMain",
            "GameSelect3",
        };
        
        flow["LittleFighter"]        = new List<string>{
            "LittleFighterOutro",
            "Intro1",
            "LittleFighter",
        };
        flow["LittleFighterIntro"]   = new List<string>{"LittleFighter"};
        flow["LittleFighterMain"]    = new List<string>{"LittleFighterIntro"};
        flow["LittleFighterOutro"]   = new List<string>{
            "EOutro",
            "LittleFighterMain",
            "GameSelect3",
            };

        flow["SpaceBase"]        = new List<string>{
            "SpaceBaseOutro",
            "Intro1",
            "SpaceBase",
        };
        flow["SpaceBaseIntro"]   = new List<string>{"SpaceBase"};
        flow["SpaceBaseMain"]    = new List<string>{"SpaceBaseIntro"};
        flow["SpaceBaseOutro"]   = new List<string>{
            "EOutro",
            "SpaceBaseMain",
            "GameSelect3"};

        flow["Tunnel"]        = new List<string>{
            "TunnelOutro",
            "Intro1",
            "SpaceBase",
        };
        flow["TunnelIntro"]   = new List<string>{"Tunnel"};
        flow["TunnelMain"]    = new List<string>{"TunnelIntro"};
        flow["TunnelOutro"]   = new List<string>{
            "EOutro",
            "TunnelMain",
            "GameSelect3"};
        flow["EOutro"]        = new List<string>{"Intro1"};
    }

    public static string GetNextScene(int index){
        Debug.Log("Loading Scene : " + SceneManager.GetActiveScene().name);
        return flow[SceneManager.GetActiveScene().name][index];
    }
}

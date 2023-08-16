using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneFlowController 
{
    private static Dictionary<string, List<string>> flow = new Dictionary<string, List<string>>();

    public static string GetActiveIntro(){
        #if INTRO3
            return "Intro3";
        #elif INTRO2
                return "Intro2";
        #else 
                return "Intro1";
        #endif
    }

    static string GetActiveOutrio(){
        #if SKIP_EU_OUTRO
                return GetActiveIntro();
        #else 
                return "EOutro";
        #endif
    }

    static string GetGameSelect(){
            #if THREE_GAME
                return "GameSelect";
            #elif T3_GAMES_2
                return "GameSelect2";
            #elif ALL_GAMES
                return "GameSelect3";
            #elif SIX_GAMES
                return "GameSelect4";
            #else 
                return GetActiveIntro();
            #endif
    }

    static SceneFlowController(){


        flow[GetActiveIntro()] = new List<string>{
            #if T_SegmentSpawner
                "AsteroidsMain",
            #elif BERZERK_GAME
                "BerzerkMain",
            #elif DIGDUG2_GAME
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
            #elif SIX_GAMES
                "GameSelect4"
            #elif DIGDUG_GAME
                "DigDugMain"
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

        flow["GameSelect4"]     = new List<string>{
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
        flow["AsteroidsOutro"] = new List<string>{
            GetActiveOutrio(),
            "AsteroidsMain",
            GetGameSelect(),};

        flow["DigDug2"]         = new List<string>{
            "DigDug2Outro",
            GetActiveIntro(),
            "DigDug2",

        };
        flow["DigDug2Intro"]    = new List<string>{"DigDug2"};
        flow["DigDug2Main"]     = new List<string>{"DigDug2Intro"};
        flow["DigDug2Outro"]    = new List<string>{
            GetActiveOutrio(),
            "DigDug2Main",
            GetGameSelect(),};


        flow["DigDug"]         = new List<string>{
            "DigDugOutro",
            GetActiveIntro(),
            "DigDug",
        };
        flow["DigDugIntro"]    = new List<string>{"DigDug"};
        flow["DigDugMain"]     = new List<string>{"DigDugIntro"};
        flow["DigDugOutro"]    = new List<string>{
            GetActiveOutrio(),
            "DigDugMain",
            GetGameSelect(),};

        flow["Frogger"]        = new List<string>{
            "FroggerOutro", 
            "FroggerOutro1",             
            GetActiveIntro()
            };
        flow["FroggerIntro"]   = new List<string>{"Frogger"};
        flow["FroggerMain"]    = new List<string>{"FroggerIntro"};
        flow["FroggerOutro"]   = new List<string>{GetActiveOutrio(),
            "FroggerMain",
            GetGameSelect(),};
        flow["FroggerOutro1"]  = new List<string>{GetActiveOutrio(),
            "FroggerMain",
            GetGameSelect(),
        };

        flow["Berzerk"]        = new List<string>{
            "BerzerkOutro",
            GetActiveIntro(),
            "Berzerk",
        };
        flow["BerzerkIntro"]   = new List<string>{"Berzerk"};
        flow["BerzerkMain"]    = new List<string>{"BerzerkIntro"};
        flow["BerzerkOutro"]   = new List<string>{
            GetActiveOutrio(),
            "BerzerkMain",
            GetGameSelect(),
        };
        
        flow["LittleFighter"]        = new List<string>{
            "LittleFighterOutro",
            GetActiveIntro(),
            "LittleFighter",
        };
        flow["LittleFighterIntro"]   = new List<string>{"LittleFighter"};
        flow["LittleFighterMain"]    = new List<string>{"LittleFighterIntro"};
        flow["LittleFighterOutro"]   = new List<string>{
            GetActiveOutrio(),
            "LittleFighterMain",
            GetGameSelect(),
            };

        flow["SpaceBase"]        = new List<string>{
            "SpaceBaseOutro",
            GetActiveIntro(),
            "SpaceBase",
        };
        flow["SpaceBaseIntro"]   = new List<string>{"SpaceBase"};
        flow["SpaceBaseMain"]    = new List<string>{"SpaceBaseIntro"};
        flow["SpaceBaseOutro"]   = new List<string>{
            GetActiveOutrio(),
            "SpaceBaseMain",
            GetGameSelect()};

        flow["Tunnel"]        = new List<string>{
            "TunnelOutro",
            GetActiveIntro(),
            "SpaceBase",
        };
        flow["TunnelIntro"]   = new List<string>{"Tunnel"};
        flow["TunnelMain"]    = new List<string>{"TunnelIntro"};
        flow["TunnelOutro"]   = new List<string>{
            GetActiveOutrio(),
            "TunnelMain",
            GetGameSelect()};
        flow["EOutro"]        = new List<string>{            
            GetActiveIntro(),
            };
    }

    public static string GetNextScene(int index){
        Debug.Log("Loading Scene : " + SceneManager.GetActiveScene().name);
        return flow[SceneManager.GetActiveScene().name][index];
    }
}

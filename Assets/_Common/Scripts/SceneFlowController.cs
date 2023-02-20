using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneFlowController 
{
    private static Dictionary<string, List<string>> flow = new Dictionary<string, List<string>>();

    static SceneFlowController(){

        flow["Intro"]          = new List<string>{

        #if ASTEROIDS_GAME
            "AsteroidsMain",
        #elif BERZERK_GAME
            "BerzerkMain",
        #elif DIGDUG_GAME
            "DigDugMain",
        #elif FROGGER_GAME
            "FroggerMain",
        #elif THREE_GAME
            "GameSelect",
        #endif
        };
        
        flow["GameSelect"]     = new List<string>{
            "AsteroidsMain", 
            "FroggerMain", 
            "DigDugMain"
        };
        flow["Asteroids"]      = new List<string>{"AsteroidsOutro"};
        flow["AsteroidsIntro"] = new List<string>{"Asteroids"};
        flow["AsteroidsMain"]  = new List<string>{"AsteroidsIntro"};
        flow["AsteroidsOutro"] = new List<string>{"Intro"};

        flow["DigDug"]         = new List<string>{
            "DigDugOutro",
            "Intro",
            "DigDug",

        };
        flow["DigDugIntro"]    = new List<string>{"DigDug"};
        flow["DigDugMain"]     = new List<string>{"DigDugIntro"};
        flow["DigDugOutro"]    = new List<string>{
            "Intro",
            "DigDug"};

        flow["Frogger"]        = new List<string>{"FroggerOutro", "FroggerOutro1", "Intro"};
        flow["FroggerIntro"]   = new List<string>{"Frogger"};
        flow["FroggerMain"]    = new List<string>{"FroggerIntro"};
        flow["FroggerOutro"]   = new List<string>{"Intro"};
        flow["FroggerOutro1"]  = new List<string>{"Intro"};

        flow["Berzerk"]        = new List<string>{
            "BerzerkOutro",
            "Intro",
            "Berzerk",
        };
        flow["BerzerkIntro"]   = new List<string>{"Berzerk"};
        flow["BerzerkMain"]    = new List<string>{"BerzerkIntro"};
        flow["BerzerkOutro"]   = new List<string>{"Intro"};
    }

    public static string GetNextScene(int index){
        Debug.Log(SceneManager.GetActiveScene().name);
        return flow[SceneManager.GetActiveScene().name][index];
    }
}
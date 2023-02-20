using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;



public static class CBuild
{

    #if UNITY_EDITOR
    private enum Game{
        Asteroids,
        Frogger,
        DigDug,
        AllThree,
        Berzerk,

        MAX,
    }

    static string _buildFolder = "_Build/";

    static Dictionary<Game, string> _paths = new Dictionary<Game, string>(){
        {Game.Asteroids, "Assets/Asteroids/Scenes/"},
        {Game.Berzerk, "Assets/Berzerk/Scenes/"},
        {Game.DigDug, "Assets/DigDug2/Scenes/"},
        {Game.Frogger, "Assets/Frogger/Scenes/"},
    };

    static Dictionary<Game, string[]> _directives = new Dictionary<Game, string[]>();
    static Dictionary<Game, string[]> _scenes = new Dictionary<Game, string[]>();

    private static void FillScenes(){
        _scenes[Game.Asteroids] = new string[] {
            "Assets/_Common/Intro.unity",
            _paths[Game.Asteroids] + "Asteroids.unity",
            _paths[Game.Asteroids] + "AsteroidsIntro.unity",
            _paths[Game.Asteroids] + "AsteroidsMain.unity",
            _paths[Game.Asteroids] + "AsteroidsOutro.unity",
        };
        _scenes[Game.Berzerk]   = new string[] {
            "Assets/_Common/Intro.unity",
            _paths[Game.Berzerk] + "Berzerk.unity",
            _paths[Game.Berzerk] + "BerzerkIntro.unity",
            _paths[Game.Berzerk] + "BerzerkMain.unity",
            _paths[Game.Berzerk] + "BerzerkOutro.unity",
        };
        _scenes[Game.DigDug]    = new string[] {
            "Assets/_Common/Intro.unity",
            _paths[Game.DigDug] + "DigDug.unity",
            _paths[Game.DigDug] + "DigDugIntro.unity",
            _paths[Game.DigDug] + "DigDugMain.unity",
            _paths[Game.DigDug] + "DigDugOutro.unity",
        };
        _scenes[Game.Frogger]   = new string[] {
            "Assets/_Common/Intro.unity",
            _paths[Game.Frogger] + "Frogger.unity",
            _paths[Game.Frogger] + "FroggerIntro.unity",
            _paths[Game.Frogger] + "FroggerMain.unity",
            _paths[Game.Frogger] + "FroggerOutro.unity",
            _paths[Game.Frogger] + "FroggerOutro1.unity",
        };
        _scenes[Game.AllThree]  = new string[] {
            "Assets/_Common/Intro.unity",
            "Assets/_Common/GameSelect.unity",
            _paths[Game.Asteroids] + "Asteroids.unity",
            _paths[Game.Asteroids] + "AsteroidsIntro.unity",
            _paths[Game.Asteroids] + "AsteroidsMain.unity",
            _paths[Game.Asteroids] + "AsteroidsOutro.unity",
            _paths[Game.DigDug] + "DigDug.unity",
            _paths[Game.DigDug] + "DigDugIntro.unity",
            _paths[Game.DigDug] + "DigDugMain.unity",
            _paths[Game.DigDug] + "DigDugOutro.unity",
            _paths[Game.Frogger] + "Frogger.unity",
            _paths[Game.Frogger] + "FroggerIntro.unity",
            _paths[Game.Frogger] + "FroggerMain.unity",
            _paths[Game.Frogger] + "FroggerOutro.unity",
            _paths[Game.Frogger] + "FroggerOutro1.unity",
        };
    }

    private static void FillDirectives(){
        _directives[Game.Asteroids] = new string[] {"ASTEROIDS_GAME"};
        _directives[Game.Berzerk]   = new string[] {"BERZERK_GAME"};
        _directives[Game.DigDug]    = new string[] {"DIGDUG_GAME"};
        _directives[Game.Frogger]   = new string[] {"FROGGER_GAME"};
        _directives[Game.AllThree]  = new string[] {"THREE_GAME"};
    }



//    [MenuItem("Build/Localized")]
//    public static void BuildLocalized(){
//        FillScenes();
//        FillDirectives();
//    }

    [MenuItem("Build/All")]
    public static void BuildAll(){
        BuildWebGL();
        BuildWindows();
    }

    [MenuItem("Build/Windows")]
    public static void BuildWindows(){
        BuildAll(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/WebGL")]
    public static void BuildWebGL(){
        BuildAll(BuildTargetGroup.WebGL, BuildTarget.WebGL);
    }


    private static void BuildAll(BuildTargetGroup targetGroup, BuildTarget platform){
        FillScenes();
        FillDirectives();

        

       //string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        for(Game gameId = Game.Asteroids; gameId < Game.MAX; gameId++){

            UnityEngine.Debug.Log( platform.ToString() + "Building " + gameId + " (" + (int)(gameId + 1) + "/" + (int)(Game.MAX) + ")");

            string path = GetPath(gameId, platform);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, _directives[gameId]);
            PlayerSettings.productName = gameId.ToString();
            PlayerSettings.bundleVersion = ((int)gameId).ToString();
            AssetDatabase.Refresh();

            BuildPipeline.BuildPlayer(
                _scenes[gameId], 
                path, 
                platform, 
                BuildOptions.None);

        //    Process proc = new Process();
        //    proc.StartInfo.FileName = path;
        //    proc.Start();
        }
    }

    private static string GetPath(Game gameId, BuildTarget platform){
        string path = _buildFolder + "/" + platform.ToString() + "/" + gameId.ToString() + "/";
        switch (platform) {
            case BuildTarget.StandaloneWindows64: path += gameId.ToString() + ".exe"; break;
        }
       return path;
    }




    
    public static void SwitchToNoPlatform() {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        defines = RemoveCompilerDefines(defines, "STEAM_BUILD", "GOG_BUILD");

        UnityEngine.Debug.Log("Compiling with DEFINE: '" + defines + "'");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines);
    }

    private static string AddCompilerDefines(string defines, params string[] toAdd){
        List<string> splitDefines = new List<string>(defines.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
        foreach (var add in toAdd)
        if (!splitDefines.Contains(add))
            splitDefines.Add(add);

        return string.Join(";", splitDefines.ToArray());
    }

    private static string RemoveCompilerDefines(string defines, params string[] toRemove){
        List<string> splitDefines = new List<string>(defines.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
        foreach(var remove in toRemove)
        splitDefines.Remove(remove);

        return string.Join(";", splitDefines.ToArray());
    }

    #endif
}

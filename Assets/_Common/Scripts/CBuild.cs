using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
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
        LittleFighter,
        SpaceBase,
        SecondThree,

        MAX,
    }

    static string _buildFolder = "_Build/";

    static Dictionary<Game, string> _paths = new Dictionary<Game, string>(){
        {Game.Asteroids, "Assets/Asteroids/Scenes/"},
        {Game.Berzerk, "Assets/Berzerk/Scenes/"},
        {Game.DigDug, "Assets/DigDug2/Scenes/"},
        {Game.Frogger, "Assets/Frogger/Scenes/"},
        {Game.LittleFighter, "Assets/LittleFighter/Scenes/"},
        {Game.SpaceBase, "Assets/SpaceBase/Scenes/"}
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
        _scenes[Game.LittleFighter]   = new string[] {
            "Assets/_Common/Intro.unity",
            _paths[Game.LittleFighter] + "LittleFighter.unity",
            _paths[Game.LittleFighter] + "LittleFighterIntro.unity",
            _paths[Game.LittleFighter] + "LittleFighterMain.unity",
            _paths[Game.LittleFighter] + "LittleFighterOutro.unity",
        };
        _scenes[Game.SpaceBase]   = new string[] {
            "Assets/_Common/Intro.unity",
            _paths[Game.SpaceBase] + "SpaceBase.unity",
            _paths[Game.SpaceBase] + "SpaceBaseIntro.unity",
            _paths[Game.SpaceBase] + "SpaceBaseMain.unity",
            _paths[Game.SpaceBase] + "SpaceBaseOutro.unity",
        };
        _scenes[Game.SecondThree] = new string[]{
            "Assets/_Common/Intro.unity",
            "Assets/_Common/GameSelect2.unity",
            _paths[Game.LittleFighter] + "LittleFighter.unity",
            _paths[Game.LittleFighter] + "LittleFighterIntro.unity",
            _paths[Game.LittleFighter] + "LittleFighterMain.unity",
            _paths[Game.LittleFighter] + "LittleFighterOutro.unity",
            _paths[Game.SpaceBase] + "SpaceBase.unity",
            _paths[Game.SpaceBase] + "SpaceBaseIntro.unity",
            _paths[Game.SpaceBase] + "SpaceBaseMain.unity",
            _paths[Game.SpaceBase] + "SpaceBaseOutro.unity",
            _paths[Game.Berzerk] + "Berzerk.unity",
            _paths[Game.Berzerk] + "BerzerkIntro.unity",
            _paths[Game.Berzerk] + "BerzerkMain.unity",
            _paths[Game.Berzerk] + "BerzerkOutro.unity",
        };
    }

    private static void FillDirectives(){
        _directives[Game.Asteroids] = new string[] {"ASTEROIDS_GAME"};
        _directives[Game.Berzerk]   = new string[] {"BERZERK_GAME"};
        _directives[Game.DigDug]    = new string[] {"DIGDUG_GAME"};
        _directives[Game.Frogger]   = new string[] {"FROGGER_GAME"};
        _directives[Game.AllThree]  = new string[] {"THREE_GAME"};
        _directives[Game.SecondThree] = new string[] { "T3_GAMES_2" };
        _directives[Game.LittleFighter] = new string[] {"LITTLE_FIGHTER_GAME"};
        _directives[Game.SpaceBase] = new string[] {"SPACE_BASE_GAME"};
    }


    private static void PrintTimeFormatted(long time, BuildTarget target){
        UnityEngine.Debug.Log(target.ToString() + "::BuildFinished takes :" + 
        ((int)(time/3600)).ToString().PadLeft(2,'0') + ":" + 
        ((int)((time%3600)/60)).ToString().PadLeft(2,'0')  + ":" + 
        ((int)(time%60)).ToString().PadLeft(2,'0'));
    }

    private static int GetCurrentTime(){
        DateTime now = DateTime.Now;
        return now.Hour * 3600 + now.Minute * 60 + now.Second;
    }

    private static void BuildAll(BuildTargetGroup targetGroup, BuildTarget platform){
        FillScenes();
        FillDirectives();

        for(Game gameId = Game.Asteroids; gameId < Game.MAX; gameId++){
            BuildGame(gameId, targetGroup, platform);
        }
    }

    private static void BuildGame(Game gameId, BuildTargetGroup targetGroup, BuildTarget platform){
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
    }


    private static string GetPath(Game gameId, BuildTarget platform){
        string path = _buildFolder + "/" + platform.ToString() + "/" + gameId.ToString() + "/";
        switch (platform) {
            case BuildTarget.StandaloneWindows64: path += gameId.ToString() + ".exe"; break;
        }
       return path;
    }
    
    
    private static string RemoveCompilerDefines(string defines, params string[] toRemove){
        List<string> splitDefines = new List<string>(defines.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
        foreach(var remove in toRemove)
        splitDefines.Remove(remove);

        return string.Join(";", splitDefines.ToArray());
    }

#region "UnityDisplay"

    [MenuItem("Build/Rebuild WebGL/Asteroids")]
    public static void BuildAsteroidsWebGL(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.Asteroids, BuildTargetGroup.WebGL, BuildTarget.WebGL);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.WebGL);
    }

    [MenuItem("Build/Rebuild Windows/Asteroids")]
    public static void BuildAsteroidsWindows(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.Asteroids, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Rebuild WebGL/Frogger")]
    public static void BuildFroggerWebGL(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.Frogger, BuildTargetGroup.WebGL, BuildTarget.WebGL);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.WebGL);
    }

    [MenuItem("Build/Rebuild Windows/Frogger")]
    public static void BuildFroggerWindows(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.Frogger, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Rebuild WebGL/AllThree")]
    public static void BuildAllThreeWebGL(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.AllThree, BuildTargetGroup.WebGL, BuildTarget.WebGL);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.WebGL);
    }

    [MenuItem("Build/Rebuild Windows/AllThree")]
    public static void BuildAllThreeWindows(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.AllThree, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Rebuild WebGL/Berzerk")]
    public static void BuildBerzerkWebGL(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.Berzerk, BuildTargetGroup.WebGL, BuildTarget.WebGL);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.WebGL);
    }

    [MenuItem("Build/Rebuild Windows/Berzerk")]
    public static void BuildBerzerkWindows(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.Berzerk, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Rebuild WebGL/DigDug")]
    public static void BuildDigDugWebGL(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.DigDug, BuildTargetGroup.WebGL, BuildTarget.WebGL);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.WebGL);
    }

    [MenuItem("Build/Rebuild Windows/DigDug")]
    public static void BuildDigDugWindows(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.DigDug, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Rebuild Windows/LittleFighter")]
    public static void BuildLittleFighterWindows(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.LittleFighter, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Rebuild WebGL/LittleFighter")]
    public static void BuildLittleFighterWebGL(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.LittleFighter, BuildTargetGroup.WebGL, BuildTarget.WebGL);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.WebGL);
    }

    [MenuItem("Build/Rebuild Windows/SpaceBase")]
    public static void BuildSpaceBaseWindows(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.SpaceBase, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Rebuild WebGL/SpaceBase")]
    public static void BuildSpaceBaserWebGL(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.SpaceBase, BuildTargetGroup.WebGL, BuildTarget.WebGL);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.WebGL);
    }

    [MenuItem("Build/Rebuild All")]
    public static void BuildAll(){
        BuildWebGL();
        BuildWindows();
    }
    [MenuItem("Build/Rebuild WebGL/All")]
    public static void BuildWebGL(){
        long time = GetCurrentTime();
        BuildAll(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.WebGL);
    }
    [MenuItem("Build/Rebuild Windows/All")]
    public static void BuildWindows(){
        long time = GetCurrentTime();
        BuildAll(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Build/Rebuild WebGL/AllThree2")]
    public static void BuildAllThree2WebGL(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.SecondThree, BuildTargetGroup.WebGL, BuildTarget.WebGL);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.WebGL);
    }

    [MenuItem("Build/Rebuild Windows/AllThree2")]
    public static void BuildAllThree2Windows(){
        FillScenes();
        FillDirectives();

        long time = GetCurrentTime();
        BuildGame(Game.SecondThree, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        PrintTimeFormatted(GetCurrentTime() - time, BuildTarget.StandaloneWindows64);
    }

#endregion

    #endif
}

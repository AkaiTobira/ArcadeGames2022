using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;
using System.IO;


public static partial class CBuild
{



    #if UNITY_EDITOR
    private enum Game{
        Asteroids,
        Frogger,
        DigDug2,
        Three1,
        Berzerk,
        LittleFighter,
        SpaceBase,
        Four1,
        Tunnel,
        AllForOne,
        SixGames,
        DigDug,

        MAX,
    }

    static string _buildFolder = "_Build/";

    class PathConstructor{
        List<string> elements = new List<string>();

        static Dictionary<Game, string> _paths = new Dictionary<Game, string>(){
            {Game.Asteroids, "Assets/Asteroids/Scenes/"},
            {Game.Berzerk, "Assets/Berzerk/Scenes/"},
            {Game.DigDug2, "Assets/DigDug2/Scenes/"},
            {Game.Frogger, "Assets/Frogger/Scenes/"},
            {Game.LittleFighter, "Assets/LittleFighter/Scenes/"},
            {Game.SpaceBase, "Assets/SpaceBase/Scenes/"},
            {Game.Tunnel, "Assets/Tunnel/Scenes/"},
            {Game.DigDug, "Assets/DigDug/Scenes/"}
        };

        public PathConstructor(int introType){
            AddIntroType(introType);
            elements.Add("Assets/_Common/EOutro.unity");
        }

        private void AddIntroType(int type){
            switch(type){
                case 0: elements.Add("Assets/_Common/Intro.unity"); break;
                case 1: elements.Add("Assets/_Common/Intro1.unity"); break;
                case 2: elements.Add("Assets/_Common/Intro2.unity"); break;
                case 3: elements.Add("Assets/_Common/Intro3.unity"); break;
            }
        }

        public PathConstructor AddGameSelect(int type){
            switch(type){
                case 3: elements.Add("Assets/_Common/GameSelect.unity"); break;
                case 4: elements.Add("Assets/_Common/GameSelect2.unity"); break;
                case 6: elements.Add("Assets/_Common/GameSelect4.unity"); break;
                case 7: elements.Add("Assets/_Common/GameSelect3.unity"); break;
            }

            return this;
        }

        public PathConstructor AddAsteroids(){
            elements.Add(_paths[Game.Asteroids] + "Asteroids.unity");
            elements.Add(_paths[Game.Asteroids] + "AsteroidsIntro.unity");
            elements.Add(_paths[Game.Asteroids] + "AsteroidsMain.unity");
            elements.Add(_paths[Game.Asteroids] + "AsteroidsOutro.unity");

            return this;
        }

        public PathConstructor AddBerzerk(){
            elements.Add(_paths[Game.Berzerk] + "Berzerk.unity");
            elements.Add(_paths[Game.Berzerk] + "BerzerkIntro.unity");
            elements.Add(_paths[Game.Berzerk] + "BerzerkMain.unity");
            elements.Add(_paths[Game.Berzerk] + "BerzerkOutro.unity");

            return this;
        }

        public PathConstructor AddDigDug2(){
            elements.Add(_paths[Game.DigDug2] + "DigDug2.unity");
            elements.Add(_paths[Game.DigDug2] + "DigDug2Intro.unity");
            elements.Add(_paths[Game.DigDug2] + "DigDug2Main.unity");
            elements.Add(_paths[Game.DigDug2] + "DigDug2Outro.unity");

            return this;
        }

        public PathConstructor AddDigDug(){
            elements.Add(_paths[Game.DigDug] + "DigDug.unity");
            elements.Add(_paths[Game.DigDug] + "DigDugIntro.unity");
            elements.Add(_paths[Game.DigDug] + "DigDugMain.unity");
            elements.Add(_paths[Game.DigDug] + "DigDugOutro.unity");

            return this;
        }

        public PathConstructor AddFrogger(){
            elements.Add(_paths[Game.Frogger] + "Frogger.unity");
            elements.Add(_paths[Game.Frogger] + "FroggerIntro.unity");
            elements.Add(_paths[Game.Frogger] + "FroggerMain.unity");
            elements.Add(_paths[Game.Frogger] + "FroggerOutro.unity");
            elements.Add(_paths[Game.Frogger] + "FroggerOutro1.unity");

            return this;
        }

        public PathConstructor AddLittleFighter(){
            elements.Add(_paths[Game.LittleFighter] + "LittleFighter.unity");
            elements.Add(_paths[Game.LittleFighter] + "LittleFighterIntro.unity");
            elements.Add(_paths[Game.LittleFighter] + "LittleFighterMain.unity");
            elements.Add(_paths[Game.LittleFighter] + "LittleFighterOutro.unity");

            return this;
        }


        public PathConstructor AddSpaceBase(){
            elements.Add(_paths[Game.SpaceBase] + "SpaceBase.unity");
            elements.Add(_paths[Game.SpaceBase] + "SpaceBaseIntro.unity");
            elements.Add(_paths[Game.SpaceBase] + "SpaceBaseMain.unity");
            elements.Add(_paths[Game.SpaceBase] + "SpaceBaseOutro.unity");

            return this;
        }

        public PathConstructor AddTunnel(){
            elements.Add(_paths[Game.Tunnel] + "Tunnel.unity");
            elements.Add(_paths[Game.Tunnel] + "TunnelIntro.unity");
            elements.Add(_paths[Game.Tunnel] + "TunnelMain.unity");
            elements.Add(_paths[Game.Tunnel] + "TunnelOutro.unity");

            return this;
        }

        public string[] Construct(){
            return elements.ToArray();
        }
    }

    static Dictionary<Game, string[]> _directives = new Dictionary<Game, string[]>();
    static Dictionary<Game, string[]> _scenes = new Dictionary<Game, string[]>();

    static Dictionary<BuildTarget, BuildTargetGroup> _buildConfigs = new Dictionary<BuildTarget, BuildTargetGroup>();

    private static void FillScenes(){
        _scenes[Game.Asteroids] = 
            new PathConstructor(1)
                                    .AddAsteroids()
                                    .Construct();
        _scenes[Game.Berzerk]   =             
            new PathConstructor(1)
                                    .AddBerzerk()
                                    .Construct();
        _scenes[Game.DigDug]    =             
            new PathConstructor(3)
                                    .AddDigDug()
                                    .Construct();
        _scenes[Game.DigDug2]    =             
            new PathConstructor(1)
                                    .AddDigDug2()
                                    .Construct();
        _scenes[Game.Frogger]   = 
            new PathConstructor(1)
                                    .AddFrogger()
                                    .Construct();
        _scenes[Game.Three1]  = 
            new PathConstructor(1)
                                    .AddGameSelect(3)
                                    .AddAsteroids()
                                    .AddDigDug2()
                                    .AddFrogger()
                                    .Construct();
        _scenes[Game.LittleFighter]   = 
            new PathConstructor(1)
                                    .AddLittleFighter()
                                    .Construct();

        _scenes[Game.SpaceBase]   = 
            new PathConstructor(1)
                                    .AddSpaceBase()
                                    .Construct();

        _scenes[Game.Four1] = 
            new PathConstructor(1)
                                    .AddGameSelect(4)
                                    .AddLittleFighter()
                                    .AddSpaceBase()
                                    .AddBerzerk()
                                    .AddTunnel()
                                    .Construct();

        _scenes[Game.Tunnel] = 
            new PathConstructor(1)
                                    .AddTunnel()
                                    .Construct();
        
        _scenes[Game.AllForOne] = 
            new PathConstructor(1)
                .AddGameSelect(7)
                .AddLittleFighter()
                .AddAsteroids()
                .AddBerzerk()
                .AddSpaceBase()
                .AddTunnel()
                .AddFrogger()
                .AddDigDug2()
                .Construct();

        _scenes[Game.SixGames] =
            new PathConstructor(3)
                .AddGameSelect(6)
                .AddLittleFighter()
                .AddAsteroids()
                .AddBerzerk()
                .AddSpaceBase()
                .AddFrogger()
                .AddDigDug2()
                .Construct();
    }

    private static void FillDirectives(){
        _directives[Game.Asteroids]     = new string[] {"ASTEROIDS_GAME"};
        _directives[Game.Berzerk]       = new string[] {"BERZERK_GAME"};
        _directives[Game.DigDug2]       = new string[] {"DIGDUG2_GAME"};
        _directives[Game.Frogger]       = new string[] {"FROGGER_GAME"};
        _directives[Game.Three1]        = new string[] {"THREE_GAME"};
        _directives[Game.Four1]         = new string[] { "T3_GAMES_2" };
        _directives[Game.LittleFighter] = new string[] {"LITTLE_FIGHTER_GAME"};
        _directives[Game.SpaceBase]     = new string[] {"SPACE_BASE_GAME"};
        _directives[Game.Tunnel]        = new string[] {"TUNNEL_GAME"};
        _directives[Game.AllForOne]     = new string[] { "ALL_GAMES" };
        _directives[Game.SixGames]      = new string[] { "SIX_GAMES", "INTRO3", "SKIP_EU_OUTRO" };
        _directives[Game.DigDug]        = new string[] { "DIGDUG_GAME", "INTRO3", "SKIP_EU_OUTRO"};

        _buildConfigs[BuildTarget.StandaloneLinux64] = BuildTargetGroup.Standalone;
        _buildConfigs[BuildTarget.StandaloneWindows] = BuildTargetGroup.Standalone;
        _buildConfigs[BuildTarget.StandaloneWindows64] = BuildTargetGroup.Standalone;
        _buildConfigs[BuildTarget.WebGL] = BuildTargetGroup.WebGL;
        _buildConfigs[BuildTarget.Android] = BuildTargetGroup.Android;
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
        PlayerSettings.SetArchitecture(targetGroup, 2);
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
        string path = _buildFolder + "/" + platform.ToString() + "/" + gameId.ToString();
        switch (platform) {
            case BuildTarget.StandaloneWindows64: path += "/" + gameId.ToString() + ".exe"; break;
            case BuildTarget.StandaloneWindows: path += "/" + gameId.ToString() + ".exe"; break;
            case BuildTarget.StandaloneLinux64: path += "/" + gameId.ToString() + ".x86_64"; break;
            case BuildTarget.Android: path += ".apk"; break;
            default: path += "/"; break;
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


    [MenuItem("Build/Generate Interface")]
    public static void GenerateInterface(){

        #if UNITY_EDITOR

            FillDirectives();

            string destination = Application.dataPath + "/_AutoGenerated/CBuild2.cs";
            if(!File.Exists(destination)) File.WriteAllText(destination, "");

            StreamWriter file = new StreamWriter(destination, false);


            file.WriteLine("//THIS FILE IS AUTO GENERATED");
            file.WriteLine("//AFTER ADD NEW GAME REGENEREATE BY BUILD/GENERATEINTERFACE");
            file.WriteLine("using UnityEditor;\n");
            file.WriteLine("");
            file.WriteLine("public static partial class CBuild{");
            file.WriteLine("#if UNITY_EDITOR");

            List<Game> list = new List<Game>();
            for(int i = 0; i < (int)Game.MAX; i++) {
                list.Add((Game)i);
            }
            
            list.Sort((x,y) => {
                return x.ToString().CompareTo(y.ToString());
            });

            for(int i = 0; i < list.Count; i++) {
                Game g = list[i];
                foreach( KeyValuePair<BuildTarget, BuildTargetGroup> config in _buildConfigs){
                    file.WriteLine(ConstructStaticMetod(g, config.Value, config.Key));
                }
            }

            foreach( KeyValuePair<BuildTarget, BuildTargetGroup> config in _buildConfigs){
                file.WriteLine(ConstructBuildAllStaticMetod(config.Value, config.Key));
            }

            file.WriteLine(ConstuctBuildAllStaticMetodInternal());
            file.WriteLine("#endif\n}\n");

            file.Close();
            AssetDatabase.Refresh();
        #endif
    }

    private static string ConstructStaticMetod(Game game, BuildTargetGroup targetGroup, BuildTarget target){
        string output = ConstructInterfaceEtiquiete(target.ToString(), game.ToString());
        output += ConstructFunctionName(game.ToString() + target.ToString());
        output += ConstructCommonBlock();
        output += "\t\tBuildGame(Game." + game.ToString() +", BuildTargetGroup." + targetGroup.ToString() + ", BuildTarget." + target.ToString() + ");\n";  
        return output + ConstructTimerBlock2(target);
    }

    private static string ConstructBuildAllStaticMetod(BuildTargetGroup targetGroup, BuildTarget target){
        string output = ConstructInterfaceEtiquiete(target.ToString(), "All");
        output += ConstructFunctionName(target.ToString() + "All");
        output += ConstructCommonBlock();
        output += "\t\tBuildAll(BuildTargetGroup." + targetGroup.ToString() + ", BuildTarget." + target.ToString() + ");\n";  
        return output + ConstructTimerBlock2(target);
    }

    private static string ConstuctBuildAllStaticMetodInternal(){
        string output = ConstructInterfaceEtiquiete("All");
        output += ConstructFunctionName("AllPlatforms");
        output += ConstructCommonBlock();
        foreach(KeyValuePair<BuildTarget, BuildTargetGroup> config in _buildConfigs){
            output += "\t\tBuildAll(BuildTargetGroup." + config.Value.ToString() + ", BuildTarget." + config.Key.ToString() + ");\n";  
        }
        return output + ConstructTimerBlock2(BuildTarget.NoTarget);
    }

    private static string ConstructInterfaceEtiquiete(string prefix, string suffix = null){
        string output = "\t[MenuItem(" + '"' + "Build/Rebuild " + prefix;   ;
        if(!string.IsNullOrEmpty(suffix)){
            output += "/" + suffix;
        }
        return output + '"' + ")]\n";
    }

    private static string ConstructCommonBlock(){
        return "\t\tFillScenes();\n\t\tFillDirectives();\n\n" + ConstructTimerBlock1(); 
    }

    private static string ConstructFunctionName(string sName){
        return "\tpublic static void Build" + sName + "(){\n";
    }

    private static string ConstructTimerBlock1(){
        return "\t\tlong time = GetCurrentTime();\n";
    }

    private static string ConstructTimerBlock2(BuildTarget target){
        return "\t\tPrintTimeFormatted(GetCurrentTime() - time, BuildTarget." + target.ToString() + ");\n\t}\n";
    }

#endregion

    #endif
}

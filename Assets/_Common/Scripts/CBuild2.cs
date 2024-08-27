//THIS FILE IS AUTO GENERATED
//AFTER ADD NEW GAME REGENEREATE BY BUILD/GENERATEINTERFACE
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.Build;

#endif

public static partial class CBuild{
#if UNITY_EDITOR
	[MenuItem("Build/Rebuild/AllForOne/StandaloneLinux64")]
	public static void BuildAllForOneStandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.AllForOne, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/AllForOne/StandaloneWindows")]
	public static void BuildAllForOneStandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.AllForOne, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/AllForOne/StandaloneWindows64")]
	public static void BuildAllForOneStandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.AllForOne, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/AllForOne/WebGL")]
	public static void BuildAllForOneWebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.AllForOne, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/AllForOne/Android")]
	public static void BuildAllForOneAndroid(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.AllForOne, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/AllForOne/All")]
	public static void BuildAllForOneAll(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.AllForOne);
		PrintTimeFormatted(GetCurrentTime() - time, "AllForOne");
	}

	[MenuItem("Build/Rebuild/Asteroids/StandaloneLinux64")]
	public static void BuildAsteroidsStandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Asteroids, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/Asteroids/StandaloneWindows")]
	public static void BuildAsteroidsStandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Asteroids, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/Asteroids/StandaloneWindows64")]
	public static void BuildAsteroidsStandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Asteroids, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/Asteroids/WebGL")]
	public static void BuildAsteroidsWebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Asteroids, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/Asteroids/Android")]
	public static void BuildAsteroidsAndroid(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Asteroids, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/Asteroids/All")]
	public static void BuildAsteroidsAll(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.Asteroids);
		PrintTimeFormatted(GetCurrentTime() - time, "Asteroids");
	}

	[MenuItem("Build/Rebuild/Berzerk/StandaloneLinux64")]
	public static void BuildBerzerkStandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Berzerk, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/Berzerk/StandaloneWindows")]
	public static void BuildBerzerkStandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Berzerk, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/Berzerk/StandaloneWindows64")]
	public static void BuildBerzerkStandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Berzerk, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/Berzerk/WebGL")]
	public static void BuildBerzerkWebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Berzerk, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/Berzerk/Android")]
	public static void BuildBerzerkAndroid(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Berzerk, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/Berzerk/All")]
	public static void BuildBerzerkAll(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.Berzerk);
		PrintTimeFormatted(GetCurrentTime() - time, "Berzerk");
	}

	[MenuItem("Build/Rebuild/DigDug/StandaloneLinux64")]
	public static void BuildDigDugStandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.DigDug, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/DigDug/StandaloneWindows")]
	public static void BuildDigDugStandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.DigDug, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/DigDug/StandaloneWindows64")]
	public static void BuildDigDugStandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.DigDug, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/DigDug/WebGL")]
	public static void BuildDigDugWebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.DigDug, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/DigDug/Android")]
	public static void BuildDigDugAndroid(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.DigDug, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/DigDug/All")]
	public static void BuildDigDugAll(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.DigDug);
		PrintTimeFormatted(GetCurrentTime() - time, "DigDug");
	}

	[MenuItem("Build/Rebuild/DigDug2/StandaloneLinux64")]
	public static void BuildDigDug2StandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.DigDug2, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/DigDug2/StandaloneWindows")]
	public static void BuildDigDug2StandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.DigDug2, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/DigDug2/StandaloneWindows64")]
	public static void BuildDigDug2StandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.DigDug2, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/DigDug2/WebGL")]
	public static void BuildDigDug2WebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.DigDug2, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/DigDug2/Android")]
	public static void BuildDigDug2Android(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.DigDug2, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/DigDug2/All")]
	public static void BuildDigDug2All(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.DigDug2);
		PrintTimeFormatted(GetCurrentTime() - time, "DigDug2");
	}

	[MenuItem("Build/Rebuild/Four1/StandaloneLinux64")]
	public static void BuildFour1StandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Four1, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/Four1/StandaloneWindows")]
	public static void BuildFour1StandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Four1, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/Four1/StandaloneWindows64")]
	public static void BuildFour1StandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Four1, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/Four1/WebGL")]
	public static void BuildFour1WebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Four1, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/Four1/Android")]
	public static void BuildFour1Android(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Four1, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/Four1/All")]
	public static void BuildFour1All(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.Four1);
		PrintTimeFormatted(GetCurrentTime() - time, "Four1");
	}

	[MenuItem("Build/Rebuild/Frogger/StandaloneLinux64")]
	public static void BuildFroggerStandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Frogger, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/Frogger/StandaloneWindows")]
	public static void BuildFroggerStandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Frogger, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/Frogger/StandaloneWindows64")]
	public static void BuildFroggerStandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Frogger, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/Frogger/WebGL")]
	public static void BuildFroggerWebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Frogger, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/Frogger/Android")]
	public static void BuildFroggerAndroid(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Frogger, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/Frogger/All")]
	public static void BuildFroggerAll(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.Frogger);
		PrintTimeFormatted(GetCurrentTime() - time, "Frogger");
	}

	[MenuItem("Build/Rebuild/Garden/StandaloneLinux64")]
	public static void BuildGardenStandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Garden, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/Garden/StandaloneWindows")]
	public static void BuildGardenStandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Garden, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/Garden/StandaloneWindows64")]
	public static void BuildGardenStandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Garden, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/Garden/WebGL")]
	public static void BuildGardenWebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Garden, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/Garden/Android")]
	public static void BuildGardenAndroid(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Garden, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/Garden/All")]
	public static void BuildGardenAll(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.Garden);
		PrintTimeFormatted(GetCurrentTime() - time, "Garden");
	}

	[MenuItem("Build/Rebuild/LittleFighter/StandaloneLinux64")]
	public static void BuildLittleFighterStandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.LittleFighter, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/LittleFighter/StandaloneWindows")]
	public static void BuildLittleFighterStandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.LittleFighter, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/LittleFighter/StandaloneWindows64")]
	public static void BuildLittleFighterStandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.LittleFighter, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/LittleFighter/WebGL")]
	public static void BuildLittleFighterWebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.LittleFighter, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/LittleFighter/Android")]
	public static void BuildLittleFighterAndroid(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.LittleFighter, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/LittleFighter/All")]
	public static void BuildLittleFighterAll(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.LittleFighter);
		PrintTimeFormatted(GetCurrentTime() - time, "LittleFighter");
	}

	[MenuItem("Build/Rebuild/NineGames/StandaloneLinux64")]
	public static void BuildNineGamesStandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.NineGames, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/NineGames/StandaloneWindows")]
	public static void BuildNineGamesStandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.NineGames, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/NineGames/StandaloneWindows64")]
	public static void BuildNineGamesStandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.NineGames, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/NineGames/WebGL")]
	public static void BuildNineGamesWebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.NineGames, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/NineGames/Android")]
	public static void BuildNineGamesAndroid(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.NineGames, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/NineGames/All")]
	public static void BuildNineGamesAll(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.NineGames);
		PrintTimeFormatted(GetCurrentTime() - time, "NineGames");
	}

	[MenuItem("Build/Rebuild/SixGames/StandaloneLinux64")]
	public static void BuildSixGamesStandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.SixGames, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/SixGames/StandaloneWindows")]
	public static void BuildSixGamesStandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.SixGames, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/SixGames/StandaloneWindows64")]
	public static void BuildSixGamesStandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.SixGames, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/SixGames/WebGL")]
	public static void BuildSixGamesWebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.SixGames, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/SixGames/Android")]
	public static void BuildSixGamesAndroid(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.SixGames, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/SixGames/All")]
	public static void BuildSixGamesAll(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.SixGames);
		PrintTimeFormatted(GetCurrentTime() - time, "SixGames");
	}

	[MenuItem("Build/Rebuild/SpaceBase/StandaloneLinux64")]
	public static void BuildSpaceBaseStandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.SpaceBase, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/SpaceBase/StandaloneWindows")]
	public static void BuildSpaceBaseStandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.SpaceBase, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/SpaceBase/StandaloneWindows64")]
	public static void BuildSpaceBaseStandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.SpaceBase, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/SpaceBase/WebGL")]
	public static void BuildSpaceBaseWebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.SpaceBase, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/SpaceBase/Android")]
	public static void BuildSpaceBaseAndroid(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.SpaceBase, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/SpaceBase/All")]
	public static void BuildSpaceBaseAll(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.SpaceBase);
		PrintTimeFormatted(GetCurrentTime() - time, "SpaceBase");
	}

	[MenuItem("Build/Rebuild/Three1/StandaloneLinux64")]
	public static void BuildThree1StandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Three1, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/Three1/StandaloneWindows")]
	public static void BuildThree1StandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Three1, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/Three1/StandaloneWindows64")]
	public static void BuildThree1StandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Three1, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/Three1/WebGL")]
	public static void BuildThree1WebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Three1, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/Three1/Android")]
	public static void BuildThree1Android(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Three1, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/Three1/All")]
	public static void BuildThree1All(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.Three1);
		PrintTimeFormatted(GetCurrentTime() - time, "Three1");
	}

	[MenuItem("Build/Rebuild/Tunnel/StandaloneLinux64")]
	public static void BuildTunnelStandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Tunnel, NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/Tunnel/StandaloneWindows")]
	public static void BuildTunnelStandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Tunnel, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/Tunnel/StandaloneWindows64")]
	public static void BuildTunnelStandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Tunnel, NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/Tunnel/WebGL")]
	public static void BuildTunnelWebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Tunnel, NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/Tunnel/Android")]
	public static void BuildTunnelAndroid(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildGame(Game.Tunnel, NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

	[MenuItem("Build/Rebuild/Tunnel/All")]
	public static void BuildTunnelAll(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildForAllPlayfroms(Game.Tunnel);
		PrintTimeFormatted(GetCurrentTime() - time, "Tunnel");
	}

	[MenuItem("Build/Rebuild/All/All")]
	public static void BuildAllPlatforms(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildAll(NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		BuildAll(NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		BuildAll(NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		BuildAll(NamedBuildTarget.WebGL, BuildTarget.WebGL);
		BuildAll(NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "NoTarget");
	}

	[MenuItem("Build/Rebuild/All/StandaloneLinux64")]
	public static void BuildAllPlatformsStandaloneLinux64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildAll(NamedBuildTarget.Standalone, BuildTarget.StandaloneLinux64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneLinux64");
	}

	[MenuItem("Build/Rebuild/All/StandaloneWindows")]
	public static void BuildAllPlatformsStandaloneWindows(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildAll(NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows");
	}

	[MenuItem("Build/Rebuild/All/StandaloneWindows64")]
	public static void BuildAllPlatformsStandaloneWindows64(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildAll(NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64);
		PrintTimeFormatted(GetCurrentTime() - time, "StandaloneWindows64");
	}

	[MenuItem("Build/Rebuild/All/WebGL")]
	public static void BuildAllPlatformsWebGL(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildAll(NamedBuildTarget.WebGL, BuildTarget.WebGL);
		PrintTimeFormatted(GetCurrentTime() - time, "WebGL");
	}

	[MenuItem("Build/Rebuild/All/Android")]
	public static void BuildAllPlatformsAndroid(){
		FillScenes();
		FillDirectives();

		long time = GetCurrentTime();
		BuildAll(NamedBuildTarget.Android, BuildTarget.Android);
		PrintTimeFormatted(GetCurrentTime() - time, "Android");
	}

#endif
}


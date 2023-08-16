using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
public enum SupportedLanguages{
    UK,
    DE,

    MAX_COUNT,
}

public static class AutoTranslator
{
    static  Dictionary<SupportedLanguages, Dictionary<string, string>> _translation = 
        new Dictionary<SupportedLanguages, Dictionary<string, string>>();

    static Dictionary<SupportedLanguages, string> _folders = 
    new Dictionary<SupportedLanguages, string>{
        {SupportedLanguages.UK, "Default"},
        {SupportedLanguages.DE, "De"}, 
    };

    static Dictionary<GameType, string > _gameFolders = new Dictionary<GameType, string>{
        {GameType.Asteroids, "Asteroids"},
        {GameType.NotLoaded, "Common"},
        {GameType.Berzerk,   "Berzerk"},
        {GameType.DigDug2,   "DigDug2"},
        {GameType.Frogger,   "Frogger"},
        {GameType.LittleFighter, "LittleFighter"},
        {GameType.SpaceBase, "SpaceBase"},
        {GameType.Tunnel,    "Tunnel"}
    };



    public static SupportedLanguages Language;

    public static Sprite[] LoadImage(GameType game, int FolderID){
        Sprite[] sprite = Resources.LoadAll<Sprite>("Translation/" + AutoTranslator.GetFolder(false, game) + "/" + FolderID.ToString());

        if(sprite.Length == 0){
            sprite = Resources.LoadAll<Sprite>("Translation/" + AutoTranslator.GetFolder(true, game) + "/" + FolderID.ToString());
        }

        Resources.UnloadUnusedAssets();

        return sprite;
    }


    private static string GetFolder(bool Default, GameType type){
        if(Default){
            return _gameFolders[type] + "/" + _folders[SupportedLanguages.UK];
        }

        return _gameFolders[type] + "/" + _folders[Language];
    }

    #if UNITY_EDITOR
        private static HashSet<string> _notFoundTranslations = new HashSet<string>();
    #endif

    public static string Translate(string untranslated, string value = ""){
        untranslated = string.Concat(untranslated.Where(c => !char.IsWhiteSpace(c))).ToUpper();
        if(string.IsNullOrEmpty(untranslated)) return "";
        LoadTranslation();

        if(!_translation[Language].ContainsKey(untranslated)){
            string keys = "";
            for(int i = 0; i < _translation[Language].Keys.Count; i++) {
                keys += _translation[Language].Keys.ToArray()[i] + " : ";
            }
            SaveFile(untranslated);
            return untranslated;
        }

        return _translation[Language][untranslated].Replace("@", value).Replace("\n", "<br>");
    }



    private static void SaveFile(string untranslated)
    {
        #if UNITY_EDITOR

            string tranlationsFile = Application.dataPath + "/Resources/Translation.txt";
            string destination = Application.dataPath + "/_AutoGenerated/toTranslate.txt";
            
            string alreadyLocalized = "";

            if(!File.Exists(tranlationsFile)){
                Debug.LogError("Not exist file with translations");
            }else{
                StreamReader localized = new StreamReader(tranlationsFile, false);
                alreadyLocalized = localized.ReadToEnd().ToUpper();
            }

            if(File.Exists(destination)){
                StreamReader alreadysaved = new StreamReader(destination, false);
                string saved = alreadysaved.ReadToEnd();
                string[] splited = saved.Split('\n');

                
                foreach(string s in splited){
                    if(string.IsNullOrEmpty(s)) continue;
                    string newStrigna = String.Concat(s.Where(c => !Char.IsWhiteSpace(c)));

//                    if(alreadyLocalized.Contains(newStrigna)) continue;
                    _notFoundTranslations.Add(newStrigna);
                }

                if(saved.Contains(untranslated)){
                    Debug.LogWarning("Still untraslated " + untranslated);
                    alreadysaved.Close();
                    return;
                }

                alreadysaved.Close();
            }
            
            
            StreamWriter file = new StreamWriter(destination, false);

            if(!_notFoundTranslations.Contains(untranslated)){
                _notFoundTranslations.Add(untranslated);
                Debug.LogWarning("Didn't found localization for key : " + untranslated + " \n Saved to " + destination);
            }

            foreach(string toSave in _notFoundTranslations){

                string cleanedString = String.Concat(toSave.Where(c => !Char.IsWhiteSpace(c)));
                file.WriteLine(cleanedString);
            }

            file.Close();
        #endif
    }



    private static void LoadTranslation(){

        TextAsset translations = TextAssets.Translations;

        if(_translation.ContainsKey(Language)) return;

        _translation[Language] = new Dictionary<string, string>();

//        Debug.Log(translations == null);

        if(translations == null){
            Debug.LogWarning("There is no translations file loaded");
            return;
        }

        string alpga = translations.text.ToString();
        
        string[] formule = alpga.Split(new string[] {"::"}, StringSplitOptions.None);
        
        for(int i = 1; i < formule.Length; i++) {
            string[] words = formule[i].Split('\t');
            if((int)Language + 1 >= words.Length) continue;

            string active = string.Concat(words[0].Where(c => !char.IsWhiteSpace(c))).ToUpper();
            _translation[Language][active] = words[(int)Language + 1];
            };
        }
}
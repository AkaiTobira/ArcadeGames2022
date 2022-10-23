using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public enum SupportedLanguages{
    UK,
    DE,

    MAX_COUNT,
}

public static class AutoTranslator
{
    static  Dictionary<SupportedLanguages, Dictionary<string, string>> _translation = 
        new Dictionary<SupportedLanguages, Dictionary<string, string>>();

    public static SupportedLanguages Language;

    public static string Translate(string untranslated, string value = ""){
        untranslated = string.Concat(untranslated.Where(c => !char.IsWhiteSpace(c))).ToUpper();
        if(string.IsNullOrEmpty(untranslated)) return "";
        LoadTranslation();

        if(!_translation[Language].ContainsKey(untranslated)){
            string keys = "";
            for(int i = 0; i < _translation[Language].Keys.Count; i++) {
                keys += _translation[Language].Keys.ToArray()[i] + " : ";
            }
            Debug.LogWarning("Didn't found localization for key :" + untranslated + " existing keys :" + keys);
            return "";
        }

        return _translation[Language][untranslated].Replace("@", value).Replace("\n", "<br>");
    }


    private static void LoadTranslation(){

        TextAsset translations = TextAssets.Translations;

        if(_translation.ContainsKey(Language)) return;

        _translation[Language] = new Dictionary<string, string>();

        Debug.Log(translations == null);

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
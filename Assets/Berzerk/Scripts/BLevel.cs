using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BLevel : MonoBehaviour
{
    [SerializeField] GameObject[] horizontal;
    [SerializeField] GameObject[] vertical;

    const int HORIZONTAL = 8;
    const int VERTICAL = 7; 

    private void Awake() {
        SelectRandomLevel();
    }

    public void SelectRandomLevel(){
        SetShape(LevelShapes.GetRandomLevel());
    }


    private void SetHorizontalLine(int index, string line){
        int begin = index * HORIZONTAL;
        for(int i = 0; i < line.Length; i++) {
            if(i%2 == 1) continue;
            horizontal[begin++].SetActive(line[i] == '-');
        }
    }

    private void SetVerticalLine(int index, string line){
        int begin = index * VERTICAL;
        for(int i = 0; i < line.Length; i++) {
            if(i%2 == 0) continue;
            vertical[begin++].SetActive(line[i] == '|');
        }
    }

    private void SetShape(string[] shape){
        for(int i = 0; i < 4; i++) {
            SetVerticalLine(i, shape[(i*2)]);
            SetHorizontalLine(i, shape[(i*2)+1]);            
        }

        SetVerticalLine(4, shape[shape.Length-1]);
    }
}

public static class LevelShapes{
    private static string[][] _shapes = new string[][]{
        new string[]{
            "               ", //V ' ' or '+' -ignore, '|' or '-' -enabled,
            " +-+-+   +-+-+-", //H
            "               ", //V
            " +-+-+   +-+-+-", //H
            "               ", //V
            " +-+-+   +-+-+-", //H
            "               ", //V
            " +-+-+   +-+-+-", //H
            "               ", //V
        },
        new string[]{
            "     |   |     ", //V ' ' or '+' -ignore, '|' or '-' -enabled,
            "               ", //H
            "               ", //V
            "-+-+-+-+-+-   -", //H
            "               ", //V
            "-+-+ +        -", //H
            "     |   |     ", //V
            "     +   +   +-", //H
            "     |   |     ", //V
        },/*
        new string[]{
            " | | |   | | | ", //V ' ' or '+' -ignore, '|' or '-' -enabled,
            "-+-+-+-+-+-+-+-", //H
            " | | | | | | | ", //V
            "-+-+-+-+-+-+-+-", //H
            " | | | | | | | ", //V
            "-+-+-+-+-+-+-+-", //H
            " | | | | | | | ", //V
            "-+-+-+-+-+-+-+-", //H
            " | | |   | | | ", //V
        },
        */
    };

    public static string[] GetRandomLevel(){
        return _shapes[Random.Range(0, _shapes.Length)];
    }
}

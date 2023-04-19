using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class StageGenerator {

    private ShyGuySays _module;

    private readonly Color[] _colours = new Color[] {
        Color.red,
        Color.white,
        Color.green,
        Color.blue
    };
    private readonly char[] _letters = new char[] { 'A', 'B', 'L', 'R' };

    private readonly char[,,] _flagCharts = new char[2, 4, 4] {
        {
            { 'R', 'L', 'X', 'R'},
            { 'X', 'X', 'L', 'L'},
            { 'L', 'R', 'R', 'X'},
            { 'L', 'X', 'X', 'R'}
        },
        {
            { 'R', 'L', 'R', 'X'},
            { 'X', 'X', 'L', 'R'},
            { 'L', 'R', 'X', 'L'},
            { 'R', 'L', 'R', 'X'}
        }
    };

    private readonly string[] _flagNames = new string[] { "left", "right" };
    private readonly string[] _colourNames = new string[] { "red", "white", "green", "blue" };

    public StageGenerator(ShyGuySays module) {
        _module = module;
    }

    public FlagRaise[] GenerateStage(int stageNumber, out string correctResponse) {
        var displayFlags = new FlagRaise[7];
        string logMessage;
        correctResponse = string.Empty;

        _module.Log("==================== Stage " + stageNumber + " ====================");
        for (int i = 0; i < 7; i++) {
            int flagPosition = Rnd.Range(0, 2);
            int colourIndex = Rnd.Range(0, 4);
            int letterIndex = Rnd.Range(0, 4);

            char newResponse = _flagCharts[flagPosition, letterIndex, colourIndex];

            // Prevent the case where the player needs to skip all seven flags.
            if (i == 6 && correctResponse.Replace("X", string.Empty).Length == 0) {
                while (newResponse == 'X') {
                    letterIndex += 1;
                    newResponse = _flagCharts[flagPosition, letterIndex, colourIndex];
                }
            }

            displayFlags[i] = new FlagRaise(flagPosition, _colours[colourIndex], _letters[letterIndex]);
            correctResponse += newResponse;

            logMessage = "The ";
            switch (i + 1) {
                case 1: logMessage += "1st "; break;
                case 2: logMessage += "2nd "; break;
                case 3: logMessage += "3rd "; break;
                default: logMessage += (i + 1) + "th "; break;
            }
            logMessage += "flag is " + _flagNames[flagPosition] + " " + _letters[letterIndex] + ", in " + _colourNames[colourIndex] + ". ";
            switch (correctResponse[i]) {
                case 'R': logMessage += "Respond with the right flag."; break;
                case 'L': logMessage += "Respond with the left flag."; break;
                case 'X': logMessage += "Skip this flag."; break;
            }
            _module.Log(logMessage);
        }

        correctResponse = correctResponse.Replace("X", string.Empty);
        return displayFlags;
    }

    public FlagRaise GetRandomFakeFlag(int position) {
        int colourIndex = Rnd.Range(0, 4);
        int letterIndex = Rnd.Range(0, 4);

        return new FlagRaise(position, _colours[colourIndex], _letters[letterIndex], true);
    }

}

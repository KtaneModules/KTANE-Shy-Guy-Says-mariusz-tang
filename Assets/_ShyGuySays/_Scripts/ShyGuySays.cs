using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class ShyGuySays : MonoBehaviour {

    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;

    private static int _moduleIdCounter = 0;
    private int _moduleId;

    [SerializeField] private FlagButton[] _buttonFlags;
    [SerializeField] private ShyGuyButton _centreButton;
    [SerializeField] private FlagController _display;

    private readonly FlagRaise[] _strikeRaises = new FlagRaise[] {
        new FlagRaise(0, Color.red, 'X'),
        new FlagRaise(1, Color.red, 'X')
    };
    private readonly string[] _positionNames = new string[] { "Left", "Right" };

    private StageGenerator _stageGenerator;

    private int _stageNumber = 1;
    private bool _activeStage = false;
    private string _expectedInput;
    private string _actualInput;

    private void Start() {
        _moduleId = _moduleIdCounter++;
        AssignInputHandlers();
        _stageGenerator = new StageGenerator(this);
    }

    private void AssignInputHandlers() {
        foreach (FlagButton button in _buttonFlags) {
            button.GetComponent<KMSelectable>().OnInteractEnded += delegate () { FlagRelease(button.Position); };
            button.GetComponent<KMSelectable>().OnInteract += delegate () { FlagPress(button.Position); return false; };
        }
        _centreButton.GetComponent<KMSelectable>().OnInteract += delegate () { CentrePress(); return false; };
        _centreButton.GetComponent<KMSelectable>().OnInteractEnded += delegate () { CentreRelease(); };
    }

    private void FlagPress(int position) {
        if (!_activeStage) {
            return;
        }
        if (_expectedInput[_actualInput.Length] == _positionNames[position][0]) {
            _actualInput += _positionNames[position][0];
        }
        else {
            string strikeMessage = "Incorrectly responded with the " + _positionNames[position].ToLower() + " flag after " + _actualInput.Length + " correct response";
            if (_actualInput.Length != 1) {
                strikeMessage += "s";
            }
            strikeMessage += ".";
            Strike(strikeMessage);
            _activeStage = false;
        }

        if (_actualInput == _expectedInput) {
            Log("Responded with the correct flags.");
            _display.StopQueue();
            _stageNumber += 1;
            _activeStage = false;
            if (_stageNumber == 4) {
                Module.HandlePass();
            }
        }
    }

    private void FlagRelease(int position) {

    }

    private void CentrePress() {

    }

    private void CentreRelease() {
        if (_activeStage) {
            return;
        }
        FlagAction[] actions = GenerateActions(_stageGenerator.GenerateStage(_stageNumber, out _expectedInput));
        _display.EnqueueLoop(actions, 4f / (_stageNumber * 1 + 1));
        _actualInput = string.Empty;
        _activeStage = true;
    }



    public FlagAction[] GenerateActions(FlagRaise[] raises) {
        var actions = new FlagAction[7];

        for (int i = 0; i < 7; i++) {
            float speed = _stageNumber * 0.5f + 0.5f;
            if (Rnd.Range(0, 7) == 0) {
                actions[i] = new FlagAction(
                    speed,
                    new FlagRaise[] {
                        raises[i],
                        _stageGenerator.GetRandomFakeFlag(1 - raises[i].Position)
                    }
                );
            }
            else {
                actions[i] = new FlagAction(speed, raises[i]);
            }
        }

        return actions;
    }

    public void Log(string message) {
        Debug.LogFormat("[Shy Guy Says #{0}] {1}", _moduleId, message);
    }

    public void Strike(string message) {
        Module.HandleStrike();
        Log("✕ " + message);
        _display.Enqueue(new FlagAction(1, _strikeRaises), clearExistingActions: true);
    }
}

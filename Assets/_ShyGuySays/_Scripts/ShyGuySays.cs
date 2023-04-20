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
    private bool TwitchPlaysActive;

    [SerializeField] private FlagButton[] _buttonFlags;
    [SerializeField] private ShyGuyButton _centreButton;
    [SerializeField] private FlagController _display;
    [SerializeField] private KMSelectable _colourblindButton;

    private readonly FlagRaise[] _strikeRaises = new FlagRaise[] {
        new FlagRaise(0, Color.red, 'X'),
        new FlagRaise(1, Color.red, 'X')
    };
    private readonly FlagRaise[] _okRaises = new FlagRaise[] {
        new FlagRaise(0, Color.green, 'O'),
        new FlagRaise(1, Color.green, 'K')
    };
    private readonly string[] _positionNames = new string[] { "Left", "Right" };

    private StageGenerator _stageGenerator;
    private Coroutine _timer;

    private int _stageNumber = 1;
    private bool _activeStage = false;
    private string _expectedInput;
    private string _actualInput;

    private void Start() {
        _moduleId = _moduleIdCounter++;
        Log("Press Shy Guy's mask to begin!");
        AssignInputHandlers();
        _stageGenerator = new StageGenerator(this);
    }

    private void AssignInputHandlers() {
        foreach (FlagButton button in _buttonFlags) {
            button.GetComponent<KMSelectable>().OnInteract += delegate () { FlagPress(button.Position); return false; };
        }
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
            string strikeMessage = "Incorrectly responded with the " + _positionNames[position].ToLower() + " flag after " + _actualInput.Length + " correct response!";
            if (_actualInput.Length != 1) {
                strikeMessage += "s";
            }
            strikeMessage += ".";
            Strike(strikeMessage);
            Log("Press the Shy Guy to try again.");
            StopCoroutine(_timer);
            _activeStage = false;
        }

        if (_actualInput == _expectedInput) {
            Log("Responded with the correct flags!");
            _display.StopQueue();
            StopCoroutine(_timer);
            _stageNumber += 1;
            _activeStage = false;
            _display.Enqueue(new FlagAction(1, _okRaises), clearExistingActions: true);
            if (_stageNumber == 4) {
                Audio.PlaySoundAtTransform("Solve", transform);
                Log("===================== Solved =====================");
                Log("You win!");
                Module.HandlePass();
            }
        }
    }

    private void CentreRelease() {
        if (_stageNumber == 4) {
            Audio.PlaySoundAtTransform("Flag " + Rnd.Range(1, 4), transform);
            return;
        }
        if (_activeStage) {
            return;
        }

        if (_stageNumber == 1) {
            Audio.PlaySoundAtTransform("Startup", transform);
        }

        FlagAction[] actions = GenerateActions(_stageGenerator.GenerateStage(_stageNumber, out _expectedInput));
        _display.EnqueueLoop(actions, 4f / (_stageNumber * 1 + 1));

        int time = 50 + _stageNumber * 10;
        if (TwitchPlaysActive) {
            time += 25;
        }
        _timer = StartCoroutine(StartTimer(time));
        Log("You have " + time + " seconds.");
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

    private IEnumerator StartTimer(float timerSeconds) {
        float elapsedTime = 0;
        yield return null;

        while (elapsedTime < timerSeconds) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Strike("Ran out of time!");
        Log("Press the Shy Guy to try again.");
        _activeStage = false;
    }

    public void Log(string message) {
        Debug.LogFormat("[Shy Guy Says #{0}] {1}", _moduleId, message);
    }

    public void Strike(string message) {
        Module.HandleStrike();
        Log("✕ " + message);
        _display.Enqueue(new FlagAction(1, _strikeRaises), clearExistingActions: true);
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use '!{0} <start/shyguy>' to press the Shy Guy button | '!{0} <colorblind/cb>' | "
        + "'!{0} <L/R>' to press flags. Optionally, chain presses without spaces, eg '!{0} LRLR' | On TP, the timer is extended by 25 seconds.";
#pragma warning restore 414

    private string _tpButtonNames = "LR";

    private IEnumerator ProcessTwitchCommand(string command) {
        command = command.Trim().ToUpper();

        if (command == string.Empty) {
            yield return "sendtochaterror That's an empty command...";
        }
        if (command.Length > 11) {
            yield return "sendtochaterror Your command is too long!";
        }
        if (command.Contains(" ")) {
            yield return "sendtochaterror There should be no spaces in your command.";
        }

        yield return null;

        if (command == "START" || command == "SHYGUY") {
            _centreButton.GetComponent<KMSelectable>().OnInteract();
            yield return new WaitForSeconds(0.2f);
            _centreButton.GetComponent<KMSelectable>().OnInteractEnded();
        }
        else if (command == "COLORBLIND" || command == "CB" || command == "COLOURBLIND") {
            _colourblindButton.OnInteract();
        }
        else if (command.Replace("R", string.Empty).Replace("L", string.Empty) == string.Empty) {
            foreach (char letter in command) {
                yield return "multiple strikes"; // Allow TP to release the button before stopping the routine.
                _buttonFlags[_tpButtonNames.IndexOf(letter)].GetComponent<KMSelectable>().OnInteract();
                yield return new WaitForSeconds(0.1f);
                _buttonFlags[_tpButtonNames.IndexOf(letter)].GetComponent<KMSelectable>().OnInteractEnded();
                yield return "end multiple strikes";
                yield return new WaitForSeconds(0.1f);
            }
        }
        else {
            yield return "sendtochaterror Invalid command.";
            yield break;
        }
    }

    private IEnumerator TwitchHandleForcedSolve() {
        yield return null;

        while (_stageNumber < 4) {
            if (!_activeStage) {
                _centreButton.GetComponent<KMSelectable>().OnInteract();
                yield return new WaitForSeconds(0.2f);
                _centreButton.GetComponent<KMSelectable>().OnInteractEnded();
            }

            while (_activeStage) {
                int position = _tpButtonNames.IndexOf(_expectedInput[_actualInput.Length]);
                _buttonFlags[position].GetComponent<KMSelectable>().OnInteract();
                yield return new WaitForSeconds(0.1f);
                _buttonFlags[position].GetComponent<KMSelectable>().OnInteractEnded();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}

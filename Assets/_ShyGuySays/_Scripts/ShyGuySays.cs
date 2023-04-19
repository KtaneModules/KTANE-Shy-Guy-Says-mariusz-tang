using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShyGuySays : MonoBehaviour {

    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;

    private static int _moduleIdCounter = 0;
    private int _moduleId;

    [SerializeField] private MainFlag[] _displayFlags;
    [SerializeField] private FlagButton[] _buttonFlags;
    [SerializeField] private ShyGuyButton _centreButton;
    [SerializeField] private FlagController _display;

    private readonly FlagRaise[] _strikeRaises = new FlagRaise[] {
        new FlagRaise(0, Color.red, 'X'),
        new FlagRaise(1, Color.red, 'X')
    };

    private StageGenerator _stageGenerator;

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
        string output;
        FlagAction[] actions = _stageGenerator.GenerateStage(2, out output).Select(raise => new FlagAction(2, raise)).ToArray();
        _display.EnqueueLoop(actions, 2);
    }

    private void FlagRelease(int position) {

    }

    private void CentrePress() {
        Strike("Bruh");
    }

    private void CentreRelease() {

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

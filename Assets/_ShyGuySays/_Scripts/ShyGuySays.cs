using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShyGuySays : MonoBehaviour {

    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;

    [SerializeField] private MainFlag[] _displayFlags;
    [SerializeField] private FlagButton[] _buttonFlags;
    [SerializeField] private ShyGuyButton _centreButton;
    [SerializeField] private FlagController _display;

    private void Start() {
        AssignInputHandlers();
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
        _display.Enqueue(new FlagAction(1, new FlagRaise(position, Color.red, 'X')));
    }

    private void FlagRelease(int position) {

    }

    private void CentrePress() {
        var actions = new FlagRaise[] {
            new FlagRaise(0, Color.green, 'A'),
            new FlagRaise(1, Color.blue, 'B')
        };

        _display.Enqueue(new FlagAction(2, actions), clearExistingActions: true);
    }

    private void CentreRelease() {

    }
}

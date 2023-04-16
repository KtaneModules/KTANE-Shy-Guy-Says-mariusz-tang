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

    // Use this for initialization
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
        _displayFlags[position].Flip(Color.red, 'X');
    }

    private void FlagRelease(int position) {
        _displayFlags[position].Unflip();
    }

    private void CentrePress() {
        _displayFlags[0].Flip(Color.green, 'A');
        _displayFlags[1].Flip(Color.blue, 'B');
    }

    private void CentreRelease() {
        _displayFlags[0].Unflip();
        _displayFlags[1].Unflip();
    }

    // Update is called once per frame
    private void Update() {

    }
}

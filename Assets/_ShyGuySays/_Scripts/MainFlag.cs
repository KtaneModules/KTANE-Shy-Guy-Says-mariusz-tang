using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainFlag : MonoBehaviour {

    private const float BASE_SPEED = 4;
    private readonly Vector3 _cbOffPosition = new Vector3(18.9f, -39, 0.1f);
    private readonly Vector3 _cbOnPosition = new Vector3(14, -36.2f, 0.1f);

    [SerializeField] private Animator _animator;
    [SerializeField] private TextMesh _letter;
    [SerializeField] private TextMesh _cbText;
    [SerializeField] private MeshRenderer _displaySide;

    public bool Ready { get; private set; }
    public bool ColourblindModeActive {
        get { return _colourblindModeActive; }
        set {
            if (value) {
                _letter.transform.localPosition = _cbOnPosition;
                SetCbText();
            }
            else {
                _letter.transform.localPosition = _cbOffPosition;
                _cbText.color = _displaySide.materials[1].color;
            }
            _colourblindModeActive = value;
        }
    }

    private bool _colourblindModeActive = false;

    private void Start() {
        _animator.speed = BASE_SPEED;
        Ready = true;
    }

    public void Flip(Color colour, char letter) {
        Ready = false;
        _displaySide.materials[1].color = colour;
        _letter.color = colour == Color.white ? Color.white * 0.5f : Color.white;
        _letter.text = letter.ToString();
        if (ColourblindModeActive) {
            SetCbText();
        }
        else {
            _cbText.color = colour;
        }

        _animator.SetBool("Show", true);
    }

    public void Flip(Color colour, char letter, float speed) {
        _animator.speed = speed * BASE_SPEED;
        Flip(colour, letter);
    }

    public void Unflip() {
        _animator.SetBool("Show", false);
        _animator.speed = BASE_SPEED;
    }

    private void SetCbText() {
        Color colour = _displaySide.materials[1].color;
        _cbText.color = colour == Color.white ? Color.white * 0.5f : Color.white;

        if (colour == Color.white) {
            _cbText.text = "White";
        }
        else if (colour == Color.green) {
            _cbText.text = "Green";
        }
        else if (colour == Color.blue) {
            _cbText.text = "Blue";
        }
        else if (colour == Color.red) {
            _cbText.text = "Red";
        }
        else {
            throw new ArgumentException("What the fuck colour is this.");
        }
    }

    public void SetReady() {
        Ready = true;
    }
}

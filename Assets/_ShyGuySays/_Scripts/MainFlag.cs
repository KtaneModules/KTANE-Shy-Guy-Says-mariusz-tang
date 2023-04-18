using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainFlag : MonoBehaviour {

    private const float BASE_SPEED = 4;

    [SerializeField] private Animator _animator;
    [SerializeField] private TextMesh _text;
    [SerializeField] private MeshRenderer _displaySide;

    public bool Ready { get; private set; }

    private void Start() {
        _animator.speed = BASE_SPEED;
        Ready = true;
    }

    public void Flip(Color colour, char letter) {
        Ready = false;
        _text.color = colour == Color.white ? Color.white * 0.5f : Color.white;
        _text.text = letter.ToString();
        _displaySide.materials[1].color = colour;

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

    public void SetReady() {
        Ready = true;
    }
}

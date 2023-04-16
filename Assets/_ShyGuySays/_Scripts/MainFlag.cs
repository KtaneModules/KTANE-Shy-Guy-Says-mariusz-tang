using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainFlag : MonoBehaviour {

    [SerializeField] private Animator _animator;
    [SerializeField] private TextMesh _text;
    [SerializeField] private MeshRenderer _displaySide;

    private void Start() {
        _animator.speed = 3;
    }

    public void Flip(Color colour, char letter) {
        _text.text = letter.ToString();
        _displaySide.materials[1].color = colour;
        _animator.SetBool("Show", true);
    }

    public void Unflip() {
        _animator.SetBool("Show", false);
    }
}

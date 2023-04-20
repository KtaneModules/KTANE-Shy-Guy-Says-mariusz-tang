using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShyGuyButton : MonoBehaviour {

    [SerializeField] private Animator _animator;

    void Awake() {
        GetComponent<KMSelectable>().OnInteract += delegate () {
            _animator.SetBool("Press", true);
            GetComponent<KMSelectable>().AddInteractionPunch();
            return false;
        };
        GetComponent<KMSelectable>().OnInteractEnded += delegate () { _animator.SetBool("Press", false); };
    }
}

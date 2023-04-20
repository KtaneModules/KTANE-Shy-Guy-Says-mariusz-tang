using UnityEngine;

public class FlagButton : MonoBehaviour {

    [SerializeField] private Animator _animator;
    [SerializeField] private int _position;

    public int Position { get { return _position; } }

    void Awake() {
        GetComponent<KMSelectable>().OnInteract += delegate () { _animator.SetBool("Press", true); return false; };
        GetComponent<KMSelectable>().OnInteractEnded += delegate () { _animator.SetBool("Press", false); };
    }
}

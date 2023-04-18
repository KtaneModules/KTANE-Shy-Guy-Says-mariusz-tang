using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour {

    [SerializeField] private MainFlag[] _flags;

    private ShyGuySays _module;
    private Coroutine _executeQueue;

    private Queue<FlagAction> _queue = new Queue<FlagAction>();

    private void Start() {
        _module = GetComponentInParent<ShyGuySays>();

        if (_flags.Length != 2) {
            throw new RankException("Is you man a bit stupid?");
        }
    }

    private void Update() {
        if (_queue.Count > 0 && _executeQueue == null) {
            _executeQueue = _module.StartCoroutine(ExecuteQueue());
        }
    }

    public void Enqueue(FlagAction action, bool clearExistingActions = false) {
        if (clearExistingActions) {
            _queue.Clear();
        }

        _queue.Enqueue(action);
    }

    private IEnumerator ExecuteQueue() {
        while (_queue.Count > 0) {
            FlagAction nextAction = _queue.Dequeue();

            foreach (FlagRaise raise in nextAction.Raises) {
                _flags[raise.Position].Flip(raise.Colour, raise.Letter, nextAction.Speed);
            }

            yield return new WaitForSeconds(1 / nextAction.Speed);

            foreach (FlagRaise raise in nextAction.Raises) {
                _flags[raise.Position].Unflip();
            }

            while (!(_flags[0].Ready && _flags[1].Ready)) {
                yield return null;
            }
        }

        _executeQueue = null;
    }
}

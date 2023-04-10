using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerListener : MonoBehaviour
{
    [SerializeField] LayerMask mask;

    public UnityEvent OnEnter;
    public UnityEvent OnStay;
    public UnityEvent OnExit;

    [SerializeField] bool onlyFireEnteredOnce = true;
    [SerializeField] bool onlyFireExitOnce = true;

    bool onEnterFired = false;
    bool onExitFired = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.MatchesLayerMask(mask) && !onEnterFired)
        {
            onEnterFired= true;
            OnEnter?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.MatchesLayerMask(mask) && !onExitFired)
        {
            onExitFired = true;
            OnExit?.Invoke();
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.MatchesLayerMask(mask))
            OnStay?.Invoke();
    }
}

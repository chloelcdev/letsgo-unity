using System.Collections.Generic;
using UnityEngine;

namespace SapphireStateMachine
{
    public class StateMachine : MonoBehaviour
    {
        public bool debugEnabled;
        public string stateDebugReadout = "";
        public State currentState;

        private void Update()
        {
            currentState?.OnUpdate();
            currentState?.TryTick();
        }

        private void FixedUpdate()
        {
            currentState?.OnFixedUpdate();
        }

        public void SetState(State state)
        {
            if (state == null)
            {
                Debug.LogError("Attempted to set null state in StateMachine");
                return;
            }

            currentState?.OnStateExit();

            if (debugEnabled) Debug.Log($"Left state {currentState?.Name}");

            currentState = state;

            stateDebugReadout = state?.Name ?? "";

            currentState?.OnStateEnter();

            if (debugEnabled) Debug.Log($"Entered state {currentState?.Name}");
        }
    }
}
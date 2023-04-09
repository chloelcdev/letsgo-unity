using System.Collections.Generic;
using UnityEngine;

namespace SapphireStateMachine
{
    public class StateMachine : MonoBehaviour
    {
        public bool debugEnabled;
        public List<State> debugStates = new List<State>();
        public State currentState;

        private void Update()
        {
            currentState.OnUpdate();
            currentState.TryTick();

            if (debugEnabled)
            {
                Debug.Log($"Current State: {currentState.Name}");

                foreach (var state in debugStates)
                {
                    if (state == currentState)
                        Debug.Log($"- {state.Name} [Active]");
                    else
                        Debug.Log($"- {state.Name}");
                }
            }
        }

        private void FixedUpdate()
        {
            currentState.OnFixedUpdate();
        }

        public void SetState(State state)
        {
            if (state == null)
            {
                Debug.LogError("Attempted to set null state in StateMachine");
                return;
            }

            currentState?.OnStateExit();

            currentState = state;

            currentState?.OnStateEnter();

            if (debugEnabled)
                Debug.Log($"Changed state to {currentState.Name}");
        }
    }
}
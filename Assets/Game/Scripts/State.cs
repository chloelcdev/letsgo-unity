using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SapphireStateMachine
{
    public abstract class State
    {
        protected StateMachine stateMachine;
        public float lastTickTime { get; private set; }

        public abstract string Name { get; }
        public virtual float TickRate { get; } = 0.0f; // default tick rate is 0 per second

        public State(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public virtual void OnStateEnter() { }
        public virtual void OnStateExit() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnTick() { }

        public void TryTick()
        {
            float tickInterval = 1.0f / TickRate;
            float currentTime = Time.time;

            if (currentTime - lastTickTime >= tickInterval)
            {
                lastTickTime = currentTime;
                OnTick();
            }
        }
    }
    public abstract class State<T> : State
    {
        protected T Owner;

        public State(StateMachine _stateMachine, T _owner) : base(_stateMachine)
        {
            Owner = _owner;
        }
    }
}


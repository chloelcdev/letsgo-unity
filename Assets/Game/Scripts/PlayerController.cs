using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;
using SapphireStateMachine;
using System;

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(AnimationController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float DeadZone = 0.1f;

    [SerializeField] new Collider2D collider;
    [SerializeField] new SpriteRenderer renderer;

    [SerializeField] StateMachine stateMachine;

    [SerializeField] AnimationController animationController;

    [SerializeField] PlayerStats stats;

    private Vector3 inputVector;

    private void Awake()
    {
        stateMachine = new StateMachine();
        stateMachine.SetState(new IdleState(this));
    }

    void PollInput()
    {
        inputVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
    }

    private void Update()
    {
        PollInput();
    }

    private void FixedUpdate()
    {
        transform.position += inputVector * stats.MovementSpeed * Time.fixedDeltaTime;
    }

    [Serializable]
    public class NamedAnimationClip
    {
        public string Name;
        public AnimationClip Clip;
    }

    #region States

    public class IdleState : State<PlayerController>
    {

        public override string Name => "Idle";

        public IdleState(PlayerController owner) : base(owner?.stateMachine, owner) { }

        public override void OnStateEnter()
        {
            Owner.animationController.PlayAnimation("Idle");
        }
        public override void OnUpdate()
        {
            if (Owner.inputVector.magnitude > Owner.DeadZone)
                stateMachine.SetState(new WalkState(Owner));
        }
    }

    public class WalkState : State<PlayerController>
    {
        public override string Name => "Walk";

        public WalkState(PlayerController owner) : base(owner?.stateMachine, owner) { }

        public override void OnStateEnter()
        {
            Owner.animationController.PlayAnimation("Walk");
        }

        public override void OnUpdate()
        {
            if (Owner.inputVector.magnitude <= Owner.DeadZone)
                stateMachine.SetState(new IdleState(Owner));
        }
    }

    public class AttackState : State<PlayerController>
    {
        public override string Name => "Attack";

        private readonly string animationName;
        public AnimationClip animation;

        private float endTime;

        public AttackState(StateMachine stateMachine, PlayerController owner, string animationName) : base(stateMachine, owner)
        {
            this.animationName = animationName;

            animation = Owner.animationController.GetClip(animationName);
            this.endTime = Time.time + animation.length;
        }
        

        public override void OnStateEnter()
        {
            Owner.animationController.PlayAnimation(animationName);
        }

        public override void OnUpdate()
        {
            if (Time.time >= endTime)
                stateMachine.SetState(new IdleState(Owner));
        }
    }

    #endregion
}


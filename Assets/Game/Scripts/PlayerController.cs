using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;
using SapphireStateMachine;
using System;

[RequireComponent(typeof(AnimationController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float DeadZone = 0.1f;

    [SerializeField] new Collider2D collider;
    [SerializeField] new SpriteRenderer renderer;

    [SerializeField] private StateMachine stateMachine;

    [SerializeField] AnimationController animator;

    [SerializeField] private float movementSpeed = 5f;

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

        if (inputVector != Vector3.zero)
            stateMachine.SetState(new WalkState(this));
        else
            stateMachine.SetState(new IdleState(this));
    }

    private void FixedUpdate()
    {
        transform.position += inputVector * movementSpeed * Time.fixedDeltaTime;
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
            Owner.animator.PlayAnimation("Idle");
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
            Owner.animator.PlayAnimation("Walk");
        }

        public override void OnUpdate()
        {
            if (Owner.inputVector.magnitude <= Owner.DeadZone)
                stateMachine.SetState(new IdleState(Owner));
        }
    }

    #endregion
}


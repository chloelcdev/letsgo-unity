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
    }

    [Serializable]
    public class NamedAnimationClip
    {
        public string Name;
        public AnimationClip Clip;
    }

    #region States

    public abstract class PlayerState : State<PlayerController>
    {
        protected PlayerState(PlayerController _owner) : base(_owner.stateMachine, _owner) { }

        protected void CheckForAttack()
        {
            if (Input.GetButtonDown("Fire1"))
                stateMachine.SetState(new AttackState(Owner, "Attack"));
        }

        protected void UpdateDirection()
        {
            if (Owner.inputVector != Vector3.zero)
            {
                float angle = Mathf.Atan2(Owner.inputVector.y, Owner.inputVector.x) * Mathf.Rad2Deg;
                Owner.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            }
        }
    }

    public class IdleState : PlayerState
    {

        public override string Name => "Idle";

        public IdleState(PlayerController owner) : base(owner) { }

        public override void OnStateEnter()
        {
            Owner.animationController.PlayAnimation("Idle");
        }
        public override void OnUpdate()
        {
            if (Owner.inputVector.magnitude > Owner.DeadZone)
                stateMachine.SetState(new WalkState(Owner));
            else
                CheckForAttack();
        }
    }

    public class WalkState : PlayerState
    {
        public override string Name => "Walk";

        public WalkState(PlayerController owner) : base(owner) { }

        public override void OnStateEnter()
        {
            Owner.animationController.PlayAnimation("Walk");
        }

        public override void OnUpdate()
        {
            if (Owner.inputVector.magnitude <= Owner.DeadZone)
                stateMachine.SetState(new IdleState(Owner));
            else
                CheckForAttack();

            UpdateDirection();
        }

        public override void OnFixedUpdate()
        {
            Owner.transform.position += Owner.inputVector * Owner.stats.MovementSpeed * Time.fixedDeltaTime;
        }


    }

    public class AttackState : PlayerState
    {
        public override string Name => "Attack";

        private readonly string animationName;
        public AnimationClip animation;

        private float endTime;

        Vector2 direction;

        public AttackState(PlayerController owner, string animationName) : base(owner)
        {
            this.animationName = animationName;

            animation = Owner.animationController.GetClip(animationName);
            this.endTime = Time.time + animation.length;

            direction = Owner.inputVector;

            UpdateDirection();
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


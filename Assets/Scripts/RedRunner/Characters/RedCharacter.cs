using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using UnityStandardAssets.CrossPlatformInput;

using RedRunner.Utilities;
using RedRunner.Networking;

namespace RedRunner.Characters
{

	public class RedCharacter : Character
	{

		#region Fields

		private static float FURTHEST_PLAYER_OVERSHOOT = 5f;

		[Header ( "Character Details" )]
		[Space]
		[SerializeField]
		protected float m_TargetRunSpeed = 8f;
		[SerializeField]
		protected float m_RunSmoothTime = 5f;
		[SerializeField]
		protected float m_RunSpeed = 5f;
		[SerializeField]
		protected float m_WalkSpeed = 1.75f;
		[SerializeField]
		protected float m_JumpStrength = 10f;
		[SerializeField]
		protected float m_DoubleJumpStrength = 8f;
		[SerializeField]
		protected float m_WallSlideSlowdown = 0.75f;
		[SerializeField]
		protected string[] m_Actions = new string[0];
		[SerializeField]
		protected int m_CurrentActionIndex = 0;

		[Header ( "Character Reference" )]
		[Space]
		[SerializeField]
		protected Rigidbody2D m_Rigidbody2D;
		[SerializeField]
		protected Collider2D m_Collider2D;
		[SerializeField]
		protected Mirror.NetworkAnimator m_Animator;
		[SerializeField]
		protected GroundCheck m_GroundCheck;
		[SerializeField]
		protected WallDetector m_WallDetector;
		[SerializeField]
		protected ParticleSystem m_RunParticleSystem;
		[SerializeField]
		protected ParticleSystem m_JumpParticleSystem;
		[SerializeField]
		protected ParticleSystem m_WaterParticleSystem;
		[SerializeField]
		protected ParticleSystem m_BloodParticleSystem;
		[SerializeField]
		protected Skeleton m_Skeleton;
        [SerializeField]
        protected Colourer m_Colourer;
        [SerializeField]
		protected float m_RollForce = 10f;

		[Header ( "Character Audio" )]
		[Space]
		[SerializeField]
		protected AudioSource m_MainAudioSource;
		[SerializeField]
		protected AudioSource m_FootstepAudioSource;
		[SerializeField]
		protected AudioSource m_JumpAndGroundedAudioSource;

		public delegate void PlayerEvent();

		public static event PlayerEvent LocalPlayerSpawned;

		public static event PlayerEvent OnTargetChanged;

        #endregion

        #region Private Variables
        protected CharacterState m_State = CharacterState.Stopped;
		protected bool m_ClosingEye = false;
		protected bool m_Guard = false;
		protected bool m_Block = false;
		protected Vector2 m_Speed = Vector2.zero;
		protected float m_CurrentRunSpeed = 0f;
		protected float m_CurrentSmoothVelocity = 0f;
		protected int m_CurrentFootstepSoundIndex = 0;
		protected Vector3 m_InitialScale;
        [SerializeField]
        private GameEvent m_LeftEvent;
        [SerializeField]
        private GameEvent m_RightEvent;
		protected bool m_HasDoubleJump = false;
		protected bool m_IsWallSliding = false;

		#endregion

		#region Properties

		public static RedCharacter Local { get; private set; }

		private static RedCharacter m_Target = null;
		public static RedCharacter Target
		{
			get
			{
				return m_Target;
			}

			private set
			{
				if (m_Target != value) {
					m_Target = value;
					OnTargetChanged();
				}
			}
		}

		public override float MaxRunSpeed
		{
			get
			{
				return m_TargetRunSpeed;
			}
		}

		public override float RunSmoothTime
		{
			get
			{
				return m_RunSmoothTime;
			}
		}

		public override float RunSpeed
		{
			get
			{
				return m_RunSpeed;
			}
		}

		public override float WalkSpeed
		{
			get
			{
				return m_WalkSpeed;
			}
		}

		public override float JumpStrength
		{
			get
			{
				return m_JumpStrength;
			}
		}

		public override Vector2 Speed
		{
			get
			{
				return m_Speed;
			}
		}

		public override string[] Actions
		{
			get
			{
				return m_Actions;
			}
		}

		public override string CurrentAction
		{
			get
			{
				return m_Actions [ m_CurrentActionIndex ];
			}
		}

		public override int CurrentActionIndex
		{
			get
			{
				return m_CurrentActionIndex;
			}
		}

		public override GroundCheck GroundCheck
		{
			get
			{
				return m_GroundCheck;
			}
		}

		public override Rigidbody2D Rigidbody2D
		{
			get
			{
				return m_Rigidbody2D;
			}
		}

		public override Collider2D Collider2D
		{
			get
			{
				return m_Collider2D;
			}
		}

		public override Animator Animator
		{
			get
			{
				return m_Animator.animator;
			}
		}

		public override ParticleSystem RunParticleSystem
		{
			get
			{
				return m_RunParticleSystem;
			}
		}

		public override ParticleSystem JumpParticleSystem
		{
			get
			{
				return m_JumpParticleSystem;
			}
		}

		public override ParticleSystem WaterParticleSystem
		{
			get
			{
				return m_WaterParticleSystem;
			}
		}

		public override ParticleSystem BloodParticleSystem
		{
			get
			{
				return m_BloodParticleSystem;
			}
		}

		public override Skeleton Skeleton
		{
			get
			{
				return m_Skeleton;
			}
		}

		public override bool ClosingEye
		{
			get
			{
				return m_ClosingEye;
			}
		}

		public override bool Guard
		{
			get
			{
				return m_Guard;
			}
		}

		public override bool Block
		{
			get
			{
				return m_Block;
			}
		}

		public override AudioSource Audio
		{
			get
			{
				return m_MainAudioSource;
			}
		}

		#endregion

		#region MonoBehaviour Messages

		void Awake ()
		{
            m_InitialScale = transform.localScale;
			m_GroundCheck.OnGrounded += GroundCheck_OnGrounded;
			m_WallDetector.OnWallEnter += StartWallSlide;
			m_WallDetector.OnWallExit += StopWallSlide;
			m_Skeleton.OnActiveChanged += Skeleton_OnActiveChanged;
			IsDead = new Property<bool>(false);
            IsFinished = new Property<bool>(false);
            m_ClosingEye = false;
			m_Guard = false;
			m_Block = false;
			m_CurrentFootstepSoundIndex = 0;
			GameManager.OnReset += GameManager_OnReset;

			// Default to static rigidbodies.
			// We don't want to perform physics simulations for other players.
			m_Rigidbody2D.bodyType = RigidbodyType2D.Static;

			LocalPlayerSpawned += () =>
			{
				// Once we find out we are the local player, simulate our rigidbody.
				Local.m_Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                m_RightEvent.RegisterAction(RightEvent);
				m_LeftEvent.RegisterAction(LeftEvent);
			};
		}

        void Start()
        {
            m_Colourer.SetColor(m_Colourer.RndRunnerColor(netId)); // TODO: have server assign color
        }
        
        private void LeftEvent()
        {
            if (Local != this)
            {
                return;
            }

            if (m_State == CharacterState.Left)
			{
				if (!m_WallDetector.TouchingWall || GroundCheck.IsGrounded || m_HasDoubleJump)
				{
					Jump();
				}
			}
			else{
                m_State = CharacterState.Left;
                ResetMovement();
				if (m_WallDetector.TouchingWall && !GroundCheck.IsGrounded)
				{
					StopWallSlide();
					Jump();
					ApplyHorizontalBoost(m_DoubleJumpStrength/2.0f);
				}
            }
		}
        private void RightEvent()
        {
            if (Local != this)
            {
                return;
            }

            if (m_State == CharacterState.Right)
			{
				if (!m_WallDetector.TouchingWall || GroundCheck.IsGrounded || m_HasDoubleJump)
				{
					Jump();
				}
			}
			else
			{
				m_State = CharacterState.Right;
				ResetMovement();
				if (m_WallDetector.TouchingWall && !GroundCheck.IsGrounded)
				{
					StopWallSlide();
					Jump();
					ApplyHorizontalBoost(m_DoubleJumpStrength/2.0f);
				}
			}
        }

        private void ResetMovement()
        {
            m_CurrentRunSpeed = m_RunSpeed;
        }

		public override void OnStartLocalPlayer()
		{
			base.OnStartLocalPlayer();

			Local = this;
			LocalPlayerSpawned();
		}

        private void UpdateMovement()
        {
             // Speed Calculations
            if (m_CurrentRunSpeed < m_TargetRunSpeed)
            {
                m_CurrentRunSpeed = Mathf.SmoothDamp(m_CurrentRunSpeed, m_TargetRunSpeed, ref m_CurrentSmoothVelocity, m_RunSmoothTime);
            }else if (m_CurrentRunSpeed > m_TargetRunSpeed)
			{
				m_CurrentRunSpeed = Mathf.SmoothDamp(m_CurrentRunSpeed, m_TargetRunSpeed, ref m_CurrentSmoothVelocity, m_RunSmoothTime);
			}

			// Input Processing

			switch (m_State)
            {
                case CharacterState.Left:
                    Move(-1f);
                    break;

                case CharacterState.Right:
                    Move(1f);
                    break;

                default:
                    // Don't move
                    break;
            }

            // Speed
            m_Speed = new Vector2(Mathf.Abs(m_Rigidbody2D.velocity.x), Mathf.Abs(m_Rigidbody2D.velocity.y));
        }

        void Update ()
		{
			ComputeTarget();

			if (Local != this)
			{
				return;
			}

			if ( transform.position.y < 0f )
			{
				Die ();
			}

			if ( m_Rigidbody2D.velocity.y < 0f && m_IsWallSliding)
			{
				Vector2 velocity = m_Rigidbody2D.velocity;
				velocity.y *= m_WallSlideSlowdown;
				m_Rigidbody2D.velocity = velocity;
			}

            UpdateMovement();

			if ( IsDead.Value && !m_ClosingEye )
			{
				StartCoroutine ( CloseEye () );
			}
			if ( CrossPlatformInputManager.GetButtonDown ( "Guard" ) )
			{
				m_Guard = !m_Guard;
			}
			if ( m_Guard )
			{
				if ( CrossPlatformInputManager.GetButtonDown ( "Fire" ) )
				{
					m_Animator.SetTrigger ( m_Actions [ m_CurrentActionIndex ] );
					if ( m_CurrentActionIndex < m_Actions.Length - 1 )
					{
						m_CurrentActionIndex++;
					}
					else
					{
						m_CurrentActionIndex = 0;
					}
				}
			}

			if ( Input.GetButtonDown ( "Roll" ) )
			{
				Vector2 force = new Vector2 ( 0f, 0f );
				if ( transform.localScale.z > 0f )
				{
					force.x = m_RollForce;
				}
				else if ( transform.localScale.z < 0f )
				{
					force.x = -m_RollForce;
				}
				m_Rigidbody2D.AddForce ( force );
			}
		}

		void LateUpdate ()
		{
			if (Local != this)
			{
				return;
			}

			m_Animator.animator.SetFloat ( "Speed", m_Speed.x );
			m_Animator.animator.SetFloat ( "VelocityX", Mathf.Abs ( m_Rigidbody2D.velocity.x ) );
			m_Animator.animator.SetFloat ( "VelocityY", m_Rigidbody2D.velocity.y );
			m_Animator.animator.SetBool ( "IsGrounded", m_GroundCheck.IsGrounded );
			m_Animator.animator.SetBool ( "IsDead", IsDead.Value );
            m_Animator.animator.SetBool ( "Block", m_Block );
			m_Animator.animator.SetBool ( "Guard", m_Guard );
			if ( Input.GetButtonDown ( "Roll" ) )
			{
				m_Animator.SetTrigger ( "Roll" );
			}
		}

		#endregion

		#region Private Methods

		private void ComputeTarget() {
			if (Local == null) {
				return;
			}

			if (!Local.IsDead.Value && !Local.IsFinished.Value)
			{
				Target = Local;
			} else if (!IsDead.Value && !IsFinished.Value)
			{
				if (Target == null ||
					Target.IsDead.Value ||
					Target.IsFinished.Value ||
					transform.position.x > Target.transform.position.x + FURTHEST_PLAYER_OVERSHOOT)
				{
					Target = this;
				}
			}
		}

		IEnumerator CloseEye ()
		{
			m_ClosingEye = true;
			yield return new WaitForSeconds ( 0.6f );
			while ( m_Skeleton.RightEye.localScale.y > 0f )
			{
				if ( m_Skeleton.RightEye.localScale.y > 0f )
				{
					Vector3 scale = m_Skeleton.RightEye.localScale;
					scale.y -= 0.1f;
					m_Skeleton.RightEye.localScale = scale;
				}
				if ( m_Skeleton.LeftEye.localScale.y > 0f )
				{
					Vector3 scale = m_Skeleton.LeftEye.localScale;
					scale.y -= 0.1f;
					m_Skeleton.LeftEye.localScale = scale;
				}
				yield return new WaitForSeconds ( 0.05f );
			}
		}

		#endregion

		#region Public Methods

		public virtual void PlayFootstepSound ()
		{
			if ( m_GroundCheck.IsGrounded )
			{
				AudioManager.Singleton.PlayFootstepSound ( m_FootstepAudioSource, ref m_CurrentFootstepSoundIndex );
			}
		}

		public override void Move ( float horizontalAxis )
		{
			if ( IsActive() )
			{
				float speed = m_CurrentRunSpeed;
				Vector2 velocity = m_Rigidbody2D.velocity;
				velocity.x = speed * horizontalAxis;
				m_Rigidbody2D.velocity = velocity;
				if ( horizontalAxis > 0f )
				{
					Vector3 scale = transform.localScale;
					scale.x = Mathf.Sign ( horizontalAxis );
					transform.localScale = scale;
				}
				else if ( horizontalAxis < 0f )
				{
					Vector3 scale = transform.localScale;
					scale.x = Mathf.Sign ( horizontalAxis );
					transform.localScale = scale;
				}
			}
		}

		public override void Jump ()
		{
			if (IsActive())
			{
				if (m_GroundCheck.IsGrounded)
				{
					ApplyJumpPhysics(m_JumpStrength);
				}
				else if (m_HasDoubleJump)
				{
					ApplyJumpPhysics(m_DoubleJumpStrength);
					m_HasDoubleJump = false;
				}
				else if (m_WallDetector.TouchingWall)
				{
					ApplyJumpPhysics(m_JumpStrength);
					m_HasDoubleJump = false;
				}
			}
		}

		private void ApplyJumpPhysics(float jumpStrength)
		{
			Vector2 velocity = m_Rigidbody2D.velocity;
			velocity.y = jumpStrength;
			m_Rigidbody2D.velocity = velocity;
			m_Animator.animator.ResetTrigger("Jump");
			m_Animator.SetTrigger("Jump");
			m_JumpParticleSystem.Play();
			AudioManager.Singleton.PlayJumpSound(m_JumpAndGroundedAudioSource);
		}

		private void ApplyHorizontalBoost(float boostStrength)
		{
			m_CurrentRunSpeed = boostStrength;
		}

		public void StartWallSlide()
		{
			m_IsWallSliding = true;
		}

		public void StopWallSlide()
		{
			m_IsWallSliding = false;
		}

		public override void Die ()
		{
			Die ( false );
		}

		public override void Die ( bool blood )
        {
            if (IsDead.Value) return;
            IsDead.Value = true;
            m_Skeleton.SetActive(true, m_Rigidbody2D.velocity);
            if (blood)
            {
                ParticleSystem particle = Instantiate<ParticleSystem>(
                                                m_BloodParticleSystem,
                                                transform.position,
                                                Quaternion.identity);
                Destroy(particle.gameObject, particle.main.duration);
            }
            CameraController.Singleton.fastMove = true;
            OnInactive();
        }

        public override void Finish()
        {
            if (IsFinished.Value) return;
            IsFinished.Value = true;
            m_Skeleton.SetActive( true, m_Rigidbody2D.velocity );
            // TODO dance?
            CameraController.Singleton.fastMove = true;
            OnInactive();
        }

        private bool IsActive()
        {
            return !IsDead.Value && !IsFinished.Value;
        }

        private void OnInactive()
        {
			if (Local == this) {
				RoundsManager.Local.CmdDeactivateSelf((int)Local.netId, IsFinished.Value);
			}
        }

        public override void EmitRunParticle ()
		{
			if ( IsActive() )
			{
				m_RunParticleSystem.Emit ( 1 );
			}
		}

		public override void Reset ()
		{
			IsDead.Value = false;
            IsFinished.Value = false;
            m_ClosingEye = false;
			m_Guard = false;
			m_Block = false;
			m_CurrentFootstepSoundIndex = 0;
			transform.localScale = m_InitialScale;
			m_Rigidbody2D.velocity = Vector2.zero;
			m_Skeleton.SetActive ( false, m_Rigidbody2D.velocity );
            m_State = CharacterState.Stopped;
			m_WallDetector.Reset();
			StopWallSlide();
		}

		#endregion

		#region Events

		void GameManager_OnReset ()
		{
			Reset ();
		}

		void Skeleton_OnActiveChanged ( bool active )
		{
			m_Animator.enabled = !active;
			m_Collider2D.enabled = !active;
			m_Rigidbody2D.simulated = !active;
		}

		void GroundCheck_OnGrounded ()
		{
			if ( IsActive() )
			{
				m_JumpParticleSystem.Play ();
				AudioManager.Singleton.PlayGroundedSound ( m_JumpAndGroundedAudioSource );
				m_HasDoubleJump = true;
			}
		}

		#endregion

		[System.Serializable]
		public class CharacterDeadEvent : UnityEvent
		{
			
		}

	}

}

using Sandbox;
using System;
using System.Linq;

namespace Minigolf
{
	/// <summary>
	/// Create an utter barebones Player, we don't want any actual player!
	/// </summary>
	public partial class GolfPlayer : Sandbox.Player
	{
		[NetPredicted]
		public Camera DevCamera { get; set; }

		public GolfBall Ball { get => ActiveChild as GolfBall; }

		[Net] public int Strokes { get; set; } = 0;

		public FollowBallCamera BallCamera { get; set; }

		public GolfPlayer()
        {
			if (Host.IsClient)
				BallCamera = new FollowBallCamera();
		}

		protected override void OnDestroy()
		{
			if ( IsServer && ActiveChild.IsValid() )
			{
				ActiveChild.Delete();
				ActiveChild = null;
			}

			base.OnDestroy();
		}

		public override void Respawn()
		{
			// Setup our dud controller and animator
			SetupControllerAndAnimator();

			// Disable everythong on the player
			PhysicsEnabled = false;
			EnableAllCollisions = false;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = false;

			Transmit = TransmitType.Always;

			SetupGolfBall();

			// needed?
			UpdatePhysicsHull();
			ResetInterpolation();

			// do i even wanna do this
			GameBase.Current?.PlayerRespawn(this);
		}

		public void SetupGolfBall()
		{
			if ( ActiveChild.IsValid() )
				return;
		
			var ball = new GolfBall();
			ball.Player = this;
			ActiveChild = ball;

			Pvs.Add( ball );

			ball.ResetPosition( Vector3.Zero );
		}

		// CLIENTSIDE todo: move to controller
		public float ShotPower = 0.0f;

		public override void BuildInput(ClientInput input)
		{
			Host.AssertClient();

			VoiceRecord = input.Down(InputButton.Voice);

			GetActiveCamera()?.BuildInput(input);

			// Devcam might stop processing
			if (input.StopProcessing)
				return;

			// Okay maybe this should be a controller or something?
			if (input.Down(InputButton.Attack1))
			{
				ShotPower = Math.Clamp(ShotPower - input.AnalogLook.pitch, 0, 100);
			}

			if (ShotPower > 0.0f && !input.Down(InputButton.Attack1))
            {
				ConsoleSystem.Run("minigolf_stroke", BallCamera.Angles.yaw, (int)ShotPower);
				ShotPower = 0;
            }
		}

		public override void PostCameraSetup(Camera camera)
		{
			Host.AssertClient();
		}

		public override Camera GetActiveCamera()
		{
			if (DevCamera != null)
				return DevCamera;

			// If the Game wants to show a cinematic camera here let it
			if ((Game.Current as GolfGame).WaitingToStart)
					return (Game.Current as GolfGame).MapCamera;

			// Otherwise use our BallCamera
			return BallCamera;
		}

		protected override void Tick()
		{
			if ( ActiveChild is IPlayerControllable playerController )
			{
				playerController.OnPlayerControlTick( this );
			}
		}
	}
}

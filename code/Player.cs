using Sandbox;
using System;
using System.Linq;

namespace Minigolf
{
	/// <summary>
	/// Create an utter barebones Player, we don't want any actual player!
	/// </summary>
	partial class GolfPlayer : Sandbox.Player
	{
		[NetPredicted]
		public FollowBallCamera BallCamera { get; set; }

		[NetPredicted]
		public PlayerBall Ball { get; set; }

		public override void Respawn()
		{
			if (Ball == null)
				Ball = new PlayerBall();

			Ball.WorldPos = (Game.Current as GolfGame).FindBallSpawn(0);

			// Setup our dud controller and animator
			SetupControllerAndAnimator();

			BallCamera = new FollowBallCamera();

			// Disable everythong on the player
			PhysicsEnabled = false;
			EnableAllCollisions = false;
			EnableDrawing = false;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = false;

			// needed?
			UpdatePhysicsHull();
			ResetInterpolation();

			// do i even wanna do this
			GameBase.Current?.PlayerRespawn(this);
		}

		// CLIENTSIDE todo: move to controller
		public float ShotPower = 0.0f;

		public override void BuildInput(ClientInput input)
		{
			Host.AssertClient();

			VoiceRecord = input.Down(InputButton.Voice);

			// Okay maybe this should be a controller or something?
			if (input.Down(InputButton.Attack1))
			{
				ShotPower = Math.Clamp(ShotPower - input.AnalogLook.pitch, 0, 100);
			}

			if (ShotPower > 0.0f && !input.Down(InputButton.Attack1))
            {
				ConsoleSystem.Run("golf_shoot", BallCamera.Angles.yaw, ShotPower);

				Log.Info($"Power: {ShotPower}");
				ShotPower = 0;
            }

			GetActiveCamera()?.BuildInput(input);
		}

		public override void PostCameraSetup(Camera camera)
		{
			Host.AssertClient();
		}

		public override Camera GetActiveCamera()
		{
			// If the Game wants to show a cinematic camera here let it

			// Otherwise use our BallCamera
			return BallCamera;
		}

		protected override void Tick()
		{

		}
	}
}

using Sandbox;
using System;
using System.Threading.Tasks;

namespace Minigolf
{
	[Library("simple_moving_obstacle")]
	public partial class SimpleMovingObstacle : KeyframeEntity
	{
		public enum Flags
		{
			RotateBackwards = 2,
			NonSolidToPlayer = 4,
			Passable = 8,
			OneWay = 16,
			NoAutoReturn = 32,
			Roll = 64,
			Pitch = 128,
			Use = 256,
			NoNpcs = 512,
			Touch = 1024,
			StartLocked = 2048,
			Silent = 4096,
			UseCloses = 8192,
			SilentToNpcs = 16384,
			IgnoreUse = 32768,
			StartUnbreakable = 524288,
		}

		public enum DoorState
		{
			Open,
			Closed,
			Opening,
			Closing
		}

		[Property("movedir")]
		public Angles MoveDir { get; set; }

		[Property( "movedir_islocal")]
		public bool MoveDirIsLocal { get; set; }

		[Property( "spawnpos")]
		public bool SpawnOpen { get; set; }

		[Property( "lip")]
		public float Lip { get; set; }

		[Property( "speed")]
		public float Speed { get; set; }

		[Property( "wait")]
		public float TimeBeforeReset { get; set; }

		Vector3 PositionA;
		Vector3 PositionB;

		public DoorState State { get; protected set; } = DoorState.Open;

		public override void Spawn()
		{
			base.Spawn();

			SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

			PositionA = Position;

			// Get the direction we want to move
			var dir = Rotation.From(MoveDir).Forward;
			if (MoveDirIsLocal) dir = Transform.NormalToWorld(dir);

			// Open position is the size of the bbox in the direction minus the lip size
			var boundSize = OOBBox.Size;

			PositionB = Position + dir * (MathF.Abs(boundSize.Dot(dir)) - Lip);

			State = DoorState.Closed;

			if (SpawnOpen)
			{
				Position = PositionB;
				State = DoorState.Open;
			}

			// Auto start
			_ = AutoStart();
		}

		async Task AutoStart()
		{
			while (true)
            {
				if (State == DoorState.Open || State == DoorState.Opening) State = DoorState.Closing;
				else if (State == DoorState.Closed || State == DoorState.Closing) State = DoorState.Opening;

				var position = (State == DoorState.Opening) ? PositionB : PositionA;
				await DoMove(position);
				await Task.DelaySeconds(TimeBeforeReset);
			}
		}

		async Task DoMove(Vector3 position)
		{
			var tx = Transform;

			var distance = Vector3.DistanceBetween( tx.Position, position );
			var timeToTake = distance / Speed;

			tx.Position = position;

			_ = await KeyframeTo( tx, timeToTake, null );
		}
	}
}

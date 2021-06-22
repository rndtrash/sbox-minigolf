using Sandbox;
using Sandbox.Internal;

namespace Minigolf
{
	[Library( "simple_rotating", Description = "A rotating mesh entity." )]
	[Hammer.Solid]
	[Hammer.RenderProperties]
	[Hammer.DrawAngles( nameof( AxisDirection ), nameof( AxisIsLocal ) )]
	public partial class SimpleRotating : ModelEntity
	{
		[Property]
		[DefaultValue( true )]
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// The rotation speed of the entity in degrees per second.
		/// Negative speed can be used to change direction.
		/// </summary>
		[Property]
		[DefaultValue( 100.0f )]
		public float Speed { get; set; } = 100.0f;

		/// <summary>
		/// The direction of the axis the mesh will rotate on.
		/// If AxisIsLocal is defined this is relative to the entitiy's current rotation.
		/// </summary>
		/// <remarks>
		/// We define this as Angles rather then a normalized Vector3 so we can take
		/// advantage of Hammer's drawangles.
		/// </remarks>
		[Property( Title = "Axis Direction (Pitch Yaw Roll)" )]
		public Angles AxisDirection { get; set; }

		/// <summary>
		/// Is the axis local to the entity's current rotation?
		/// This can be useful for more specific behaviour in nested entities.
		/// </summary>
		[Property( Title = "Axis Direction Is Local" )]
		[DefaultValue( true )]
		public bool AxisIsLocal { get; set; } = true;

		protected bool _solid;

		/// <summary>
		/// Whether this entity has collisions.
		/// </summary>
		[Property]
		[DefaultValue( true )]
		public bool Solid
		{
			get => _solid;
			set
			{
				_solid = value;
				EnableAllCollisions = value;
			}
		}

		// On spawn use the world rotation for an easy World -> Local direction transform.
		protected Rotation InitialWorldRotation;

		protected Vector3 Axis
		{
			get => Angles.AngleVector( AxisDirection ) * (AxisIsLocal ? Rotation.Identity : InitialWorldRotation);
		}

		public override void Spawn()
		{
			base.Spawn();

			// Keep track of initial rotation for world axis
			InitialWorldRotation = Rotation;

			EnableAllCollisions = Solid;

			SetupPhysicsFromModel( PhysicsMotionType.Static );
		}

		[Event.Tick]
		protected void UpdateRotation()
		{
			// This entity makes sense to sometimes be created clientside only for aethestic purposes.
			// Otherwise we should only set it serverside.
			if ( IsClient && !IsClientOnly )
				return;

			if ( !Enabled )
				return;

			// Make sure we use the quaternion directly instead of using angles, prevents any locks.
			// Tick rate can be variable, so use the tick interval on our speed in degrees per second.
			LocalRotation *= Rotation.FromAxis( Axis, Speed * Global.TickInterval );
		}

		/// <summary>
		/// Start the rotator rotating.
		/// </summary>
		[Input]
		protected void Start() => Enabled = true;

		/// <summary>
		/// Stop the rotator from rotating.
		/// </summary>
		[Input]
		protected void Stop() => Enabled = false;

		/// <summary>
		/// Toggle the rotator between rotating and not rotating.
		/// </summary>
		[Input]
		protected void Toggle() => Enabled = !Enabled;
	}
}

﻿using System.Linq;
using Sandbox;

namespace Woosh.Espionage;

public partial class Pawn : AnimatedEntity
{
	public override void Spawn()
	{
		Model = Model.Load( "models/sbox_props/watermelon/watermelon.vmdl" );

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	// An example BuildInput method within a player's Pawn class.
	[ClientInput] public Vector3 InputDirection { get; protected set; }
	[ClientInput] public Angles ViewAngles { get; set; }

	public override void BuildInput()
	{
		InputDirection = Input.AnalogMove;

		var look = Input.AnalogLook;

		var viewAngles = ViewAngles;
		viewAngles += look;
		ViewAngles = viewAngles.Normal;
	}

	/// <summary>
	/// Called every tick, clientside and serverside.
	/// </summary>
	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		Components.GetOrCreate<InteractionHandler>().Simulate( cl );

		if ( Input.Pressed( "slot3" ) )
		{
			var inv = Components.Get<InventoryContainer>();
			Components.GetOrCreate<CarriableHandler>().Deploy( inv.Get<Mark23Weapon>(), 1, 0.6f );
		}

		Rotation = ViewAngles.ToRotation();

		// build movement from the input values
		var movement = InputDirection.Normal;

		// rotate it to the direction we're facing
		Velocity = Rotation * movement;

		// apply some speed to it
		Velocity *= Input.Down( InputButton.Run ) ? 1000 : 200;

		if ( Game.IsServer && Input.Down( InputButton.Drop ) )
		{
			var inv = Components.Get<IEntityInventory>();
			inv.Drop( inv.All.FirstOrDefault() );
		}

		Components.GetOrCreate<CarriableHandler>().Simulate( cl );

		// apply it to our position using MoveHelper, which handles collision
		// detection and sliding across surfaces for us
		MoveHelper helper = new MoveHelper( Position, Velocity );
		helper.Trace = helper.Trace.Size( 16 );
		if ( helper.TryMove( Time.Delta ) > 0 )
		{
			Position = helper.Position;
		}

		// If we're running serverside and Attack1 was just pressed, spawn a ragdoll
		if ( Game.IsServer && Input.Pressed( "shoot" ) )
		{
			var ragdoll = new Prop();
			ragdoll.Model = Model.Load( "models/sbox_props/watermelon/watermelon.vmdl" );
			ragdoll.Position = Position + Rotation.Forward * 40;
			ragdoll.Rotation = Rotation.LookAt( Vector3.Random.Normal );
			ragdoll.Scale = 0.4f;
			ragdoll.PhysicsGroup.Velocity = Rotation.Forward * 1000;
		}
	}

	/// <summary>
	/// Called every frame on the client
	/// </summary>
	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );

		// Update rotation every frame, to keep things smooth
		Rotation = ViewAngles.ToRotation();

		Camera.Position = Position;
		Camera.Rotation = Rotation;

		// Set field of view to whatever the user chose in options
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );

		// Set the first person viewer to this, so it won't render our model
		Camera.FirstPersonViewer = this;
	}
}

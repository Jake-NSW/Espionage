﻿using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed partial class FirearmShootSimulatedEntityState : ObservableEntityComponent<Firearm>, ISimulatedEntityState<Firearm>
{
	public FirearmSetup Setup => Entity.Setup;

	public bool TryEnter()
	{
		return IsFireable( true );
	}

	public bool Simulate( IClient cl )
	{
		if ( IsFireable() )
			Shoot();

		return !Setup.IsAutomatic || !Input.Down( "shoot" );
	}

	public void OnStart() { }
	public void OnFinish() { }

	// Logic

	protected override void OnActivate()
	{
		base.OnActivate();
		n_SinceLastShot = 0;
	}

	[Net, Predicted, Local] private TimeSince n_SinceLastShot { get; set; }

	public bool IsFireable( bool checkInput = false )
	{
		// Check if any state is running
		if ( checkInput )
		{
			// Check for input
			if ( Setup.IsAutomatic ? !Input.Down( "shoot" ) : !Input.Pressed( "shoot" ) )
				return false;
		}

		return n_SinceLastShot >= 60 / Setup.RateOfFire;
	}

	public void Shoot()
	{
		n_SinceLastShot = 0;

		if ( !Prediction.FirstTime )
			return;

		Events.Run( new WeaponFired( new Vector3( -3, 0.2f, 0.2f ) * 35, new Vector3( -1, 0.2f, 0.2f ) * 35 ) );

		// Play Effects
		// PlayClientEffects( WeaponClientEffects.Attack );

		// Owner, Shoot from View Model
		if ( Entity.IsLocalPawn )
		{
			var muzzle = Entity.Effects?.Target?.GetAttachment( "muzzle" ) ?? Entity.Owner.Transform;
			CmdReceivedShootRequest( NetworkIdent, muzzle.Position, muzzle.Rotation.Forward );
			return;
		}

		// No Owner, Shoot from World Model
		if ( Entity.Owner == null && Game.IsServer ) { }
	}

	[ConCmd.Server]
	private static void CmdReceivedShootRequest( int indent, Vector3 pos, Vector3 forward )
	{
		_ = new Prop
		{
			Model = Model.Load( "models/sbox_props/watermelon/watermelon.vmdl" ),
			Position = pos + forward,
			Rotation = Rotation.LookAt( Vector3.Random.Normal ),
			Scale = 0.4f,
			PhysicsGroup = { Velocity = forward * 1000 }
		};
	}

}

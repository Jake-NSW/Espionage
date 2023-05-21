﻿using System.Linq;
using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class Operator : Pawn, IMutateCameraSetup
{
	public Entity Active => Components.Get<CarriableHandler>().Active;
	public IEntityInventory Inventory => Components.Get<IEntityInventory>();
	public DeployableSlotHandler Hands => Components.Get<DeployableSlotHandler>();
	
	public override void Spawn()
	{
		base.Spawn();

		// Gameplay
		Components.Create<PawnLeaning>();
		Components.Create<WalkController>();

		// Interaction
		Components.Create<InteractionHandler>();
		Components.Create<PushEntityInteraction>();
		Components.Create<UseEntityInteraction>();
		Components.Create<PickupEntityInteraction>();
		Components.Create<EquipEntityInteraction>();
		Components.Create<ControllableEntityInteraction>();

		// Inventory
		Components.Create<CarriableHandler>();
		Components.Create<InventoryContainer>();
		Components.Add( new DeployableSlotHandler( 3 ) );
	}

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		(Active as IMutateCameraSetup)?.OnPostCameraSetup( ref setup );

		foreach ( var component in Components.All().OfType<IMutateCameraSetup>() )
		{
			component.OnPostCameraSetup( ref setup );
		}
	}

	protected override void OnPostInputBuild( ref InputContext context )
	{
		(Active as IMutateInputContext)?.OnPostInputBuild( ref context );
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		Components.Get<InteractionHandler>()?.Simulate( cl );
		Components.Get<CarriableHandler>()?.Simulate( cl );
		Components.Get<PawnLeaning>()?.Simulate( cl );

		if ( Input.Pressed( "slot_primary" ) )
		{
			Hands.Deploy( CarrySlot.Front );
		}

		if ( Input.Pressed( "slot_secondary" ) )
		{
			Hands.Deploy( CarrySlot.Back );
		}

		if ( Input.Pressed( "slot_holster" ) )
		{
			Hands.Deploy( CarrySlot.Holster );
		}

		if ( Input.Pressed( "drop" ) )
		{
			Hands.Drop( Hands.Active );
		}
	}
}

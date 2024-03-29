﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class InteractionHandler : ObservableEntityComponent<Pawn>, ISingletonComponent
{
	public InteractionHandler()
	{
		m_Interactions = Array.Empty<IEntityInteraction>();
	}

	protected override void OnAutoRegister()
	{
		Register<DeployingEntity>( OnDeployed );
		Register<DeployedEntity>( OnDeployed );
		Register<SimulateSnapshot>( OnSimulate );
	}

	private void OnDeployed()
	{
		// Some Holdable Entities add Components
		Rebuild();
	}

	// State

	public IReadOnlyList<IEntityInteraction> Interactions => m_Interactions;
	private IEntityInteraction[] m_Interactions;

	public Entity Hovering => p_Hovering;
	[Predicted] private Entity p_Hovering { get; set; }

	private void OnSimulate( Event<SimulateSnapshot> evt )
	{
		var result = Scan();
		var hovering = result.Entity;

		if ( p_Hovering != hovering )
		{
			p_Hovering = hovering;
			Rebuild();
		}

		foreach ( var interaction in Interactions )
		{
			if ( Game.IsClient )
				interaction.OnHovering( hovering );

			interaction.Simulate( result, evt.Signal.Client );
		}
	}

	public void Rebuild()
	{
		m_Interactions = p_Hovering == null ? Array.Empty<IEntityInteraction>() : Entity.Components.GetAll<IEntityInteraction>().Where( e => e.IsInteractable( p_Hovering ) ).ToArray();
		var hovering = m_Interactions.Length == 0 ? null : p_Hovering;
		Run( new InteractionTargetChanged( hovering, m_Interactions ) );
	}

	private TraceResult Scan( float size = 8 )
	{
		var eyes = Entity.AimRay;
		var ray = Trace.Ray( eyes.Position, eyes.Position + eyes.Forward * 72 ).Ignore( Entity ).DynamicOnly();

		var first = ray.Run();
		return first.Entity != null ? first : ray.Size( size ).Run();
	}
}

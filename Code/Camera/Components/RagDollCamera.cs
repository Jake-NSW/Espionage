﻿using Sandbox;

namespace Woosh.Espionage;

public sealed class RagDollCamera : CameraController
{
	public RagDollCamera( Entity entity ) : base( entity ) { }

	protected override void Enabled( ref CameraSetup setup )
	{
		setup.Position = Entity.Position + Vector3.Up * 72 + Vector3.Backward * 96;
	}

	protected override void Update( ref CameraSetup setup, in InputContext input )
	{
		var center = Entity.WorldSpaceBounds.Center;
		var rot = input.ViewAngles.ToRotation();

		var distance = 130.0f * Entity.Scale;
		var targetPos = center + Entity.Scale;
		targetPos += rot.Forward * -distance;

		var tr = Trace.Ray( center, targetPos )
			.WithAnyTags( "solid" )
			.Ignore( Entity )
			.Radius( 8 )
			.Run();

		setup.Position = tr.EndPosition;
		setup.Rotation = rot;
	}
}
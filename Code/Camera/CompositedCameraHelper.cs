﻿using Sandbox;

namespace Woosh.Espionage;

public sealed class CompositedCameraHelper
{
	public SceneCamera Target { get; }

	public CompositedCameraHelper( SceneCamera camera )
	{
		Game.AssertClient();
		Target = camera;
	}

	// Setup

	public CameraMutateScope Update( InputContext input, IController<CameraSetup> controller = null )
	{
		var setup = new CameraSetup( Target ) { FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView ) };
		Build( ref setup, input, controller );
		return new CameraMutateScope( Target, setup );
	}

	// Active

	private IController<CameraSetup> m_Last;

	private void Build( ref CameraSetup setup, in InputContext input, IController<CameraSetup> controller )
	{
		var maybe = controller ?? Find();
		if ( m_Last != maybe )
		{
			m_Last?.Disabled();
			m_Last = maybe;
			m_Last?.Enabled( ref setup );
		}

		m_Last?.Update( ref setup, input );
	}

	private static IController<CameraSetup> Find()
	{
		// Pull camera from Pawn if it exists
		if ( Game.LocalPawn is Pawn pawn )
			return pawn.Camera;

		return null;
	}
}

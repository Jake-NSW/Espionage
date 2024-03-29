﻿using System;
using Sandbox;

namespace Woosh.Espionage;

public delegate void CameraMutateDelegate( ref CameraSetup setup );

public struct CameraMutateScope : IDisposable
{
	private readonly SceneCamera m_Camera;

	public CameraMutateScope( SceneCamera camera, CameraSetup setup )
	{
		m_Camera = camera;
		m_Setup = setup;
	}

	private CameraSetup m_Setup;

	public void Mutate( IMutate<CameraSetup> mutate )
	{
		mutate?.OnMutate( ref m_Setup );
	}

	public void Mutate( CameraMutateDelegate mutate )
	{
		mutate?.Invoke( ref m_Setup );
	}

	public void Dispose()
	{
		m_Camera.Position = m_Setup.Position;
		m_Camera.Rotation = m_Setup.Rotation;
		m_Camera.FirstPersonViewer = m_Setup.Viewer;
		m_Camera.FieldOfView = m_Setup.FieldOfView;

		var originalFov = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );
		var offset = originalFov - m_Camera.FieldOfView;
		
		m_Camera.Attributes.Set( "viewModelFov", Screen.CreateVerticalFieldOfView( 60 ) - offset );
	}
}

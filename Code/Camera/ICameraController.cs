﻿using Sandbox;

namespace Woosh.Espionage;

// Controller

public interface IEntityCameraController : IComponent, ICameraController { }

public interface ICameraController
{
	void Enabled( ref CameraSetup setup );
	void Update( ref CameraSetup setup, in InputContext input );
	void Disabled();
}

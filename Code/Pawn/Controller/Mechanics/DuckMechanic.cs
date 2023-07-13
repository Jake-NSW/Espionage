﻿using Sandbox;

namespace Woosh.Espionage;

public abstract class PlayerControllerMechanic : EntityComponent<Pawn>, IPostMutate<CameraSetup>
{
	void IPostMutate<CameraSetup>.OnPostMutate( ref CameraSetup setup ) => OnCameraSetup( ref setup );
	protected virtual void OnCameraSetup( ref CameraSetup setup ) { }
}

public sealed class StanceMechanic : PlayerControllerMechanic
{
	public void Simulate( IClient cl, InputContext context ) { }
}

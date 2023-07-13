﻿using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class RustFirearmViewModelAnimator : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	protected override void OnAutoRegister()
	{
		Register<WeaponFired>( () => Entity.SetAnimParameter( "fire", true ) );
		Register<DeployingEntity>( () => Entity.SetAnimParameter( "deploy", true ) );
	}

	void IPostMutate<CameraSetup>.OnPostMutate( ref CameraSetup setup ) { }
}

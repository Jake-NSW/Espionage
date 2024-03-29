﻿using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class SandboxFirearmViewModelAnimator : ObservableEntityComponent<ViewModel>, IViewModelEffect
{
	protected override void OnAutoRegister()
	{
		base.OnAutoRegister();
		
		Register<FirearmFired>( () => Entity.SetAnimParameter( "b_attack", true ) );
		Register<ReloadStarted>( () => Entity.SetAnimParameter( "b_reload", true ) );
		Register<PawnLanded>( () => Entity.SetAnimParameter( "b_grounded", true ) );
		Register<DeployingEntity>( () => Entity.SetAnimParameter( "b_deploy", true ) );
	}

	void IMutate<CameraSetup>.OnMutate( ref CameraSetup setup ) { }
}

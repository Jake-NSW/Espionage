﻿using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class GenericFirearmViewmodelAnimator : ObservableEntityComponent<Firearm>
{
	protected override void OnActivate()
	{
		Events.Register<WeaponFired>( OnShoot );
	}

	protected override void OnDeactivate()
	{
		Events.Unregister<WeaponFired>( OnShoot );
	}

	private void OnShoot()
	{
		(Entity?.Effects.Target as AnimatedEntity)?.SetAnimParameter( "bFire", true );
	}
}

﻿using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class RustFirearmViewmodelAnimator : ObservableEntityComponent<Firearm>
{
	protected override void OnActivate()
	{
		Events.Register<WeaponFireEvent>( OnShoot );
	}

	protected override void OnDeactivate()
	{
		Events.Unregister<WeaponFireEvent>( OnShoot );
	}

	private void OnShoot()
	{
		(Entity?.Effects.Target as AnimatedEntity)?.SetAnimParameter( "fire", true );
	}
}

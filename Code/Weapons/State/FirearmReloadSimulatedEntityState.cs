﻿using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed partial class FirearmReloadSimulatedEntityState : ObservableEntityComponent<Firearm>, ISimulatedEntityState<Firearm>, ISingletonComponent
{
	public bool TryEnter()
	{
		return Input.Pressed( "reload" );
	}

	[Net, Predicted, Local] private TimeSince n_SinceReload { get; set; }

	public bool Simulate( IClient cl )
	{
		// We're Done!
		return n_SinceReload > 3;
	}

	public void OnStart()
	{
		n_SinceReload = 0;
		
		Run( new ReloadStarted() );
	}
	
	public void OnFinish() { }
}

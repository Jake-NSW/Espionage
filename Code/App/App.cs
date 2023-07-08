﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed partial class App : GameManager, IObservable
{
	public new static App Current
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		get => GameManager.Current as App;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Get<T>() where T : EntityComponent
	{
		return Current.Components.Get<T>();
	}

	// Instance

	public IDispatcher Events { get; }

	public App()
	{
		Events = new Dispatcher( this, _ => GlobalEventDispatcher.Instance, _ => null );

		// Setup Components
		if ( Game.IsServer )
		{
			Components.Create<CameraBuilderComponent>();
			Components.Create<GamemodeHandlerComponent>();
			Components.Create<ProjectileSimulator>();
		}
	}

	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );
		Events.Run<FrameUpdate>();
	}

	public override void Simulate( IClient cl )
	{
		Components.Each<ISimulated, IClient>( cl, ( client, simulated ) => simulated.Simulate( client ) );
		base.Simulate( cl );
	}

	[GameEvent.Client.PostCamera]
	private void OnPostCamera()
	{
		Events.Run<PostCameraSetup>();
	}

	public override void ClientJoined( IClient client )
	{
		Events.Run( new ClientJoined( client ) );
		base.ClientJoined( client );

		var spawn = All.OfType<SpawnPoint>().MinBy( _ => Guid.NewGuid() ).Transform;
		spawn.Position += Vector3.Up * 4;

		var pistol = new Mk23Firearm();
		var smg = new Smg2Firearm();
		
		var pawn = client.Possess<Operator>( spawn );
		pawn.Inventory.Add( pistol, smg );
		pawn.Slots.Deploy( CarrySlot.Front );
	}

	public override void ClientDisconnect( IClient cl, NetworkDisconnectionReason reason )
	{
		Events.Run( new ClientDisconnected( cl, reason ) );
		base.ClientDisconnect( cl, reason );
	}

	private async static Task LoadMapFromDragDrop( string url )
	{
		var package = await Package.FetchAsync( url, true );
		if ( package.PackageType == Package.Type.Map )
			Game.ChangeLevel( package.FullIdent );
	}

	public override bool OnDragDropped( string text, Ray ray, string action )
	{
		if ( action == "drop" )
			_ = LoadMapFromDragDrop( text );

		return base.OnDragDropped( text, ray, action );
	}
}

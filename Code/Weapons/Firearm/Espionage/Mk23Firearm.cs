﻿using Editor;
using Sandbox;
using Woosh.Espionage;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library( "esp_mk23_firearm" ), HammerEntity, EditorModel( WORLD_MODEL )]
public sealed class Mk23Firearm : Firearm, ISlotted<CarrySlot>, IHave<EntityInfo>
{
	private const string WORLD_MODEL = "weapons/mk23/espionage_mk23.vmdl";

	public EntityInfo Item { get; } = new EntityInfo
	{
		Nickname = "MK23",
		Display = "SOCOM Mark23",
		Brief = "Heckler & Koch",
		Icon = "gavel",
		Description = "A semi-automatic large framed pistol, chambered in .45 ACP.",
		Group = "weapon"
	};

	public Mk23Firearm()
	{
		if ( !Game.IsClient )
			return;

		Events.Register<ValidAmmoProviderCheck>( static evt => evt.Signal.AddType<Mk23StandardMagazine>() );
		Events.Register<PlayClientEffects<FirearmClientEffects>>( static evt => Sounds.Play( evt.Signal.Effects, Game.LocalPawn.AimRay.Position ) );
		Events.Register<CreatedViewModel>(
			static evt => evt.Signal.ViewModel.Build()
				.WithModel( Cloud.Model( "woosh.mdl_esp_vmk23" ) )
				.WithAspect( new ViewModelEffectsAspect() )
				.WithComponent( new EspionageFirearmViewModelAnimator() )
				.WithMaterialGroup( "chrome" )
				.WithBodyGroup( "muzzle", 0 )
				.WithBodyGroup( "module", 1 )
		);
	}

	public override void Spawn()
	{
		base.Spawn();

		Model = Cloud.Model( "woosh.mdl_esp_mk23" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	protected override FirearmSetup Default => new FirearmSetup()
	{
		IsAutomatic = false,
		IsSilenced = false,
		RateOfFire = 550,
		Force = 400,
		Spread = 0.2f,
		Draw = new DrawTime( 1, 0.6f )
	};

	public CarrySlot Slot => CarrySlot.Holster;

	private static SoundBank<FirearmClientEffects> Sounds { get; } = new SoundBank<FirearmClientEffects>()
	{
		[FirearmClientEffects.Attack] = "mk23_firing_sound", 
		[FirearmClientEffects.Attack | FirearmClientEffects.Silenced] = "mk23_firing_suppressed_sound",
	};
}

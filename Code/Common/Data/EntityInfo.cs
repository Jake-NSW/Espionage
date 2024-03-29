﻿using Sandbox;
using Sandbox.ModelEditor;

namespace Woosh.Espionage;

[GameData( "esp_entity_info" )]
public readonly struct EntityInfo
{
	public static EntityInfo FromDisplayInfo( DisplayInfo info )
	{
		return new EntityInfo()
		{
			Display = info.Name,
			Icon = info.Icon,
			Group = info.Group,
			Description = info.Description
		};
	}

	public static EntityInfo FromEntity( Entity ent )
	{
		if ( ent is IHave<EntityInfo> info )
			return info.Item;

		if ( ent is ModelEntity mdl && mdl.Model?.HasData<EntityInfo>() == true )
			return mdl.Model.GetData<EntityInfo>();

		return FromDisplayInfo( ent.Info() );
	}

	public bool IsValid => Nickname != null || Display != null || Brief != null || Icon != null || Description != null || Group != null;

	public string Nickname { get; init; }
	public string Display { get; init; }
	public string Brief { get; init; }
	public string Icon { get; init; }
	public string Description { get; init; }
	public string Group { get; init; }
}

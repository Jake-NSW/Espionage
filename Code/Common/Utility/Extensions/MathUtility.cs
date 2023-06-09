﻿using System;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Common;

public static class MathUtility
{
	// Damping

	public static float Damp( this float value, float to, float damp )
	{
		damp = 1 - MathF.Pow( damp, Time.Delta );
		return MathX.Lerp( value, to, damp );
	}

	public static Vector3 Damp( this Vector3 value, Vector3 to, float damp )
	{
		damp = 1 - MathF.Pow( damp, Time.Delta );
		return Vector3.Lerp( value, to, damp );
	}

	public static Rotation Damp( this Rotation value, Rotation to, float damp )
	{
		damp = 1 - MathF.Pow( damp, Time.Delta );
		return Rotation.Lerp( value, to, damp );
	}

	// Float

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Abs( this float value )
	{
		return MathF.Abs( value );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Max( this float value, float max )
	{
		return MathF.Max( value, max );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Round( this float value, int digits )
	{
		return MathF.Round( value, digits );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Min( this float value, float min )
	{
		return MathF.Min( value, min );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static float Spring( float from, float to, float time )
	{
		time = time.Clamp( 0f, 1f );
		time = (MathF.Sin( time * MathF.PI * (.2f + 2.5f * time * time * time) ) * MathF.Pow( 1f - time, 2.2f ) + time) * (1f + (1.2f * (1f - time)));
		return from + (to - from) * time;
	}

	// Quaternion

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotation WithPitch( this Rotation rotation, float pitch )
	{
		var rot = rotation * Rotation.FromPitch( rotation.Pitch() ).Inverse;
		rot *= Rotation.FromPitch( pitch );
		return rot;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotation WithYaw( this Rotation rotation, float pitch )
	{
		var rot = rotation * Rotation.FromYaw( rotation.Yaw() ).Inverse;
		rot *= Rotation.FromYaw( pitch );
		return rot;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Rotation WithRoll( this Rotation rotation, float pitch )
	{
		var rot = rotation * Rotation.FromRoll( rotation.Roll() ).Inverse;
		rot *= Rotation.FromRoll( pitch );
		return rot;
	}

	// Vector3 

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Vector3 Spring( Vector3 from, Vector3 to, float time )
	{
		return new Vector3( Spring( from.x, to.x, time ), Spring( from.y, to.y, time ), Spring( from.z, to.z, time ) );
	}
}

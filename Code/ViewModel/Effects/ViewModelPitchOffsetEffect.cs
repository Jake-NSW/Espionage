﻿using Sandbox;
using Woosh.Common;

namespace Woosh.Espionage;

public sealed class ViewModelPitchOffsetEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float PitchOffset { get; set; }
	public float YawOffset { get; set; }
	public float Damping { get; set; } = 15;

	public ViewModelPitchOffsetEffect( float pitchOffset = 5, float yawOffset = 4 )
	{
		PitchOffset = pitchOffset;
		YawOffset = yawOffset;
	}

	private Rotation m_LastOffsetRot = Rotation.Identity;
	private Vector3 m_LastOffsetPos;

	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		var offset = Camera.Rotation.Pitch().Remap( -90, 90, -1, 1 );
		var rot = setup.Rotation;

		m_LastOffsetRot = Rotation.Slerp( m_LastOffsetRot, Rotation.Lerp( Rotation.From( offset * PitchOffset, 0, 0 ), Rotation.Identity, setup.Hands.Aim ), Damping * Time.Delta );
		m_LastOffsetPos = m_LastOffsetPos.LerpTo( Vector3.Lerp( rot.Up * offset * YawOffset, Vector3.Zero, setup.Hands.Aim ), Damping * Time.Delta );

		setup.Hands.Angles *= m_LastOffsetRot;
		setup.Hands.Offset += m_LastOffsetPos;
	}
}

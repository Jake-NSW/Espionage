﻿using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelRecoilEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect, IMutate<InputContext>
{
	public float Snap { get; }
	public float Return { get; }
	public float Multiplier { get; init; } = 1;
	public float CameraMulti { get; init; } = 4f;

	public ViewModelRecoilEffect( float snap = 30, float returnSpeed = 7 )
	{
		Snap = snap;
		Return = returnSpeed;
	}

	protected override void OnAutoRegister()
	{
		Register<WeaponFired>(
			e =>
			{
				var rand = Game.Random;
				var recoil = e.Data.Recoil;

				m_Target += new Vector3( recoil.x, rand.Float( -recoil.y, recoil.y ), rand.Float( -recoil.z, recoil.z ) ) * Time.Delta;
			}
		);
	}

	private Rotation m_Current = Rotation.Identity;
	private Vector3 m_Target;

	public void OnPostSetup( ref CameraSetup setup )
	{
		var rot = setup.Rotation;

		m_Target = m_Target.LerpTo( Vector3.Zero, Return * Time.Delta );
		m_Current = Rotation.Slerp( m_Current, Rotation.From( m_Target.x, m_Target.y, m_Target.z ), Snap * Time.Delta );

		setup.Hands.Angles *= m_Current * 2f * Multiplier;

		// Add Yaw and Roll Offsets
		setup.Hands.Offset += rot.Left * m_Current.Yaw() / 2;

		// Add Pitch Offsets
		setup.Hands.Offset += (rot.Forward * (m_Current.Pitch()) / 2);

		// Add recoil to view angles
		setup.Rotation *= Rotation.Lerp(
			Rotation.Identity,
			// Inverse pitch, but keep yaw and roll
			m_Current.Inverse * Rotation.FromPitch( m_Current.Pitch() * 2 ),
			CameraMulti
		);
	}

	public void OnPostSetup( ref InputContext setup )
	{
		setup.ViewAngles.pitch += m_Current.Pitch() * 0.004f;
		setup.ViewAngles.yaw += m_Current.Yaw() * 0.006f;
	}
}

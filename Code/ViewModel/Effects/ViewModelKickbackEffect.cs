﻿using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelKickbackEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public float Snap { get; }
	public float Return { get; }

	public ViewModelKickbackEffect( float snap = 25, float returnSpeed = 12 )
	{
		Snap = snap;
		Return = returnSpeed;
	}

	protected override void OnAutoRegister()
	{
		Register<WeaponFired>(
			e =>
			{
				var kick = e.Data.Kickback;
				m_Target += new Vector3( kick.x, kick.y.Range(), kick.z.Range() ) * Time.Delta;
			}
		);
	}

	private Vector3 m_Current;
	private Vector3 m_Target;

	public void OnPostMutate( ref CameraSetup setup )
	{
		var rot = setup.Rotation.WithRoll( 0 );

		m_Target = m_Target.Damp( Vector3.Zero, Return, Time.Delta );
		m_Current = m_Current.Damp( m_Target, Snap, Time.Delta );

		setup.Hands.Offset += (rot.Forward * m_Current.x) + (rot.Left * m_Current.y) + (rot.Down * m_Current.z);
	}
}

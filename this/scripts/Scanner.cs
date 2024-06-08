using Godot;
using System;

namespace Scrunglium
{
	public partial class Scanner : Area3D
	{
		[Export]
		private Timer timer;

		[Export]
		private AudioStreamPlayer3D audio_player;

		[Export]
		public float scan_angle
		{
			get => Mathf.Acos(-_scan_dot);
			set => _scan_dot = -Mathf.Cos(value);
		}
		private float _scan_dot;

		public override void _PhysicsProcess(double delta)
		{
			if (!timer.IsStopped()) return;

			foreach (var iArea in GetOverlappingAreas())
				if (iArea is Barcode iBarcode)
					if (GlobalBasis.Z.Dot(iArea.GlobalBasis.Z) < _scan_dot)
					{
						iBarcode.Scan();
						audio_player.Play();
						timer.Start();
					}
		}
	}
}

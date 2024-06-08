using Godot;
using System;

namespace Scrunglium
{
	public partial class Pickup : RigidBody3D
	{
		public bool is_scanned;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public void Scan()
		{
			is_scanned = true;
		}
	}
}

using Godot;
using System;

namespace Scrunglium
{
	public partial class Barcode : Area3D
	{
		[Export]
		public Pickup owner;

		public void Scan()
		{
			owner.is_scanned = true;
		}
	}
}

using Godot;
using System;

namespace Scrunglium
{
	public partial class Hand : RigidBody3D
	{
		[ExportCategory("Motion")]

		[Export]
		public float linear_sensitivity = 1f;

		[Export]
		public float angular_sensitivity = 1f;

		[Export]
		public float angular_reset_sensitivity = 1f;

		[ExportCategory("Positioning")]

		[Export]
		public Camera3D camera;

		[Export]
		private Area3D area;

		[Export]
		private Plane plane;


		private Vector3 mouse_position_in_world;
		private Vector3 mouse_motion;

		private Generic6DofJoint3D joint;

		private Quaternion _target_rotation;

		private RigidBody3D _target_rigidbody;

		private bool _is_holding_target;
		public bool is_holding_target
		{
			get => _is_holding_target;
			private set
			{
				if (_target_rigidbody == null) return;
				_is_holding_target = value;

				_target_rigidbody.GravityScale = value ? 0f : 1f;

				if (value)
				{
					joint = new Generic6DofJoint3D
					{
						NodeA = GetPath(),
						NodeB = _target_rigidbody.GetPath()
					};
					AddChild(joint);
				}
				else
				{
					joint?.QueueFree();
					joint = null;

					_target_rotation = Quaternion;
				}
			}
		}

		private bool _is_rotating_target;
		public bool is_rotating_target
		{
			get => _is_rotating_target;
			private set
			{
				// if (_targetRigidbody == null) return;
				_is_rotating_target = value;

				if (value)
					_target_rotation = Quaternion.Identity;
				else
					_target_rotation = Quaternion;
			}
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;

			_target_rotation = Quaternion;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{


		}

		public override void _PhysicsProcess(double delta)
		{
			if (!is_holding_target)
			{
				_target_rigidbody = null;
				foreach (var iOverlap in area.GetOverlappingBodies())
				{
					if (iOverlap == this) continue;
					if (iOverlap is RigidBody3D iBody)
					{
						_target_rigidbody = iBody;
						break;
					}
					continue;
				}
			}

			if (Input.IsActionJustPressed("grab"))
				is_holding_target = true;
			else if (Input.IsActionJustReleased("grab"))
				is_holding_target = false;

			if (Input.IsActionJustPressed("rotate"))
				is_rotating_target = true;
			else if (Input.IsActionJustReleased("rotate"))
				is_rotating_target = false;

			if (is_rotating_target)
				_AngularMotionProcess(delta);
			else
				_LinearMotionProcess(delta);

			mouse_motion = default;
		}

		private void _LinearMotionProcess(double delta)
		{
			var input_motion = mouse_motion * linear_sensitivity;
			ApplyForce(input_motion * (float)delta);

			var reset_torque = (_target_rotation * Quaternion.Inverse()).GetEuler() * angular_reset_sensitivity;
			ApplyTorque(reset_torque * (float)delta);
		}

		private void _AngularMotionProcess(double delta)
		{
			var input_motion = new Vector3(-mouse_motion.Y, -mouse_motion.X, 0f) * angular_sensitivity;
			ApplyTorque(input_motion * (float)delta);
		}

		public override void _Input(InputEvent @event)
		{
			if (@event is InputEventMouseMotion @motion)
			{
				mouse_position_in_world = (Vector3)plane.IntersectsRay(
					camera.ProjectRayOrigin(@motion.GlobalPosition),
					camera.ProjectRayNormal(@motion.GlobalPosition)
				);
				mouse_motion = new(@motion.Relative.X, -@motion.Relative.Y, 0f);
			}

			if (Input.IsActionJustPressed("quit"))
				GetTree().Quit();
		}
	}
}

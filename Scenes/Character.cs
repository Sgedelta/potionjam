using Godot;
using System;

public partial class Character : CharacterBody3D
{
	//godot presupplied 3D movement:

	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	
	private bool _controlsEnabled = true;
	
	public bool GetControlsEnabled() {
		return _controlsEnabled;
	}
	
	public void SetControlsEnabled(bool enabled) {
		_controlsEnabled = enabled;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (_controlsEnabled) {
			// Handle Jump.
			if (Input.IsActionJustPressed("movement_jump") && IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}

			// Get the input direction and handle the movement/deceleration.
			Vector2 inputDir = Input.GetVector("movement_left", "movement_right", "movement_up", "movement_down");
			Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			if (direction != Vector3.Zero)
			{
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}

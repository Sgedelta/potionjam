#nullable enable
using Godot;
using System;

public partial class Character : CharacterBody3D
{
	//The NPC which the character will talk to upon pressing the interact key
	//Set by NPCs when the character enters or exits their range
	//Null for no interaction (i.e. no NPCs in range)
	private NPC? _queuedInteraction = null;
	
	//Sets which NPC the player character will talk to when interacting
	public void SetInteraction(NPC interaction) {
		_queuedInteraction = interaction;
	}
	
	//Clears the NPC the player character will talk to when interacting
	public void ClearInteraction() {
		_queuedInteraction = null;
	}
	
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

			//point toward the current camera so that the Transform.Basis is correct
			if(IsInstanceValid(GameManager._instance.CC))
			{
				Transform = Transform.LookingAt(
					Position + //our position, plus
                    (GameManager._instance.CC.CamDesiredOffset * new Vector3(1, 0, -1)) //the camera's current offset from desired position
					);
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

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

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			if (_queuedInteraction == null) {
				velocity.Y = JumpVelocity;
			}
			else {
				//TODO: Disable/enable character control before/after the dialogue runs
				//Can't be done yet since updated character movement isn't merged
				string dialogueTitle = _queuedInteraction.GetDialogueTitle();
				DialogueManager._instance.GetDialogueRunner().StartDialogue(dialogueTitle);
			}
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
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

		Velocity = velocity;
		MoveAndSlide();
	}
}

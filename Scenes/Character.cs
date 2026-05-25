#nullable enable
using Godot;
using System;
using System.Collections.Generic;

public partial class Character : CharacterBody3D
{
	//The NPCs the character is in range of
	//Changed by NPCs when the character enters or exits their range
	//Null for no interaction (i.e. no NPCs in range)
	private List<NPC> _queuedInteractions = new List<NPC>();
	
	//Sets which NPC the player character will talk to when interacting
	public void AddInteraction(NPC interaction) {
		if (!_queuedInteractions.Contains(interaction)) {
			_queuedInteractions.Add(interaction);
		}
	}
	
	//Clears the NPC the player character will talk to when interacting
	public void RemoveInteraction(NPC interaction) {
		_queuedInteractions.Remove(interaction);
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
			if (_queuedInteractions.Count == 0) {
				velocity.Y = JumpVelocity;
			}
			else {
				//Get the closest NPC to the player out of those stored in _queuedInteractions
				NPC closestInteraction = _queuedInteractions[0];
				float closestDistance = float.MaxValue;
				foreach (NPC interaction in _queuedInteractions) {
					float distance = (interaction.GlobalPosition - this.GlobalPosition).Length();
					if (distance < closestDistance) {
						closestInteraction = interaction;
						closestDistance = distance;
					}
				}
				//TODO: Disable/enable character control before/after the dialogue runs
				//Can't be done yet since updated character movement isn't merged
				string dialogueTitle = closestInteraction.GetDialogueTitle();
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

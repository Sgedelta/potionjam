#nullable enable
using Godot;
using System;
using System.Collections.Generic;
using YarnSpinnerGodot;

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
					SetControlsEnabled(false);
					string dialogueTitle = closestInteraction.GetDialogueTitle();
					
					/* Test case for quests
					Potion testPotion = new Potion();
					Ingredient testIngredient = GD.Load<Ingredient>("res://Resources/Ingredients/QuestTestIngredient.tres");
					PotionStep testStep1 = new PotionStep();
					testStep1.Type = testIngredient;
					testStep1.Stage = IngredientStage.BASE;
					testPotion.AddStep(testStep1);
					PotionStep testStep2 = new PotionStep();
					testStep2.Type = testIngredient;
					testStep2.Stage = IngredientStage.MODIFIER;
					testPotion.AddStep(testStep2);
					testPotion.GetPotionValues(); //necessary (at least in this test case) to properly compute flavor
					
					closestInteraction.GivePotion(testPotion); //This line gives the potion to the NPC so that the dialogue system can consider the potion.
					*/
					
  					DialogueRunner dr = DialogueManager._instance.GetDialogueRunner();
					dr.StartDialogue(dialogueTitle);
  					dr.onDialogueComplete += () => {SetControlsEnabled(true);}; //Note, might need to "remove" this from the dr or it'll slowly stack up..? Not critical atm, but to think about
				}
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

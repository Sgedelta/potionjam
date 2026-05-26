using Godot;
using Godot.Collections;
using System;
using YarnSpinnerGodot;

public partial class NPC : Node3D
{
	//The title in the Yarn script to start this NPC's dialogue from.
	[Export]
	private string _dialogueTitle = "";
	
	//The quest associated with this NPC. Null for no quest.
	[Export]
	private Quest _quest = null;
	
	//The potion given to the NPC (if any), used to verify quest completion.
	private Potion _givenPotion = null;
	
	//Get the Yarn dialogue title this NPC will run when interacted with.
	public string GetDialogueTitle() {
		return _dialogueTitle;
	}
	
	//Returns whether the NPC's current potion satisfies its quest.
	//Will push a warning and return false if the NPC has no potion or no quest - use HasPotion() and HasQuest() if needed.
	public bool CheckQuest() {
		if (!HasPotion()) {
			GD.PushWarning("Warning: CheckQuest() was called on an NPC with no potion!");
			return false;
		}
		if (!HasQuest()) {
			GD.PushWarning("Warning: CheckQuest() was called on an NPC with no quest!");
			return false;
		}
		return _quest.Check(_givenPotion);
	}
	
	//Returns whether this NPC has been given a potion.
	public bool HasPotion() {
		return (_givenPotion != null);
	}
	
	//Returns whether this NPC has a quest that it gives.
	public bool HasQuest() {
		return (_quest != null);
	}
	
	//Gives the NPC a potion, which will be checked in dialogue to see if it's what the NPC wants.
	//Call this before running the NPC's dialogue.
	public void GivePotion(Potion potion) {
		_givenPotion = potion;
	}
	
	//Clears the potion this NPC considers when checking its quest completion.
	public void ClearPotion() {
		_givenPotion = null;
	}
	
	
	public void OnRangeEntered(Node3D other) {
		if (other is Character) {
			((Character)other).AddInteraction(this);
		}
	}
	
	public void OnRangeExited(Node3D other) {
		if (other is Character) {
			((Character)other).RemoveInteraction(this);
		}
	}
	
	public override void _Ready() {
		DialogueRunner dr = DialogueManager._instance.GetDialogueRunner();
		if (HasQuest()) {
			dr.AddFunction("CheckQuest_" + _quest.Name, CheckQuest);
		}
	}
}

using Godot;
using System;
using YarnSpinnerGodot;

public partial class NPC : Node3D
{
	[Export]
	private string _dialogueTitle = "";
	
	//Get the Yarn dialogue title this NPC will run when interacted with.
	public string GetDialogueTitle() {
		return _dialogueTitle;
	}
	
	public void OnRangeEntered(Node3D other) {
		if (other is Character) {
			((Character)other).SetInteraction(this);
		}
	}
	
	public void OnRangeExited(Node3D other) {
		if (other is Character) {
			((Character)other).ClearInteraction();
		}
	}
}

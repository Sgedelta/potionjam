using Godot;
using System;
using YarnSpinnerGodot;

public partial class DialogueManager : Node
{
	//Singleton setup
	public static DialogueManager _instance;
	public DialogueManager GDInstance { get { return _instance; } }
	
	//Ensure only one DialogueManager exists
	public override void _EnterTree()
	{
		if(!IsInstanceValid(DialogueManager._instance))
		{
			_instance = this;
		}
		else
		{
			Free();
		}
	}
	
	public DialogueRunner GetDialogueRunner() {
		return GetNode<DialogueRunner>("DialogueRunner");
	}
}

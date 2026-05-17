using Godot;
using System;

public partial class GameManager : Node
{
	//singleton
	public static GameManager _instance; //could do a true private and public accessor, but I don't think needed, at least for now. Don't overwrite ig, lol - Sam
	public GameManager GDInstance { get { return _instance; } } //gdscript cannot access static variables, but Game Manager is global so it's alright

	public override void _EnterTree()
	{
		if(!IsInstanceValid(GameManager._instance))
		{
			//other instance does not exist or has been deleted
			_instance = this;

		}
		else
		{
			//another game manager exists, so we are deleting this one
			Free();
		}
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

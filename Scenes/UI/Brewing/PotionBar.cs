using Godot;
using System;

public partial class PotionBar : HBoxContainer
{
	private const int PillSize = 60;

	public void AddPotion()
	{
		var pillScene = GD.Load<PackedScene>("res://Scenes/UI/Brewing/PotionPill.tscn");
		var pill = pillScene.Instantiate<Panel>();

		//give each potion a diff texture??
		//pill.Texture = flaskTexture;

		AddChild(pill);
	}
}

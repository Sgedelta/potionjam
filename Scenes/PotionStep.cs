using Godot;
using System;

public partial class PotionStep : RefCounted //NOT a node!
{
	[Export] public Ingredient Type;
	public IngredientStage Stage = IngredientStage.BASE;


}


public enum IngredientStage
{
	BASE,
	MODIFIER,
	METAMODIFIER
}

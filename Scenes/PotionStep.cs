using Godot;
using System;

[GlobalClass]
public partial class PotionStep : RefCounted //NOT a node!
{
	[Export] public Ingredient Type;
	public IngredientStage Stage = IngredientStage.BASE;

	public PotionStep()
	{

	}

	public PotionStep(Ingredient ingredient, IngredientStage stage)
	{
		Type = ingredient;
		Stage = stage;
	}

}


public enum IngredientStage
{
	BASE,
	MODIFIER,
	METAMODIFIER
}

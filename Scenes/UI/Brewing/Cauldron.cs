using Godot;

public partial class Cauldron : Panel
{
	private CauldronUI _parent;

	public override void _Ready() 
	{
		_parent = GetParent<CauldronUI>();
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		return ModZone.TryParse(data, out _, out _) ||
			(data.As<GodotObject>() is IngredientSlot s && s.IngredientData != null);
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		GD.Print($"arr count: {data.AsGodotArray().Count}");
		Ingredient ingredient;
		IngredientStage stage;
		
		if (ModZone.TryParse(data, out ingredient, out stage)) {}
		else if (data.As<GodotObject>() is IngredientSlot slot && slot.IngredientData != null)
		{
			ingredient = slot.IngredientData;
			stage = slot.CurrentStage;
		}
		else return;
		
		if (ModZone.TryGetSourceZone(data, out var source)) source.Clear();
		
		_parent.OnCauldronDrop(ingredient, stage);
	}
}

using Godot;

public partial class ToolsPanel : VBoxContainer
{
	[Signal] public delegate void StageAssignedEventHandler(Ingredient ingredient, int newStage);

	private ModZone _modZone;
	private ModZone _metaModZone;

	public override void _Ready()
	{
		_modZone = GetNode<ModZone>("ModZone");
		_metaModZone = GetNode<ModZone>("MetaModZone");

		_modZone.ZoneStage = IngredientStage.MODIFIER;
		_metaModZone.ZoneStage = IngredientStage.METAMODIFIER;

		_modZone.IngredientDropped += (ing, stage) => EmitSignal(SignalName.StageAssigned, ing, stage);
		_metaModZone.IngredientDropped += (ing, stage) => EmitSignal(SignalName.StageAssigned, ing, stage);
	}
}

using Godot;

public partial class ToolsPanel : VBoxContainer
{
	private ModZone _modZone;
	private ModZone _metaModZone;

	public override void _Ready()
	{
		_modZone = GetNode<ModZone>("ModZone");
		_metaModZone = GetNode<ModZone>("MetaModZone");

		_modZone.ZoneStage = IngredientStage.MODIFIER;
		_metaModZone.ZoneStage = IngredientStage.METAMODIFIER;
	}
}

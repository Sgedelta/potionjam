using Godot;

public partial class ModZone : Panel
{
	[Signal] public delegate void IngredientDroppedEventHandler(Ingredient ingredient, int newStage);

	[Export] public IngredientStage ZoneStage = IngredientStage.MODIFIER;

	private Label _label;

	private StyleBoxFlat _normalStyle;
	private StyleBoxFlat _highlightStyle;

	public override void _Ready()
	{
		_label = GetNode<Label>("Label");
		_label.Text = ZoneStage == IngredientStage.MODIFIER ? "Modifier" : "Meta-modifier";

		// had to do in code fsr
		_normalStyle = new StyleBoxFlat();
		_normalStyle.BgColor           = new Color(0.18f, 0.18f, 0.22f);
		_normalStyle.BorderColor        = new Color(0.45f, 0.45f, 0.55f);
		_normalStyle.SetBorderWidthAll(2);
		_normalStyle.SetCornerRadiusAll(6);

		_highlightStyle = (StyleBoxFlat)_normalStyle.Duplicate();
		_highlightStyle.BgColor    = new Color(0.25f, 0.35f, 0.55f);
		_highlightStyle.BorderColor = new Color(0.5f, 0.75f, 1f);

		AddThemeStyleboxOverride("panel", _normalStyle);
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		bool can = data.As<GodotObject>() is IngredientSlot slot && slot.IngredientData != null;
		AddThemeStyleboxOverride("panel", can ? _highlightStyle : _normalStyle);
		return can;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		AddThemeStyleboxOverride("panel", _normalStyle);
		if (data.As<GodotObject>() is not IngredientSlot slot) return;
		if (slot.IngredientData == null) return;
		EmitSignal(SignalName.IngredientDropped, slot.IngredientData, (int)ZoneStage);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationDragEnd)
			AddThemeStyleboxOverride("panel", _normalStyle);
	}
}

using Godot;

public partial class ModZone : Panel
{
	[Signal] public delegate void IngredientDroppedEventHandler(Ingredient ingredient, int newStage);
	[Export] public IngredientStage ZoneStage = IngredientStage.MODIFIER;

	private Label _label;
	private StyleBoxFlat _normalStyle;
	private StyleBoxFlat _highlightStyle;
	private StyleBoxFlat _rejectStyle;
	private Tween _rejectTween;
	private bool _wasHovered = false;
	private IngredientSlot _heldSlot;

	public override void _Ready()
	{
		_label = GetNode<Label>("Label");
		_label.Text = ZoneStage == IngredientStage.MODIFIER ? "Modifier" : "Meta-modifier";
		
		_heldSlot = GetNode<IngredientSlot>("IngredientSlot");
		_heldSlot.Visible = false;
		_heldSlot.MouseFilter = MouseFilterEnum.Ignore;

		_normalStyle = new StyleBoxFlat();
		_normalStyle.BgColor           = new Color(0.18f, 0.18f, 0.22f);
		_normalStyle.BorderColor        = new Color(0.45f, 0.45f, 0.55f);
		_normalStyle.SetBorderWidthAll(2);
		_normalStyle.SetCornerRadiusAll(6);

		_highlightStyle = (StyleBoxFlat)_normalStyle.Duplicate();
		_highlightStyle.BgColor    = new Color(0.25f, 0.35f, 0.55f);
		_highlightStyle.BorderColor = new Color(0.5f, 0.75f, 1f);
		
		_rejectStyle = (StyleBoxFlat)_normalStyle.Duplicate();
		_rejectStyle.BgColor    = new Color(0.35f, 0.12f, 0.12f);
		_rejectStyle.BorderColor = new Color(1f, 0.25f, 0.25f);

		AddThemeStyleboxOverride("panel", _normalStyle);
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		if(_heldSlot.Visible) 
		{
			_wasHovered = false;
			return false;
		}
		bool can = TryParse(data, out _, out _) ||
			(data.As<GodotObject>() is IngredientSlot slot && slot.IngredientData != null) && IsValidForZone(slot);;
		_wasHovered = can;
		AddThemeStyleboxOverride("panel", can ? _highlightStyle : _normalStyle);
		return can;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		AddThemeStyleboxOverride("panel", _normalStyle);
		Ingredient ingredient;
		if (TryParse(data, out ingredient, out _)) {}
		else if (data.As<GodotObject>() is IngredientSlot slot && slot.IngredientData != null)
			ingredient = slot.IngredientData;
		else return;
		
		if (TryGetSourceZone(data, out var source)) source.Clear();
		
		SetHeldIngredient(ingredient);
		EmitSignal(SignalName.IngredientDropped, ingredient, (int)ZoneStage);
	}

	public override void _Notification(int what)
	{
		if (what != NotificationDragEnd) return;
		
		_heldSlot.Modulate = Colors.White;
		_wasHovered = false;
		AddThemeStyleboxOverride("panel", _normalStyle);
	}
	
	private void SetHeldIngredient(Ingredient ingredient)
	{
		_heldSlot.IngredientData = ingredient;
		_heldSlot.CurrentStage = ZoneStage;
		_heldSlot.Visible = true;
		_heldSlot.MouseFilter = MouseFilterEnum.Ignore;
		_heldSlot.Refresh();
	}
	
	public override Variant _GetDragData(Vector2 atPosition)
	{
		if (!_heldSlot.Visible || _heldSlot.IngredientData == null) return default;
		
		_heldSlot.Modulate = new Color(1,1,1,0.3f);
		var preview = new TextureRect
		{
			Texture = _heldSlot.GetNode<TextureRect>("Icon").Texture,
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			CustomMinimumSize = new Vector2(48, 48),
		};
		SetDragPreview(preview);
		return new Godot.Collections.Array{_heldSlot.IngredientData, (int)_heldSlot.CurrentStage, this};
	}

	public void Clear()
	{
		_heldSlot.Visible = false;
		_heldSlot.Modulate = Colors.White;
		_heldSlot.IngredientData = null;
		_heldSlot.MouseFilter = MouseFilterEnum.Ignore;
	}
	
	private void FlashReject()
	{
		_rejectTween?.Kill();
		AddThemeStyleboxOverride("panel", _rejectStyle);
		_rejectTween = CreateTween();
		_rejectTween.TweenInterval(0.3f);
		_rejectTween.TweenCallback(Callable.From(() =>
			AddThemeStyleboxOverride("panel", _normalStyle)));
	}
	
	public static bool TryParse(Variant data, out Ingredient ingredient, out IngredientStage stage)
	{
		ingredient = null;
		stage = IngredientStage.BASE;
		if (data.VariantType != Variant.Type.Array) return false;
		var arr = data.AsGodotArray();
		if (arr.Count < 2) return false;
		ingredient = arr[0].As<Ingredient>();
		stage = (IngredientStage)(int)arr[1];
		return ingredient != null;
	}
	
	// always true for now
	private bool IsValidForZone(IngredientSlot slot)
	{
		return true;
	}
	
	public static bool TryGetSourceZone(Variant data, out ModZone zone)
	{
		zone = null;
		if (data.VariantType != Variant.Type.Array) return false;
		var arr = data.AsGodotArray();
		if (arr.Count < 3) return false;
		zone = arr[2].As<ModZone>();
		return zone != null;
	}
}

using Godot;

public partial class IngredientSlot : Panel
{
	public Ingredient IngredientData;
	public IngredientStage CurrentStage = IngredientStage.BASE;

	private TextureRect _icon;
	private Label _stageLabel;

	private static StyleBoxFlat _emptyStyle;

	public override void _Ready()
	{
		_icon = GetNode<TextureRect>("Icon");
		_stageLabel = GetNodeOrNull<Label>("StageLabel");

		if (_emptyStyle == null)
		{
			_emptyStyle = new StyleBoxFlat();
			_emptyStyle.BgColor = new Color(0.12f, 0.12f, 0.15f);
			_emptyStyle.SetBorderWidthAll(1);
			_emptyStyle.SetCornerRadiusAll(4);
			_emptyStyle.BorderColor = new Color(0.3f, 0.3f, 0.35f);
		}

		Refresh();
	}

	public void Refresh()
	{
		if (_icon == null) return;

		_icon.Texture = CurrentStage switch
		{
			IngredientStage.BASE => IngredientData?.BaseSprite,
			IngredientStage.MODIFIER => IngredientData?.ModSprite,
			IngredientStage.METAMODIFIER => IngredientData?.MetaModSprite,
			_ => null,
		};

		if (_stageLabel != null)
		{
			_stageLabel.Text = CurrentStage switch
			{
				IngredientStage.MODIFIER => "MOD",
				IngredientStage.METAMODIFIER => "META",
				_ => "",
			};
		}

		var style = new StyleBoxFlat();
		style.BgColor = new Color(0.15f, 0.15f, 0.18f);
		style.SetBorderWidthAll(2);
		style.SetCornerRadiusAll(4);
		style.BorderColor = CurrentStage switch
		{
			IngredientStage.BASE => new Color(0.4f, 0.8f, 0.4f),
			IngredientStage.MODIFIER => new Color(0.4f, 0.6f, 1.0f),
			IngredientStage.METAMODIFIER => new Color(1.0f, 0.5f, 0.2f),
			_ => new Color(0.3f, 0.3f, 0.3f),
		};
		AddThemeStyleboxOverride("panel", style);
	}

	public void SetEmpty(bool empty)
	{
		if (_icon != null)
			_icon.Visible = !empty;

		if (_stageLabel != null)
			_stageLabel.Visible = !empty;

		if (empty)
			AddThemeStyleboxOverride("panel", _emptyStyle);
		else
			Refresh();
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		if (IngredientData == null) return default;
		if (_icon != null && !_icon.Visible) return default; // in cauldron, can't drag

		var preview = new TextureRect
		{
			Texture = _icon?.Texture,
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			CustomMinimumSize = new Vector2(48, 48),
		};
		SetDragPreview(preview);
		return this;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data) => false;
}

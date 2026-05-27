using Godot;

public partial class CauldronUI : Panel
{
	[Signal] public delegate void IngredientAddedEventHandler(Ingredient ingredient, int stage);
	[Signal] public delegate void FlushedEventHandler();

	private Panel _cauldron;
	private HBoxContainer _strip;
	private Label _hoverLabel;
	private Button _flushButton;

	private const int IconSize = 52;

	private readonly System.Collections.Generic.List<(Ingredient ingredient, IngredientStage stage)> _steps = new();

	public override void _Ready()
	{
		_cauldron  = GetNode<Panel>("Cauldron");
		_strip = GetNode<HBoxContainer>("IngredientStrip");
		_hoverLabel = GetNode<Label>("HoverLabel");
		_flushButton = GetNode<Button>("FlushButton");

		_hoverLabel.Visible = false;

		_flushButton.Pressed += OnFlushPressed;

		_cauldron.MouseEntered += () => _hoverLabel.Visible = _steps.Count > 0;
		_cauldron.MouseExited += () => _hoverLabel.Visible = false;
	}

	public void OnCauldronDrop(Ingredient ingredient, IngredientStage stage)
	{
		if (ingredient == null) return;

		_steps.Add((ingredient, stage));
		RebuildStrip();
		EmitSignal(SignalName.IngredientAdded, ingredient, (int)stage);
	}

	public void SetHoverText(string text) => _hoverLabel.Text = text;

	public void ClearSilent()
	{
		_steps.Clear();
		RebuildStrip();
		_hoverLabel.Visible = false;
	}

	public System.Collections.Generic.List<(Ingredient, IngredientStage)> GetSteps() => _steps;

	private void RebuildStrip()
	{
		foreach (Node child in _strip.GetChildren())
			child.QueueFree();

		foreach (var (ingredient, stage) in _steps)
		{
			Texture2D tex = stage switch
			{
				IngredientStage.BASE => ingredient.BaseSprite,
				IngredientStage.MODIFIER => ingredient.ModSprite,
				IngredientStage.METAMODIFIER => ingredient.MetaModSprite,
				_ => ingredient.BaseSprite,
			};

			var wrapper = new VBoxContainer();
			wrapper.CustomMinimumSize = new Vector2(IconSize, IconSize + 14);

			var icon = new TextureRect
			{
				Texture = tex,
				ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
				StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
				CustomMinimumSize = new Vector2(IconSize, IconSize),
			};
			wrapper.AddChild(icon);

			_strip.AddChild(wrapper);
		}
	}

	private void OnFlushPressed()
	{
		_steps.Clear();
		RebuildStrip();
		_hoverLabel.Visible = false;
		EmitSignal(SignalName.Flushed);
	}
}

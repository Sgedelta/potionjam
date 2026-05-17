using Godot;

public partial class BackpackGrid : VBoxContainer
{
	[Export] public PackedScene IngredientSlotScene;
	[Export] public int GridSize = 12;

	private GridContainer _grid;

	private readonly System.Collections.Generic.Dictionary<Ingredient, IngredientSlot> _slotMap = new();

	private StyleBoxFlat _emptyStyle; // prob a bettter way to do this lol
	private StyleBoxFlat _occupiedStyle;

	public override void _Ready()
	{
		_grid = GetNode<GridContainer>("BackpackGrid");
		_grid.Columns = 3;

		_emptyStyle = new StyleBoxFlat();
		_emptyStyle.BgColor = new Color(0.12f, 0.12f, 0.15f);
		_emptyStyle.SetBorderWidthAll(1);
		_emptyStyle.SetCornerRadiusAll(4);
		_emptyStyle.BorderColor = new Color(0.3f, 0.3f, 0.35f);

		SpawnPlaceholders();
	}

	private void SpawnPlaceholders()
	{
		for (int i = 0; i < GridSize; i++)
		{
			var p = new Panel();
			p.CustomMinimumSize = new Vector2(72, 72);
			p.AddThemeStyleboxOverride("panel", _emptyStyle);
			_grid.AddChild(p);
		}
	}

	public void AddIngredient(Ingredient ingredient, IngredientStage stage = IngredientStage.BASE)
	{
		if (_slotMap.TryGetValue(ingredient, out var existing))
		{
			existing.CurrentStage = stage;
			existing.Refresh();
			return;
		}
		
		int index = _slotMap.Count;

		var slot = IngredientSlotScene.Instantiate<IngredientSlot>();
		slot.IngredientData    = ingredient;
		slot.CurrentStage      = stage;
		slot.CustomMinimumSize = new Vector2(72, 72);

		var placeholder = _grid.GetChild(index);
		_grid.AddChild(slot);
		_grid.MoveChild(slot, index);
		placeholder.QueueFree();

		slot.Refresh();
		_slotMap[ingredient] = slot;
	}

	public void HideIngredient(Ingredient ingredient)
	{
		if (_slotMap.TryGetValue(ingredient, out var slot))
		{
			slot.SetEmpty(true);
		}
	}

	public void ShowIngredient(Ingredient ingredient, IngredientStage stage)
	{
		if (_slotMap.TryGetValue(ingredient, out var slot))
		{
			slot.CurrentStage = stage;
			slot.SetEmpty(false);
			slot.Refresh();
		}
	}

	public IngredientSlot GetSlot(Ingredient ingredient)
	{
		_slotMap.TryGetValue(ingredient, out var slot);
		return slot;
	}
}

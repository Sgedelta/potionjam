using Godot;
using Godot.Collections;

public partial class Brewing : Control
{
	[Export] public Array<Ingredient> AllIngredients = new Array<Ingredient>();

	private PotionBar   _potionBar;
	private BackpackGrid _backpack;
	private CauldronUI  _cauldron;
	private ToolsPanel  _tools;
	private ConfirmPotion _confirmButton;

	private Potion _currentPotion; // to see potion effects + check validity

	public override void _Ready()
	{
		_potionBar = GetNode<PotionBar> ("VBox/PotionBar");
		_backpack = GetNode<BackpackGrid> ("VBox/BrewingArea/Backpack");
		_cauldron = GetNode<CauldronUI> ("VBox/BrewingArea/CauldronUI");
		_tools = GetNode<ToolsPanel> ("VBox/BrewingArea/Tools");
		_confirmButton = GetNode<ConfirmPotion> ("VBox/ConfirmPotion");

		_currentPotion = new Potion();
		AddChild(_currentPotion);

		foreach (var ingredient in AllIngredients)
			_backpack.AddIngredient(ingredient, IngredientStage.BASE); // add unique ingreds to bag

		//signals
		_cauldron.IngredientAdded += OnIngredientAddedToCauldron;
		_cauldron.Flushed += OnCauldronFlushed;
		_tools.StageAssigned += OnStageAssigned;
		_confirmButton.Pressed += OnConfirmPressed;

		RefreshConfirmButton();
	}

	private void OnIngredientAddedToCauldron(Ingredient ingredient, int stageInt)
	{
		var stage = (IngredientStage)stageInt;

		_backpack.HideIngredient(ingredient); //not sure if we want this?

		var step = new PotionStep { Type = ingredient, Stage = stage };
		_currentPotion.AddStep(step);

		UpdateHoverLabel();
		RefreshConfirmButton();
	}

	private void OnCauldronFlushed()
	{
		//because ingredients are unlimited, we no longer need to bring ingredients back to the backpack

		_currentPotion.FlushPotion();
		UpdateHoverLabel();
		RefreshConfirmButton();
	}

	// changing ingred to mod / metamod
	private void OnStageAssigned(Ingredient ingredient, int newStageInt)
	{
		var newStage = (IngredientStage)newStageInt;

		var slot = _backpack.GetSlot(ingredient);
		if (slot == null) //replaces slot - do we want this?
			_backpack.AddIngredient(ingredient, newStage);
		else
			_backpack.ShowIngredient(ingredient, newStage);
	}

	private void OnConfirmPressed()
	{
		_currentPotion.GetPotionValues();

		if (_currentPotion.Validity != Potion.PotionValidity.VALID)
		{
			//show error somehow?
			return;
		}

		_potionBar.AddPotion();

		_currentPotion.FlushPotion();
		_cauldron.ClearSilent();

		foreach (var ingredient in AllIngredients)
			_backpack.ShowIngredient(ingredient, IngredientStage.BASE);

		UpdateHoverLabel();
		RefreshConfirmButton();
	}

	private void UpdateHoverLabel()
	{
		var vals = _currentPotion.GetPotionValues();

		if (vals == null || vals.Count == 0 || _currentPotion.GetStepCount() == 0)
		{
			_cauldron.SetHoverText("Cauldron is empty");
			return;
		}

		var sb = new System.Text.StringBuilder();
		sb.AppendLine("Potion Effects");
		foreach (var kv in vals)
		{
			var unit = kv.Value;
			if (unit.Value != 0f || unit.Clarity != 0f)
				sb.AppendLine($"{kv.Key}: {unit.Value:+0.##;-0.##;0}  (clarity {unit.Clarity:+0.##;-0.##;0})");
		}
		if (_currentPotion.Validity != Potion.PotionValidity.VALID)
			sb.AppendLine($"\n⚠ {_currentPotion.Validity}");

		_cauldron.SetHoverText(sb.ToString());
	}

	private void RefreshConfirmButton()
	{
		_currentPotion.GetPotionValues();
		bool ready = _currentPotion.Validity == Potion.PotionValidity.VALID;
		_confirmButton.Disabled = !ready;
		_confirmButton.Modulate = ready ? Colors.White : new Color(0.6f, 0.6f, 0.6f); //only need rn bc disabled texture is same as normal
	}
}

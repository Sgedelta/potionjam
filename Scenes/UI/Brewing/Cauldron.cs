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
		bool can = data.As<GodotObject>() is IngredientSlot s && s.IngredientData != null;
		return can;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		if (data.As<GodotObject>() is IngredientSlot slot)
			_parent.OnCauldronDrop(slot);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationDragEnd)
		{
			//something
		}
	}
}

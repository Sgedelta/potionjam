using Godot;

public partial class ConfirmPotion : TextureButton
{
	private Tween _pulseTween;
	private Control _overlay;

	public override void _Ready()
	{
		_overlay = GetNode<Control>("TooltipDetector");
	}
	
	public void SetReady(bool ready)
	{
		Disabled = !ready;
		Modulate = ready ? Colors.White : new Color(0.5f, 0.5f, 0.5f);
		_overlay.TooltipText = ready ? "" : "Potion is not valid";

		_pulseTween?.Kill();

		if (ready)
		{
			_pulseTween = CreateTween().SetLoops();
			_pulseTween.TweenProperty(this, "scale", new Vector2(1.06f, 1.06f), 0.5f)
					   .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
			_pulseTween.TweenProperty(this, "scale", Vector2.One, 0.5f)
					   .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		}
		else
		{
			Scale = Vector2.One;
		}
	}
}

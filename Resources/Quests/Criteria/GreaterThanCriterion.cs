using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GreaterThanCriterion : BaseCriterion
{
	// The minimum value this potion stat must have.
	[Export]
	public float MinValue = 0;
	
	// Whether a stat value exactly equal to MinValue will be accepted.
	[Export]
	public bool MinInclusive = true;
	
	public override bool Check(Potion potion) {
		float val;
		
		if (StatToCheck == PotionStats.Flavor) {
			val = potion.Flavor;
		}
		else {
			Dictionary<PotionStats, StatUnit> stats = potion.GetPotionValues();
			StatUnit stat = stats[StatToCheck];
			if (CheckClarity) {
				val = stat.Clarity;
			}
			else {
				val = stat.Value;
			}
		}
		
		if (MinInclusive) {
			return (val >= MinValue);
		}
		else {
			return (val > MinValue);
		}
	}
}

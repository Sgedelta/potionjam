using Godot;
using Godot.Collections;

[GlobalClass]
public partial class LessThanCriterion : BaseCriterion
{
	//The maximum value this potion stat must have.
	[Export]
	public float MaxValue = 0;
	
	//Whether a stat value exactly equal to MaxValue will be accepted.
	[Export]
	public bool MaxInclusive = true;
	
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
		
		if (MaxInclusive) {
			return (val <= MaxValue);
		}
		else {
			return (val < MaxValue);
		}
	}
}

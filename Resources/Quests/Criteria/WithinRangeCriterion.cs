using Godot;
using Godot.Collections;

[GlobalClass]
public partial class WithinRangeCriterion : BaseCriterion
{
	//The minimum value this potion stat must have.
	[Export]
	public float MinValue = 0;
	
	//The maximum value this potion stat must have.
	[Export]
	public float MaxValue = 0;
	
	//Whether a stat value exactly equal to MinValue will be accepted.
	[Export]
	public bool MinInclusive = true;
	
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
		
		bool minCheck;
		bool maxCheck;
		if (MinInclusive) {
			minCheck = (val >= MinValue);
		}
		else {
			minCheck = (val > MinValue);
		}
		if (MaxInclusive) {
			maxCheck = (val <= MaxValue);
		}
		else {
			maxCheck = (val < MaxValue);
		}
		
		return (minCheck && maxCheck);
	}
}

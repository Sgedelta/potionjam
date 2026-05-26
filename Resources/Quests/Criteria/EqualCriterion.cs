using Godot;
using Godot.Collections;

[GlobalClass]
public partial class EqualCriterion : BaseCriterion
{
	//The exact value this potion stat must have.
	[Export]
	public float EqualValue = 0;
	
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
		
		return (val == EqualValue);
	}
}

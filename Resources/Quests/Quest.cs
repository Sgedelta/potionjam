using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Quest : Resource
{
	//The name of this quest.
	//Used internally in Yarn scripts to check for completion.
	[Export]
	public string Name = "";
	
	//The potion criteria required to complete this quest.
	//All of these must be true of a potion for the quest to count as completed.
	[Export]
	public Array<BaseCriterion> Criteria = new Array<BaseCriterion>();
	
	[Export]
	//If true, all potion stats that do NOT have a criterion checking for them must be 0 for the quest to be completed.
	//Useful for "potion must have no other effects" conditions, without having to check for 0 in each individual stat.
	//Only checks stat value, not clarity, and does not check flavor.
	public bool UncheckedMustBeZero = false;
	
	//Check whether a given potion passes all quest criteria (and satisifies UncheckedMustBeZero, if needed).
	//A result of true indicates that the potion successfully completes the quest.
	public bool Check(Potion potion) {
		Dictionary<PotionStats, bool> checkedStats = new Dictionary<PotionStats, bool>();
		foreach (PotionStats stat in Enum.GetValues(typeof(PotionStats))) {
			checkedStats[stat] = false;
		}
		
		foreach (BaseCriterion criterion in Criteria) {
			if (criterion.Check(potion) == false) {
				return false;
			}
			checkedStats[criterion.StatToCheck] = true;
		}
		
		if (UncheckedMustBeZero) {
			Dictionary<PotionStats, StatUnit> stats = potion.GetPotionValues();
			foreach (var entry in checkedStats) {
				if (entry.Key == PotionStats.Flavor) { continue; }
				if (entry.Value == false && stats[entry.Key].Value != 0) {
					return false;
				}
			}
		}
		
		return true;
	}
}

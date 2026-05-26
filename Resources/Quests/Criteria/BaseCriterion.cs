using Godot;
using Godot.Collections;

//Base class for all potion stat criteria.
//Criteria are used by NPC quests to check whether a potion fits the requirements.
[GlobalClass]
public abstract partial class BaseCriterion : Resource
{
	//Which potion stat this Criterion checks the value or clarity of.
	[Export]
	public PotionStats StatToCheck = PotionStats.Flavor;
	
	//If true, check the potion stat's clarity; if false, check its base value.
	//Ignored if StatToCheck is flavor, since flavor doesn't have clarity.
	[Export]
	public bool CheckClarity = false;
	
	//Check the given potion against this criterion.
	public abstract bool Check(Potion potion);
}

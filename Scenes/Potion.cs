using Godot;
using Godot.Collections;
using System;

public partial class Potion : Node3D
{
	public enum PotionValidity
	{
		VALID,
		NOT_ENOUGH_INGREDIENTS,
		NO_FIRST_BASE,
		TOO_MANY_INGREDIENTS
	}

	public Array<PotionStep> Steps = new Array<PotionStep>();
	private bool _isDirty = true;
	public PotionValidity Validity;
	private Dictionary<PotionStats, StatUnit> _lastPotionVals;

	public Dictionary<PotionStats, StatUnit> GetPotionValues()
	{
		//return old info if nothing has changed
		if(!_isDirty)
		{
			return _lastPotionVals;
		}

		//otherwise we need to construct a new one
		if(_lastPotionVals == null)
		{
			_lastPotionVals = new Dictionary<PotionStats, StatUnit>();
		} 
		else
		{
			_lastPotionVals.Clear();
		}

		//check basic validity
		if(Steps.Count <= 0)
		{
			Validity = PotionValidity.NOT_ENOUGH_INGREDIENTS;
			return _lastPotionVals;
		} 
		else if (Steps[0].Stage != IngredientStage.BASE) {
			Validity = PotionValidity.NO_FIRST_BASE;
			return _lastPotionVals;
		}

		//fill in with a bunch of empty data
		foreach(PotionStats stat in Enum.GetValues(typeof(PotionStats)))
		{
			_lastPotionVals.Add(stat, new StatUnit(stat));
		}

		//System.Collections.Generic.List<Tuple<float, int, int>> ActiveModifiers = new();
		Array<ModifierUnit> activeMods = new Array<ModifierUnit>();
		Array<ModifierUnit> activeMetaMods = new Array<ModifierUnit>();

		//loop through all steps, applying from the most recent to the oldest. this is so modifiers apply properly
		for(int i = Steps.Count-1; i >= 0; i--)
		{
			PotionStep step = Steps[i];
			float val = 0; //used in many cases
			float clarityChange = 0;

			switch(step.Stage)
			{
				case IngredientStage.BASE:
					//base ingredients are the simplest. Modify the stats by a given amount, taking into account any active modifiers
					foreach(PotionStats stat in step.Type.BaseStateChange.Keys)
					{
						//get base
						val = step.Type.BaseStateChange[stat];

						//modify
						foreach (ModifierUnit mod in activeMods)
						{
							val = ApplyModifierToValue(val, out clarityChange, mod);
						}

						//apply
						_lastPotionVals[stat].Value += val;
						_lastPotionVals[stat].Clarity += clarityChange;

					}
					//now, reduce mods
					for(int modIndex = 0; modIndex < activeMods.Count; modIndex++)
					{
						activeMods[modIndex].TurnsLeft -= 1;
						if (activeMods[modIndex].TurnsLeft == 0)
						{
							activeMods.RemoveAt(modIndex);
							modIndex--;
						}
					}
					break;
				case IngredientStage.MODIFIER:
					//create a new modifier
					ModifierUnit newMod = new ModifierUnit(step.Type.ModType);
					newMod.Strength = step.Type.ModStrength;

					//modify the modifier
					foreach(ModifierUnit meta in activeMetaMods)
					{
						newMod.Strength = ApplyModifierToValue(val, out clarityChange, meta);
					}

					//apply or insert
					activeMods.Add(newMod);

					break;
				case IngredientStage.METAMODIFIER:
					//make a new metamod
					ModifierUnit newMeta = new ModifierUnit(step.Type.MetaModType);
					newMeta.Strength = step.Type.MetaModStrength;

					//add
					activeMetaMods.Add(newMeta);

					break;
			}

			//now, also check always. for now, always does not "use" modifiers at all - possible for them to but it's slightly more complex
		}

		//potion is valid and stats are done!
		Validity = PotionValidity.VALID;
		return _lastPotionVals;

	}

	public float ApplyModifierToValue(float val, out float clarityChange, ModifierUnit mod)
	{
		clarityChange = 0;

		switch (mod.ModType)
		{
			case ModifierType.Linear:
				val += mod.Strength;
				break;

			case ModifierType.Scalar:
				val *= mod.Strength;
				break;

			case ModifierType.Percentile:
				val *= (1 + (mod.Strength / 100));
				break;

			case ModifierType.Clarity:
				//no change to val
				clarityChange += mod.Strength;
				break;
		}

		return val;
	}

}





public partial class StatUnit : RefCounted
{
	public PotionStats Stat;
	public float Value;
	public float Clarity;

	public StatUnit(PotionStats stat)
	{
		Stat = stat;
		Value = 0;
		Clarity = 0;
	}
}

public partial class ModifierUnit : RefCounted
{
	public ModifierType ModType;
	public float Strength;
	public int TurnsLeft;

	public ModifierUnit(ModifierType modType)
	{
		ModType = modType;
		Strength = 0;
		TurnsLeft = -1;
	}

	
}

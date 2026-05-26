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
		TOO_MANY_INGREDIENTS,
		INVALID_BASE,
		INVALID_MOD,
		INVALID_META
	}

	private Array<PotionStep> _steps = new Array<PotionStep>();
	private bool _isDirty = true;

    private PotionValidity _validity;
	public PotionValidity Validity { get { 
        if(_isDirty)
            {
                RecalculatePotion();
            }
        return _validity;
        }  
    }
	private Dictionary<PotionStats, StatUnit> _lastPotionVals;
	//flavor is only updated when potion vals is updated!
	private float _flavor = 0;
	public float Flavor { get { return _flavor; } }

	[Export] public PotionThemes Theme;
	[Export] public Array<ShaderMaterial> PotShaders = new Array<ShaderMaterial>();


	public Dictionary<PotionStats, StatUnit> GetPotionValues()
	{
        //return old info if nothing has changed
        if (!_isDirty)
        {
            return _lastPotionVals;
        }

        RecalculatePotion();

        return _lastPotionVals;

	}


	public void RecalculatePotion(bool force = false)
	{
        //check dirty and basic validity
        if(!force)
        {
            //return old info if nothing has changed
            if (!_isDirty)
            {
                return;
            }

            //otherwise we need to construct a new one
            if (_lastPotionVals == null)
            {
                _lastPotionVals = new Dictionary<PotionStats, StatUnit>();
            }
            else
            {
                _lastPotionVals.Clear();
            }

            //check basic validity
            if (_steps.Count <= 0)
            {
                _validity = PotionValidity.NOT_ENOUGH_INGREDIENTS;
                return;
            }
            else if (_steps[0].Stage != IngredientStage.BASE)
            {
                _validity = PotionValidity.NO_FIRST_BASE;
                return;
            }
        }

		//fill in with a bunch of empty data
		foreach(PotionStats stat in Enum.GetValues(typeof(PotionStats)))
		{
			_lastPotionVals.Add(stat, new StatUnit(stat));
		}
		_flavor = 0;

        Array<ModifierUnit> activeMods = new Array<ModifierUnit>();
        Array<ModifierUnit> activeMetaMods = new Array<ModifierUnit>();

		//loop through all steps, applying from the most recent to the oldest. this is so modifiers apply properly
		for(int i = _steps.Count-1; i >= 0; i--)
		{
			PotionStep step = _steps[i];
			float val = 0; //used in many cases
			float clarityChange = 0;

			switch(step.Stage)
			{
				case IngredientStage.BASE:

					if(!step.Type.BaseValid)
					{
						_validity = PotionValidity.INVALID_BASE;
						return;
					}

					//update flavor
					_flavor += step.Type.BaseFlavor;

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

                    if (!step.Type.ModValid)
                    {
                        _validity = PotionValidity.INVALID_MOD;
                        return;
                    }

                    //update flavor
                    _flavor += step.Type.ModFlavor;

                    //create a new modifier
                    ModifierUnit newMod = new ModifierUnit(step.Type.ModType);
					newMod.Strength = step.Type.ModStrength;

                    //modify the modifier
                    foreach (ModifierUnit meta in activeMetaMods)
                    {
                        newMod.Strength = ApplyModifierToValue(val, out clarityChange, meta);
                    }

                    //apply or insert
                    activeMods.Add(newMod);

					break;
				case IngredientStage.METAMODIFIER:

                    if (!step.Type.MetaValid)
                    {
                        _validity = PotionValidity.INVALID_META;
                        return;
                    }

                    //update flavor
                    _flavor += step.Type.MetaFlavor;

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
		_validity = PotionValidity.VALID;
		_isDirty = false;


        //now, before we leave, update any of the shaders that we need to with the appropriate values...
        if (PotShaders.Count > 0)
		{
            UpdateThemeValues();
        }


		return;

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

	private void UpdateThemeValues()
	{
        StatUnit topStat = new StatUnit(PotionStats.NONE);
        StatUnit midStat = new StatUnit(PotionStats.NONE);
        StatUnit lowStat = new StatUnit(PotionStats.NONE);

        float top = 0;
        float mid = 0;
        float low = 0;

        Color topCol;
        Color midCol;
        Color lowCol;

        Vector4 topVec;
        Vector4 midVec;
        Vector4 lowVec;

        //first identify the top 3 stats
        foreach (StatUnit unit in _lastPotionVals.Values)
        {
            //we are determining "influence" by simply counting the max abs val of stat change
            float influence = Mathf.Abs(unit.Value);

            //you *could* write this logic once using pointers. fuck that!
            if (influence > top)
            {
                //low is the old mid (old low is "discarded")
                lowStat = midStat;
                low = mid;
                //mid is the old top
                midStat = topStat;
                mid = top;
                //and top is the new unit
                topStat = unit;
                top = influence;
            }
            else if (influence > mid)
            {
                //low is the old mid (old low is "discarded")
                lowStat = midStat;
                low = mid;
                //mid is the new unit
                midStat = unit;
                mid = influence;
            }
            else if (influence > low)
            {
                //low is the new unit (old low is "discarded")
                lowStat = unit;
                low = influence;
            }
        }



        //then, get their colors
        if (low == 0)
        {
            //if low is still 0, there is no lowest value, so we either need to send just one color, or mid as low, and a blend as mid
            if (mid == 0)
            {
                //if there is no top, send NONE's value for all three
                if (top == 0)
                {
                    topCol = Theme.GetStatColor(new StatUnit(PotionStats.NONE));
                    midCol = topCol;
                    lowCol = topCol;
                }
                else //otherwise, send top's value for all three
                {
                    topCol = Theme.GetStatColor(topStat);
                    midCol = topCol;
                    lowCol = topCol;
                }

            }
            else
            {
                //we have a top and a mid, but no low. Get the top and mid, and send top as top, low as mid, and mid as a blend of top and mid
                topCol = Theme.GetStatColor(topStat);
                lowCol = Theme.GetStatColor(midStat);
                midCol = topCol.Blend(lowCol);
            }
        }
        else
        {
            //we have top, middle, and low values. send them
            topCol = Theme.GetStatColor(topStat);
            midCol = Theme.GetStatColor(midStat);
            lowCol = Theme.GetStatColor(lowStat);
        }

        //then, turn each color into a vector4
        topVec = new Vector4(topCol.R, topCol.G, topCol.B, topCol.A);
        midVec = new Vector4(midCol.R, midCol.G, midCol.B, midCol.A);
        lowVec = new Vector4(lowCol.R, lowCol.G, lowCol.B, lowCol.A);

        //then, send those vectors to the shader
        foreach (ShaderMaterial s in PotShaders)
        {
            s.SetShaderParameter("top_val", topVec);
            s.SetShaderParameter("mid_val", midVec);
            s.SetShaderParameter("low_val", lowVec);

        }
    }


	public void AddStep(PotionStep newStep)
	{
		_steps.Add(newStep);
		_isDirty = true;
	}

	public void FlushPotion()
	{
		_steps.Clear();
		_isDirty = true;
	}

	public int GetStepCount()
	{
		return _steps.Count;
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

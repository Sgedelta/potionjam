using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class PotionThemes : Resource
{
    [Export] public Dictionary<PotionStats, Color> StatColors = new Dictionary<PotionStats, Color> 
    {
        {PotionStats.NONE, new Color(1, 1, 1, 1) },
        {PotionStats.Vision, new Color(1, 1, 1, 1) },
        {PotionStats.Vitality, new Color(1, 1, 1, 1) },
        {PotionStats.Energy, new Color(1, 1, 1, 1) },
        {PotionStats.Charm, new Color(1, 1, 1, 1) },
        {PotionStats.Bravery, new Color(1, 1, 1, 1) },
        {PotionStats.Age, new Color(1, 1, 1, 1) },
        {PotionStats.Goodness, new Color(1, 1, 1, 1) },
        {PotionStats.Weight, new Color(1, 1, 1, 1) },
        //add more here as more stats are added...

    };

    [Export] Curve ClarityAlphaCurve = new Curve();
    [Export] Curve ClaritySaturationCurve = new Curve();
    [Export] Curve ClarityBrightnessCurve = new Curve();

    public Color GetStatColor(StatUnit unit)
    {
        if(!StatColors.ContainsKey(unit.Stat))
        {
            throw new ArgumentException($"Potion Theme does not have stat {unit.Stat}!");
        }
        else if(unit.Stat == PotionStats.NONE)
        {
            //none is a special case and does not have a clarity. send the pure color
            return StatColors[PotionStats.NONE];
        }

        float baseHue = 0;
        float saturation = 0;
        float value = 0;
        float alpha = 0;

        StatColors[unit.Stat].ToHsv(out baseHue, out saturation, out value);

        //adjust based on curves

        alpha = ClarityAlphaCurve.Sample(Mathf.Clamp(unit.Clarity, ClarityAlphaCurve.MinDomain, ClarityAlphaCurve.MaxDomain));
        saturation *= ClaritySaturationCurve.Sample(Mathf.Clamp(unit.Clarity, ClaritySaturationCurve.MinDomain, ClaritySaturationCurve.MaxDomain));
        value *= ClarityBrightnessCurve.Sample(Mathf.Clamp(unit.Clarity, ClarityBrightnessCurve.MinDomain, ClarityBrightnessCurve.MaxDomain));

        return Color.FromHsv(baseHue, saturation, value, alpha);
    }

}

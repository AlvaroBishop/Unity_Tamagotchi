using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TamagotchiController : MonoBehaviour
{
    // Parámetros del Tamagotchi
    private float hunger = 100f;     // Hambre
    private float fun = 100f;        // Diversión
    private float hygiene = 100f;    // Higiene
    private float energy = 100f;     // Energía

    // Objetos de texto para mostrar los valores
    public Text hungerText;
    public Text funText;
    public Text hygieneText;
    public Text energyText;
    public Text moodText;

    // Conjuntos difusos para cada parámetro
    private Dictionary<string, FuzzySet> fuzzySets;

    // Reglas difusas
    private List<FuzzyRule> fuzzyRules;

    private void Start()
    {
        // Inicializar los conjuntos difusos y las reglas difusas
        InitializeFuzzySets();
        InitializeFuzzyRules();

        // Actualizar los objetos de texto con los valores iniciales
        UpdateParameterTexts();
    }

    private void Update()
    {
        // Evaluación difusa del estado de ánimo
        float mood = EvaluateMood();

        // Actualizar el objeto de texto del estado de ánimo
        UpdateMoodText(mood);

        // Actualizar los parámetros según el paso del tiempo
        UpdateParameters();
    }

    private void InitializeFuzzySets()
    {
        // Conjuntos difusos
        fuzzySets = new Dictionary<string, FuzzySet>();

        FuzzySet hungerSet = new FuzzySet();
        hungerSet.AddMembershipFunction("SinHambre", new MembershipFunction(0, 0, 10, 20));
        hungerSet.AddMembershipFunction("PocoHambre", new MembershipFunction(10, 20, 30, 40));
        hungerSet.AddMembershipFunction("HambreModerada", new MembershipFunction(30, 40, 50, 60));
        hungerSet.AddMembershipFunction("MuchaHambre", new MembershipFunction(50, 60, 100, 100));
        fuzzySets["Hunger"] = hungerSet;

        FuzzySet funSet = new FuzzySet();
        funSet.AddMembershipFunction("SinDiversión", new MembershipFunction(0, 0, 10, 20));
        funSet.AddMembershipFunction("PocaDiversión", new MembershipFunction(10, 20, 30, 40));
        funSet.AddMembershipFunction("DiversiónModerada", new MembershipFunction(30, 40, 50, 60));
        funSet.AddMembershipFunction("MuchaDiversión", new MembershipFunction(50, 60, 100, 100));
        fuzzySets["Fun"] = funSet;

        FuzzySet hygieneSet = new FuzzySet();
        hygieneSet.AddMembershipFunction("MalaHigiene", new MembershipFunction(0, 0, 10, 20));
        hygieneSet.AddMembershipFunction("RegularHigiene", new MembershipFunction(10, 20, 30, 40));
        hygieneSet.AddMembershipFunction("BuenaHigiene", new MembershipFunction(30, 40, 50, 60));
        hygieneSet.AddMembershipFunction("ExcelenteHigiene", new MembershipFunction(50, 60, 100, 100));
        fuzzySets["Hygiene"] = hygieneSet;

        FuzzySet energySet = new FuzzySet();
        energySet.AddMembershipFunction("BajaEnergía", new MembershipFunction(0, 0, 10, 20));
        energySet.AddMembershipFunction("ModeradaEnergía", new MembershipFunction(10, 20, 30, 40));
        energySet.AddMembershipFunction("AltaEnergía", new MembershipFunction(30, 40, 50, 60));
        energySet.AddMembershipFunction("MuyAltaEnergía", new MembershipFunction(50, 60, 100, 100));
        fuzzySets["Energy"] = energySet;
    }

    private void InitializeFuzzyRules()
    {
        // Reglas difusas
        fuzzyRules = new List<FuzzyRule>();

        FuzzyRule rule1 = new FuzzyRule();
        rule1.AddAntecedent(new MembershipTerm(fuzzySets["Hunger"].GetMembership("MuchaHambre"), () => hunger));
        rule1.SetConsequent(0.9f); // Valor difuso del estado de ánimo: 0.9 (triste)
        fuzzyRules.Add(rule1);

        FuzzyRule rule2 = new FuzzyRule();
        rule2.AddAntecedent(new MembershipTerm(fuzzySets["Fun"].GetMembership("PocaDiversión"), () => fun));
        rule2.SetConsequent(0.5f); // Valor difuso del estado de ánimo: 0.5 (neutral)
        fuzzyRules.Add(rule2);

        FuzzyRule rule3 = new FuzzyRule();
        rule3.AddAntecedent(new MembershipTerm(fuzzySets["Hygiene"].GetMembership("MalaHigiene"), () => hygiene));
        rule3.SetConsequent(0.7f); // Valor difuso del estado de ánimo: 0.7 (triste)
        fuzzyRules.Add(rule3);

        FuzzyRule rule4 = new FuzzyRule();
        rule4.AddAntecedent(new MembershipTerm(fuzzySets["Energy"].GetMembership("BajaEnergía"), () => energy));
        rule4.SetConsequent(0.6f); // Valor difuso del estado de ánimo: 0.6 (neutral)
        fuzzyRules.Add(rule4);

        // Reglas adicionales por cada conjunto difuso
        FuzzyRule rule5 = new FuzzyRule();
        rule5.AddAntecedent(new MembershipTerm(fuzzySets["Hunger"].GetMembership("SinHambre"), () => hunger));
        rule5.SetConsequent(0.1f); // Valor difuso del estado de ánimo: 0.1 (muy feliz)
        fuzzyRules.Add(rule5);

        FuzzyRule rule6 = new FuzzyRule();
        rule6.AddAntecedent(new MembershipTerm(fuzzySets["Fun"].GetMembership("MuchaDiversión"), () => fun));
        rule6.SetConsequent(0.8f); // Valor difuso del estado de ánimo: 0.8 (feliz)
        fuzzyRules.Add(rule6);

        FuzzyRule rule7 = new FuzzyRule();
        rule7.AddAntecedent(new MembershipTerm(fuzzySets["Hygiene"].GetMembership("ExcelenteHigiene"), () => hygiene));
        rule7.SetConsequent(0.7f); // Valor difuso del estado de ánimo: 0.7 (triste)
        fuzzyRules.Add(rule7);

        FuzzyRule rule8 = new FuzzyRule();
        rule8.AddAntecedent(new MembershipTerm(fuzzySets["Energy"].GetMembership("AltaEnergía"), () => energy));
        rule8.SetConsequent(0.9f); // Valor difuso del estado de ánimo: 0.9 (triste)
        fuzzyRules.Add(rule8);
    }

    private void UpdateParameters()
    {
        // Actualizar los parámetros en función del tiempo transcurrido
        float deltaTime = Time.deltaTime;

        // Disminuir el hambre con el tiempo
        hunger -= 5f * deltaTime;

        // Disminuir la diversión con el tiempo
        fun -= 2.5f * deltaTime;

        // Disminuir la higiene con el tiempo
        hygiene -= 4f * deltaTime;

        // Disminuir la energía con el tiempo
        energy -= 6f * deltaTime;

        // Limitar los valores entre 0 y 100
        hunger = Mathf.Clamp(hunger, 0f, 100f);
        fun = Mathf.Clamp(fun, 0f, 100f);
        hygiene = Mathf.Clamp(hygiene, 0f, 100f);
        energy = Mathf.Clamp(energy, 0f, 100f);

        // Actualizar los objetos de texto con los nuevos valores
        UpdateParameterTexts();
    }

    private void UpdateParameterTexts()
    {
        // Actualizar los objetos de texto con los valores de los parámetros
        hungerText.text = "Hambre: " + hunger.ToString();
        funText.text = "Diversión: " + fun.ToString();
        hygieneText.text = "Higiene: " + hygiene.ToString();
        energyText.text = "Energía: " + energy.ToString();
    }

    private void UpdateMoodText(float mood)
    {
        // Obtener los nombres de los conjuntos difusos
        string hungerSetName = "Hunger";
        string funSetName = "Fun";
        string hygieneSetName = "Hygiene";
        string energySetName = "Energy";

        // Obtener los nombres de los conjuntos difusos activos según los valores de los parámetros
        List<string> activeSets = new List<string>();
        if (fuzzySets[hungerSetName].GetMembership("MuchaHambre").CalculateMembership(hunger) > 0.5f)
            activeSets.Add("Tiene mucha hambre");
        if (fuzzySets[funSetName].GetMembership("PocaDiversión").CalculateMembership(fun) > 0.5f)
            activeSets.Add("Se divierte poco");
        if (fuzzySets[hygieneSetName].GetMembership("MalaHigiene").CalculateMembership(hygiene) > 0.5f)
            activeSets.Add("Tiene mala higiene");
        if (fuzzySets[energySetName].GetMembership("BajaEnergía").CalculateMembership(energy) > 0.5f)
            activeSets.Add("Tiene baja energía");

        // Determinar la tristeza y felicidad en función del estado de ánimo difuso
        float sadness = 1f - mood;
        float happiness = mood;

        // Construir el texto del estado de ánimo
        string text = "Estado de ánimo: ";
        if (activeSets.Count == 0)
        {
            text += "Neutral";
        }
        else
        {
            text += string.Join(", ", activeSets.ToArray());
        }
        text += "\n";

        // Textos variados basados en las reglas difusas
        if (mood < 0.3f)
            text += "Está triste";
        else if (mood > 0.7f)
            text += "Está muy feliz";
        else if (happiness > sadness)
            text += "Se siente más feliz que triste";
        else if (happiness < sadness)
            text += "Se siente más triste que feliz";
        else
            text += "Se siente igual de feliz y triste";

        text += "\n";
        text += "Tristeza: " + sadness.ToString("0.00") + "\n";
        text += "Felicidad: " + happiness.ToString("0.00");

        // Actualizar el objeto de texto del estado de ánimo
        moodText.text = text;
    }

    private float EvaluateMood()
    {
        // Evaluar el estado de ánimo difuso utilizando reglas difusas
        float result = 0f;

        foreach (FuzzyRule rule in fuzzyRules)
        {
            float ruleResult = 1f;

            foreach (IMembershipTerm antecedent in rule.antecedents)
            {
                float membership = antecedent.GetMembership();
                ruleResult = Mathf.Min(ruleResult, membership);
            }

            result = Mathf.Max(result, ruleResult * rule.consequent);
        }

        return result;
    }
}

public class FuzzySet
{
    private Dictionary<string, MembershipFunction> membershipFunctions;

    public FuzzySet()
    {
        membershipFunctions = new Dictionary<string, MembershipFunction>();
    }

    public void AddMembershipFunction(string name, MembershipFunction membershipFunction)
    {
        membershipFunctions[name] = membershipFunction;
    }

    public MembershipFunction GetMembership(string name)
    {
        return membershipFunctions[name];
    }
}

public class MembershipFunction
{
    public float a, b, c, d;

    public MembershipFunction(float a, float b, float c, float d)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
    }

    public float CalculateMembership(float value)
    {
        if (value <= a || value >= d)
        {
            return 0f;
        }
        else if (value >= b && value <= c)
        {
            return 1f;
        }
        else if (value < b)
        {
            return (value - a) / (b - a);
        }
        else
        {
            return (d - value) / (d - c);
        }
    }
}

public class FuzzyRule
{
    public List<IMembershipTerm> antecedents;
    public float consequent;

    public FuzzyRule()
    {
        antecedents = new List<IMembershipTerm>();
    }

    public void AddAntecedent(IMembershipTerm antecedent)
    {
        antecedents.Add(antecedent);
    }

    public void SetConsequent(float value)
    {
        consequent = value;
    }
}

public interface IMembershipTerm
{
    float GetMembership();
}

public class MembershipTerm : IMembershipTerm
{
    private MembershipFunction membershipFunction;
    private System.Func<float> parameterValueFunction;

    public MembershipTerm(MembershipFunction membershipFunction, System.Func<float> parameterValueFunction)
    {
        this.membershipFunction = membershipFunction;
        this.parameterValueFunction = parameterValueFunction;
    }

    public float GetMembership()
    {
        float parameterValue = parameterValueFunction();
        return membershipFunction.CalculateMembership(parameterValue);
    }
}

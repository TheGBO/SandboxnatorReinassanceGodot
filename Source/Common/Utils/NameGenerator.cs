using System;
using Godot;
using Godot.Collections;
namespace NullCyan.Util;

public class NameGenerator
{
    private Array<string> commonNameBeginnings;
    private Array<string> commonNameEndings;
    private Array<string> vowels;
    private Array<string> simpleConsonants;
    private readonly Random random;
    private bool useWesternPatterns;

    private NameGenerator()
    {
        // Default to simple patterns
        useWesternPatterns = false;
        
        // Western name elements
        commonNameBeginnings = [
            "Christ", "Joh", "Will", "Ed", "Rich", "Rob", "Thom", "Jam", "Mich", "Dav", "Paddy", "Jos", "Tom",
            "Alex", "Ben", "Charl", "Fran", "Georg", "Hen", "Jac", "Louis", "Matt", "Nathan", "Mat", "Bell",
            "Pat", "Sam", "Steph", "Tim", "Vict", "Zach", "Luc", "Max", "Osc", "Pete", "Edw", "Ew", "Mac", "Jon",
            "Ann", "Beth", "Carol", "Dian", "Ell", "Em", "Gabri", "Hann", "Isab", "Jess", "Giov", "Mig", "Bern",
            "Kath", "Laur", "Liz", "Mari", "Nic", "Oliv", "Rachel", "Sarah", "Soph", "Victor", "Bruces", "Hamilt"
        ];

        commonNameEndings = [
            "opher", "nathan", "iel", "iam", "ias", "uel", "ard", "ert", "ew", "in", "eph", "el", "lad", "lass",
            "ob", "on", "ory", "uel", "vin", "y", "ty", "dy", "ny", "my", "an", "ord", "bert", "id", "i",
            "ley", "ton", "son", "man", "las", "mas", "rus", "vin", "don", "bell", "loyd", "anni", "rich",
            "a", "ia", "ie", "y", "elle", "ette", "ine", "ana", "ella", "ora", "bush", "field", "land", "borough",
            "issa", "ica", "ena", "ara", "ina", "elle", "anne", "lyn", "rose", "mary", "ace", "ray", "taylor", "essa"
        ];

        vowels = ["a", "e", "i", "o", "u"];
        simpleConsonants = ["b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "r", "s", "t", "v", "w", "z"];
        
        random = new();
    }

    public static NameGenerator Create()
    {
        return new NameGenerator();
    }

    // Fluent builder methods
    public NameGenerator UseWesternPatterns()
    {
        this.useWesternPatterns = true;
        return this;
    }

    public NameGenerator UseSimplePatterns()
    {
        this.useWesternPatterns = false;
        return this;
    }

    public NameGenerator WithNameBeginnings(Array<string> beginnings)
    {
        this.commonNameBeginnings = beginnings;
        return this;
    }

    public NameGenerator WithNameEndings(Array<string> endings)
    {
        this.commonNameEndings = endings;
        return this;
    }

    public NameGenerator WithVowels(Array<string> vowels)
    {
        this.vowels = vowels;
        return this;
    }

    public NameGenerator WithConsonants(Array<string> consonants)
    {
        this.simpleConsonants = consonants;
        return this;
    }

    private string GenerateWesternName()
    {
        if (random.Next(5) > 0)
        {
            string beginning = commonNameBeginnings[random.Next(commonNameBeginnings.Count)];
            string ending = commonNameEndings[random.Next(commonNameEndings.Count)];
            
            if (IsConsonant(beginning[beginning.Length - 1]) && IsConsonant(ending[0]))
            {
                string vowelBridge = vowels[random.Next(vowels.Count)];
                return beginning + vowelBridge + ending;
            }
            
            return beginning + ending;
        }
        else
        {
            string firstSyllable = GenerateSyllable("CV");
            string secondSyllable = GenerateSyllable("CVC");
            return firstSyllable + secondSyllable;
        }
    }

    private string GenerateSimpleName()
    {
        int syllables = random.Next(2, 6);
        string name = "";
        for (int i = 0; i < syllables; i++)
        {
            name += GenerateSyllable("CV");
        }
        return name;
    }

    private string GenerateSyllable(string pattern)
    {
        string syllable = "";
        foreach (char c in pattern)
        {
            if (c == 'C')
            {
                syllable += simpleConsonants[random.Next(simpleConsonants.Count)];
            }
            else if (c == 'V')
            {
                syllable += vowels[random.Next(vowels.Count)];
            }
        }
        return syllable;
    }

    private bool IsConsonant(char c)
    {
        return !"aeiouAEIOU".Contains(c.ToString());
    }

    public string GenerateName()
    {
        string name = useWesternPatterns ? GenerateWesternName() : GenerateSimpleName();
        return char.ToUpper(name[0]) + name.Substring(1);
    }
}
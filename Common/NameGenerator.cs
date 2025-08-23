using System;
using Godot;
using Godot.Collections;

/// <summary>
/// Generates goofy random names that can be pronounced
/// </summary>
public class NameGenerator
{
    private Array<string> consonants;
    private Array<string> vowels;
    private bool randomizeSyllableCount;
    private int syllableCount;
    private Random random;

    private NameGenerator()
    {
        // Default values
        consonants = new Array<string>() { "p", "t", "k", "f", "s", "h", "m", "n", "r", "sh", "l", "y", "w", "b", "d", "g", "v", "z", "ch" };
        vowels = new Array<string>() { "a", "e", "i", "o", "u" };
        randomizeSyllableCount = false;
        syllableCount = 3;
        random = new Random();
    }

    public static NameGenerator Create()
    {
        return new NameGenerator();
    }

    public NameGenerator WithConsonants(Array<string> consonants)
    {
        this.consonants = consonants;
        return this;
    }

    public NameGenerator WithVowels(Array<string> vowels)
    {
        this.vowels = vowels;
        return this;
    }

    public NameGenerator WithSyllableCount(int syllableCount)
    {
        this.syllableCount = syllableCount;
        this.randomizeSyllableCount = false;
        return this;
    }

    public NameGenerator WithRandomSyllableCount(int minSyllables, int maxSyllables)
    {
        this.randomizeSyllableCount = true;
        this.syllableCount = random.Next(minSyllables, maxSyllables);
        return this;
    }


    public string GenerateSyllable()
    {
        //name simplification.
        Random r = random;
        string syllable = $"{consonants[r.Next(consonants.Count)]}{vowels[r.Next(vowels.Count)]}";
        return syllable;
    }

    public string GenerateName()
    {
        string name = "";
        for (int i = 0; i < syllableCount; i++)
        {
            name += GenerateSyllable();
        }
        return name;
    }
}
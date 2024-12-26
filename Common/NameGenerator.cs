using System;
using Godot.Collections;

public class NameGenerator
{
    private Array<string> consonants;
    private string vowels;

    public NameGenerator(Array<string> consonants, string vowels)
    {
        this.consonants = consonants;
        this.vowels = vowels;
    }

    public string GenerateSyllable()
    {
        Random r = new Random();
        string syllable = $"{consonants[r.Next(consonants.Count)]}{vowels[r.Next(vowels.Length)]}";
        return syllable;
    }

    public string GenerateName(int syllables)
    {
        string name = "";
        for (int i = 0; i < syllables; i++)
        {
            name += GenerateSyllable();
        }
        return name;
    }
}
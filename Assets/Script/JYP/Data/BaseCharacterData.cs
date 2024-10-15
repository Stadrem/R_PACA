﻿public class BaseCharacterData : ICharacterData
{
    public string Name { get; set; }
    public string Description { get; set; }

    public static BaseCharacterData Create(CharactersEntryController.CharacterEntry character)
    {
        return new BaseCharacterData()
        {
            Name = character.name,
            Description = character.description
        };
    }
}
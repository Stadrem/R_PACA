using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Data.Remote;
using UnityEngine;

namespace ViewModels
{
    public class UniverseEditViewModel : INotifyPropertyChanged
    {
        private string title;
        private string genre;
        private string content;
        private List<string> tags = new List<string>();
        private string objective = "";
        private List<CharacterInfo> characters = new();
        private List<BackgroundPartInfo> backgroundParts = new();
        private Dictionary<BackgroundPartInfo, List<BackgroundPartInfo>> adjacentList = new();
        private int nextBackgroundKey = 0;
    
        private DateTime createdDate = DateTime.Today;
        // private List<string> backgrounds = new List<string>();
        
        
        
        public string Title
        {
            get => title;
            set => SetField(ref title, value);
        }

        public string Genre
        {
            get => genre;
            set => SetField(ref genre, value);
        }

        public string Content
        {
            get => content;
            set => SetField(ref content, value);
        }

        public List<string> Tags
        {
            get => tags;
            set => SetField(ref tags, value);
        }

        public string Objective
        {
            get => objective;
            set => SetField(ref objective, value);
        }


        public List<CharacterInfo> Characters
        {
            get => characters;
            set => SetField(ref characters, value);
        }

        public IEnumerator CreateCharacter(CharacterInfo character)
        {
            yield return ScenarioCharacterApi.CreateScenarioAvatar(
                character,
                (result) =>
                {
                    if (result.IsSuccess)
                    {
                        character.id = result.value;
                        characters.Add(character);
                        OnPropertyChanged(nameof(Characters));
                    }
                    else
                    {
                        Debug.LogError($"error: {result.error}");
                    }
                }
            );
        }

        public DateTime CreatedDate
        {
            get => createdDate;
            set => SetField(ref createdDate, value);
        }

        public List<BackgroundPartInfo> BackgroundParts
        {
            get => backgroundParts;
            set => SetField(ref backgroundParts, value);
        }

        public Dictionary<BackgroundPartInfo, List<BackgroundPartInfo>> AdjacentList
        {
            get => adjacentList;
            set => SetField(ref adjacentList, value);
        }

        public void LinkBackgroundPart(BackgroundPartInfo from, BackgroundPartInfo to)
        {
            if (adjacentList[from]
                .Contains(to)) return;
            adjacentList[from]
                .Add(to);
            adjacentList[to]
                .Add(from);
        }

        public IEnumerator CreateBackground(string name, string description, EBackgroundPartType type)
        {
            var backgroundInfo = new BackgroundPartInfo()
            {
                id = nextBackgroundKey,
                Name = name,
                Type = type,
                description = description
            };

            yield return ScenarioBackgroundApi.CreateScenarioMap(
                backgroundInfo,
                (result) =>
                {
                    if (result.IsSuccess)
                    {
                        backgroundInfo.id = result.value;
                        BackgroundParts.Add(backgroundInfo);
                        AdjacentList[backgroundInfo] = new List<BackgroundPartInfo>();

                        OnPropertyChanged(nameof(BackgroundParts));
                    }
                    else
                    {
                        Debug.LogError($"error: {result.error}");
                    }
                }
            );
        }

        public void Init()
        {
            //load all data's in here
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        public void DeleteCharacter(int characterId)
        {
            var character = characters.FirstOrDefault(c => c.id == characterId);
            if (character == null) return;

            characters.Remove(character);
            OnPropertyChanged(nameof(Characters));
        }

        public IEnumerator CreateUniverse(Action<ApiResult<string>> onComplete) 
        {
            yield return ScenarioApi.CreateUniverse(
                Title,
                Objective,
                Content,
                new List<string> {Genre},
                Characters.Select(c => c.id).ToList(),
                new List<string>(),
                Tags,
                onComplete
            );
        }
    }
}
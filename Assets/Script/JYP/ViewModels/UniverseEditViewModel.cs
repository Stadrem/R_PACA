using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Data.Remote;
using ExitGames.Client.Photon;
using UnityEngine;

namespace ViewModels
{
    public sealed class UniverseEditViewModel : INotifyPropertyChanged
    {
        private string title;
        private string genre;
        private string content;
        private List<string> tags = new List<string>();
        private string objective = "";
        private List<CharacterInfo> characters = new();
        private List<BackgroundPartInfo> backgroundParts = new();
        private Dictionary<BackgroundPartInfo, BackgroundPartInfo> links = new();

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

        public IEnumerator CreateCharacter(CharacterInfo character, Action<ApiResult<string>> onComplete)
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
                        onComplete(ApiResult<string>.Success("success"));
                    }
                    else
                    {
                        Debug.LogError($"error: {result.error}");
                        onComplete(ApiResult<string>.Fail(result.error));
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

        public Dictionary<BackgroundPartInfo, BackgroundPartInfo> Links
        {
            get => links;
            set => SetField(ref links, value);
        }


        public IEnumerator LinkBackgroundPart(int fromId, int toId, Action<ApiResult<string>> onComplete)
        {
            var from = BackgroundParts.FirstOrDefault(p => p.ID == fromId);
            var to = BackgroundParts.FirstOrDefault(p => p.ID == toId);

            if (from != null && from.TowardBackground != null)
            {
                onComplete(ApiResult<string>.Fail(new InvalidDataException("already linked")));
                yield break; //error! already linked
            }

            if (IsCycleExist())
            {
                onComplete(ApiResult<string>.Fail(new InvalidDataException("cycle exist")));
                yield break; //error! cycle exist
            }

            var newFrom = new BackgroundPartInfo(from);
            newFrom.TowardBackground = to;
            yield return ScenarioBackgroundApi.UpdateScenarioBackground(
                newFrom,
                (result) =>
                {
                    if (result.IsSuccess)
                    {
                        from.TowardBackground = to;
                        Links.Add(from, to);
                        onComplete(ApiResult<string>.Success("success"));
                    }
                    else
                    {
                        onComplete(ApiResult<string>.Fail(result.error));
                    }
                }
            );
        }

        private bool IsCycleExist()
        {
            int Find(int[] parent, int v)
            {
                if (parent[v] == v) return v;
                return parent[v] = Find(parent, parent[v]);
            }

            void Union(int[] parent, int u, int v)
            {
                parent[u] = v;
            }

            var parent = new int[BackgroundParts.Count];
            for (int i = 0; i < parent.Length; i++)
            {
                parent[i] = i;
            }

            foreach (var link in Links)
            {
                var from = BackgroundParts.IndexOf(link.Key);
                var to = BackgroundParts.IndexOf(link.Value);
                if (Find(parent, from) == Find(parent, to))
                {
                    return true;
                }

                Union(parent, from, to);
            }

            return false;
        }


        public IEnumerator CreateBackground(string name, string description, EBackgroundPartType type,
            Action<ApiResult<string>> onComplete)
        {
            var backgroundInfo = new BackgroundPartInfo()
            {
                ID = -1,
                Name = name,
                Type = type,
                Description = description
            };

            yield return ScenarioBackgroundApi.CreateScenarioBackground(
                backgroundInfo,
                (result) =>
                {
                    if (result.IsSuccess)
                    {
                        backgroundInfo.ID = result.value.partId;
                        BackgroundParts.Add(backgroundInfo);
                        onComplete(ApiResult<string>.Success($"{result.value.partId}"));
                        OnPropertyChanged(nameof(BackgroundParts));
                    }
                    else
                    {
                        onComplete(ApiResult<string>.Fail(result.error));
                    }
                }
            );
        }

        public void Init()
        {
            //load all data's in here
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
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
                new List<string> { Genre },
                Characters.Select(c => c.id).ToList(),
                new List<string>(),
                Tags,
                onComplete
            );
        }
    }
}
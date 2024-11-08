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

        public IEnumerator CreateCharacter(CharacterInfo character, Action<ApiResult<CharacterInfo>> onComplete)
        {
            yield return ScenarioCharacterApi.CreateScenarioAvatar(
                character,
                (result) =>
                {
                    if (result.IsSuccess)
                    {
                        characters.Add(result.value);
                        OnPropertyChanged(nameof(Characters));
                        onComplete(result);
                    }
                    else
                    {
                        Debug.LogError($"error: {result.error}");
                        onComplete(ApiResult<CharacterInfo>.Fail(result.error));
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

        public IEnumerator UnlinkBackgroundPart(int fromId, Action<ApiResult<string>> onComplete)
        {
            var from = BackgroundParts.FirstOrDefault(p => p.ID == fromId);
            if (from == null)
            {
                onComplete(ApiResult<string>.Fail(new InvalidDataException("not found")));
                yield break;
            }

            var newFrom = new BackgroundPartInfo(from)
            {
                TowardBackground = null
            };

            yield return ScenarioBackgroundApi.UpdateScenarioBackground(
                newFrom,
                (result) =>
                {
                    if (result.IsSuccess)
                    {
                        from.TowardBackground = null;
                        Links.Remove(from);
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


        public IEnumerator DeleteBackground(int backgroundId, Action<ApiResult> onComplete)
        {
            var background = BackgroundParts.FirstOrDefault(b => b.ID == backgroundId);
            if (background == null)
            {
                onComplete(ApiResult.Fail(new InvalidDataException("not found")));
                yield break;
            }

            // if something toward this background, unlink it
            if (Links.ContainsValue(background))
            {
                var fromId = Links.First(pair => pair.Value == background).Key.ID;


                bool isFinished = false;
                yield return UnlinkBackgroundPart(
                    fromId,
                    (result) =>
                    {
                        isFinished = true;
                        if (result.IsFail)
                        {
                            onComplete(ApiResult.Fail(result.error));
                        }
                    }
                );
                yield return new WaitUntil(() => isFinished);
            }


            yield return ScenarioBackgroundApi.DeleteScenarioBackground(
                backgroundId,
                (result) =>
                {
                    if (result.IsSuccess)
                    {
                        BackgroundParts.Remove(background);
                        onComplete(ApiResult.Success());
                        OnPropertyChanged(nameof(BackgroundParts));
                    }
                    else
                    {
                        onComplete(ApiResult.Fail(result.error));
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
            var sortedBackgroundParts = new List<BackgroundPartInfo>();


            if (backgroundParts.Count > 1)
            {
                // find root (only "from" background not toward background)
                var count = new Dictionary<BackgroundPartInfo, int>();
                BackgroundPartInfo? node = null;
                foreach (var pair in links)
                {
                    if (!count.ContainsKey(pair.Key))
                    {
                        count[pair.Key] = 0;
                    }

                    if (!count.ContainsKey(pair.Value))
                    {
                        count[pair.Value] = 0;
                    }

                    count[pair.Key]++;
                    count[pair.Value]--;
                }

                foreach (var pair in count)
                {
                    if (pair.Value == 1)
                    {
                        node = pair.Key;
                    }
                }

                if (node == null)
                {
                    onComplete(ApiResult<string>.Fail(new InvalidDataException("no root")));
                    yield break;
                }

                sortedBackgroundParts.Add(node);
                while (node!.TowardBackground != null)
                {
                    sortedBackgroundParts.Add(node.TowardBackground);
                    node = node.TowardBackground;
                }

                if (sortedBackgroundParts.Count != BackgroundParts.Count)
                {
                    onComplete(ApiResult<string>.Fail(new InvalidDataException("not correct graph(trimmed)")));
                    yield break;
                }
            }
            else
            {
                sortedBackgroundParts = BackgroundParts;
            }

            yield return ScenarioApi.CreateUniverse(
                Title,
                Objective,
                Content,
                new List<string> { Genre },
                Characters.Select(c => c.id).ToList(),
                sortedBackgroundParts,
                new List<string>(),
                Tags,
                onComplete
            );
        }

        public IEnumerator UpdateCharacter(CharacterInfo characterInfo, Action<ApiResult> onComplete)
        {
            yield return ScenarioCharacterApi.UpdateScenarioAvatar(
                characterInfo,
                onComplete
            );
        }
    }
}
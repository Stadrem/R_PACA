using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.Models.Universe.Battle;
using Data.Models.Universe.Characters;
using Data.Models.Universe.Characters.Player;
using Data.Remote.Api;

namespace UniversePlay
{
    public sealed partial class UniversePlayViewModel
    {
        private List<UniversePlayer> universePlayers = new();

        public List<UniversePlayer> UniversePlayers
        {
            get => universePlayers;
            set => SetField(ref universePlayers, value);
        }

        public UniversePlayer CurrentPlayer => UniversePlayers.First();

        public void AddPlayer(UniversePlayer player)
        {
            universePlayers.Add(player);
            OnPropertyChanged(nameof(UniversePlayers));
        }

        public void RemovePlayer(int userCode)
        {
            universePlayers.RemoveAll(player => player.UserCode == userCode);
            OnPropertyChanged(nameof(UniversePlayers));
        }
        
        public void UpdateStatByUserCodeWithoutRemote(int userCode, CharacterStats stats)
        {
            var player = universePlayers.Find(p => p.UserCode == userCode);
            player.Stats = stats;
            OnPropertyChanged(nameof(UniversePlayers));
        }

        public IEnumerator UpdateStatByUserCode(int userCode, CharacterStats stats)
        {
            var player = universePlayers.Find(p => p.UserCode == userCode);
            var universePlayerSetting = new UniversePlayerSettings(
                universeData.id,
                player.UserCode,
                stats
            );

            yield return ScenarioUserSettingsApi.UploadUserSettings(
                universePlayerSetting,
                (res) =>
                {
                    if (res.IsSuccess)
                    {
                        player.Stats = stats;
                        OnPropertyChanged(nameof(UniversePlayers));
                    }
                }
            );
        }

        public IEnumerator FinishBattle(
            int roomNumber, 
            bool isBattleWon, 
            Dictionary<int, int> userHp, // userCode - currentHp
            int currentInteractNpcHp,
            Action<ApiResult> callback
            )
        {
            var players = userHp.Select(pair => new BattleResultPlayer(pair.Key, pair.Value)).ToList();
            var battleResult = new BattleResult(
                roomNumber,
                isBattleWon,
                players,
                new BattleResultNpc(PlayUniverseManager.Instance.NpcManager.currentInteractInGameNpc.NpcId, currentInteractNpcHp)
            );
            yield return PlayProgressApi.SendBattleResult(
                battleResult,
                (res) =>
                {
                    if (res.IsSuccess)
                    {
                        callback(ApiResult.Success());
                    }
                    else
                    {
                        callback(ApiResult.Fail(res.error));
                    }
                }
            );
        }
    }
}
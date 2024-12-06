using Data.Models.Universe.Characters;
using Data.Models.Universe.Characters.Player;
using Photon.Pun;
using UnityEngine;
using ViewModels;

namespace UniversePlay
{
    public class InGamePlayerManager : MonoBehaviour
    {
        private Vector3 spawnPos;

        private CameraController currentPlayerCameraController;

        private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;


        /// <summary>
        /// PUN이 전역적으로 호출하기 때문에 SpawnPlayers라는 이름을 가졌지만, 하나의 플레이어만 스폰한다.
        /// </summary>
        public void SpawnPlayers()
        {
            spawnPos = GameObject.Find("SpawnPoint")
                           ?.transform.position
                       ?? Vector3.zero;

            print($"{ViewModel.CurrentPlayer.Nickname} -- {ViewModel.CurrentPlayer.Stats.GetStat(EStatType.Hp)}");
            SpawnPlayer(ViewModel.CurrentPlayer);
            currentPlayerCameraController = Camera.main?.GetComponent<CameraController>();
        }

        private void SpawnPlayer(UniversePlayer player)
        {
            var go = PhotonNetwork.Instantiate(
                "Player_Avatar",
                spawnPos,
                Quaternion.identity,
                0,
                new object[]
                {
                    player.Stats.GetStat(EStatType.Hp),
                    player.Stats.GetStat(EStatType.Str),
                    player.Stats.GetStat(EStatType.Dex)
                }
            );

            
        }

        public void BlockPlayerCamera()
        {
            currentPlayerCameraController.isBlocked = true;
        }

        public void UnblockPlayerCamera()
        {
            currentPlayerCameraController.isBlocked = false;
        }
    }
}
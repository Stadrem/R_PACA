using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Background
{
    public int Id => info.ID;
    public string Name => info.Name;
    private BackgroundPartInfo info;
    private List<PortalInPlay> portalParts;
    private Transform parent = null;
    
    private static InGamePlayerManager PlayerManager => PlayUniverseManager.Instance.InGamePlayerManager;
    public void Init(BackgroundPartInfo info)
    {
        this.info = info;
        PlayerManager.SpawnPlayers();
    }
    
}
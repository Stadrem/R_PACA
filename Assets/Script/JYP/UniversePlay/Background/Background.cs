using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Background
{
    public int Id => info.ID;

    private BackgroundPartInfo info;
    private List<PortalInPlay> portalParts;
    private Transform parent = null;

    public void Init(BackgroundPartInfo info)
    {
        this.info = info;
        PlayUniverseManager.Instance.InGamePlayerManager.SpawnPlayers();
    }

    public void LoadParts()
    {
        if (info == null) return;
        Debug.Log($"LoadParts on {SceneManager.GetActiveScene().name} / {info.Name}");
        var portals = info.PortalList;
        foreach (var portal in portals)
        {
            var portalPrefab = Resources.Load<GameObject>("BackgroundPart/Portal_Play");
            var go = GameObject.Instantiate(portalPrefab, portal.position, Quaternion.identity, parent);
            var portalInPlay = go.GetComponent<PortalInPlay>();
            portalInPlay.Init(portal);
        }
    }
    
}
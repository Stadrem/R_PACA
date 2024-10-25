using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Background
{
    public int Id => data.id;

    private BackgroundPartData data;
    private List<PortalInPlay> portalParts;
    private Transform parent = null;

    public void Init(BackgroundPartData data)
    {
        this.data = data;
        PlayUniverseManager.Instance.InGamePlayerManager.SpawnPlayers();
    }

    public void LoadParts()
    {
        if (data == null) return;
        Debug.Log($"LoadParts on {SceneManager.GetActiveScene().name} / {data.Name}");
        var portals = data.portalList;
        foreach (var portal in portals)
        {
            var portalPrefab = Resources.Load<GameObject>("BackgroundPart/Portal_Play");
            var go = GameObject.Instantiate(portalPrefab, portal.position, Quaternion.identity, parent);
            var portalInPlay = go.GetComponent<PortalInPlay>();
            portalInPlay.Init(portal);
        }
    }
    
}
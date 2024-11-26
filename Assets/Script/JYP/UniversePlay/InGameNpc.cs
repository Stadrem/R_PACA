using System;
using System.Collections;
using System.Linq;
using Cinemachine;
using Data.Models.Universe.Characters;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModels;

public class InGameNpc : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    private UniverseNpc universeNpcData;
    public int NpcId => universeNpcData.Id;
    public string NpcName => universeNpcData.Name;
    
    public Image interactableIcon;
    
    public CinemachineVirtualCamera ncVcam;
    public TMP_Text npcNameText;
    public GameObject root;
    
    private Coroutine colorChangeCoroutine;
    private bool isHovered;
    public bool IsInteractable
    {
        get => interactableIcon?.gameObject?.activeSelf ?? false;
        set => interactableIcon.gameObject.SetActive(value);
    }
    public void OnHovered()
    {
        Debug.Log($"OnHovered {universeNpcData.Name}");
        if(interactableIcon == null || !interactableIcon.gameObject.activeSelf)
            return;

        if (colorChangeCoroutine != null)
        {
            StopCoroutine(colorChangeCoroutine);
        }
        
        colorChangeCoroutine = StartCoroutine(ColorChange(0.5f, Color.green));
    }
    
    private IEnumerator ColorChange(float time, Color targetColor)
    {
        float elapsedTime = 0;
        var startColor = interactableIcon.color;
        while (elapsedTime < time)
        {
            interactableIcon.color = Color.Lerp(startColor, targetColor, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void Update()
    {
        //check hover
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if(Physics.Raycast(ray, out var hit, 100))
        {
            if (hit.collider.gameObject == gameObject && !isHovered)
            {
                isHovered = true;
                OnHovered();
            }
            else if(hit.collider.gameObject != gameObject && isHovered)
            {
                isHovered = false;
                OnHoverExit();
            }
        }
        else if(isHovered)
        {
            isHovered = false;
            OnHoverExit();
        }
        
    }

    private void OnHoverExit()
    {
        Debug.Log($"OnHoverExit {universeNpcData.Name}");
        if (colorChangeCoroutine != null)
            StopCoroutine(colorChangeCoroutine);
        
        colorChangeCoroutine = StartCoroutine(ColorChange(0.5f, Color.white));
    }

    public void Init(UniverseNpc npc)
    {
        universeNpcData = npc;
        npcNameText.text = npc.Name;
        transform.localPosition = npc.Position;
    }
    
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        print("OnPhotonInstantiate - " + info.photonView.InstantiationData[0]);
        var id = Convert.ToInt32(info.photonView.InstantiationData[0]);
        PlayUniverseManager.Instance.NpcManager.AddNpc(this);
        var npc = ViewModelManager.Instance.UniversePlayViewModel.CurrentMapNpcList
            .First(t => t.Id == id);
        
        Init(npc);
    }
}
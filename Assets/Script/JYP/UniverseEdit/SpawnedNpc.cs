﻿using System;
using UnityEngine;

namespace UniverseEdit
{
    public class SpawnedNpc : MonoBehaviour, IDraggable
    {
        public bool IsDraggable => true;
        public NpcSpawner npcSpawner;
        public int characterId { get; set; } = -1;
        private Vector3 startPos = Vector3.zero;

        public void StartDrag()
        {
            startPos = transform.position;
        }

        public void Dragging(Vector3 position)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, float.MaxValue, 1 << LayerMask.NameToLayer("DetailGround")))
            {
                transform.position = position;
            }
        }

        public void StopDrag()
        {
            // check ray cast for UI or detail ground
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (RectTransformUtility.RectangleContainsScreenPoint(
                    npcSpawner.npcListTransform,
                    Input.mousePosition
                ))
            {
                npcSpawner.ReturnToUi(characterId);
            }
            else if (Physics.Raycast(ray, out var hit, float.MaxValue, 1 << LayerMask.NameToLayer("DetailGround")))
            {
                npcSpawner.UpdateNpcPosition(
                    characterId,
                    hit.point,
                    (res) =>
                    {
                        if (res.IsFail)
                            transform.position = startPos;
                    }
                );
            }
            else
            {
                transform.position = startPos;
            }
        }
    }
}
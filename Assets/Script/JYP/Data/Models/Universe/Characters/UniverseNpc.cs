using UnityEngine;

namespace Data.Models.Universe.Characters
{
    [System.Serializable]
    public class UniverseNpc
    {
        public enum ENpcType
        {
            None = -1,
            Human,
            Goblin,
            Elf,
            Golem,
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public CharacterStats Stats { get; private set; }

        public float YRotation { get; private set; }
        public Vector3 Position { get; private set; }
        public ENpcType NpcShapeType { get; private set; }
        public int AttachedBackgroundId { get; private set; }

        public int currentHp;
        public UniverseNpc(int id, string name, string description, CharacterStats stats, float yRotation,
            Vector3 position, ENpcType npcShapeType, int attachedBackgroundId
        )
        {
            Id = id;
            Name = name;
            Description = description;
            Stats = stats;
            YRotation = yRotation;
            Position = position;
            NpcShapeType = npcShapeType;
            AttachedBackgroundId = attachedBackgroundId;
        }
    }
}
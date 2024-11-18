using System;

namespace Data.Models.Universe
{
    [Flags]
    public enum EHUDState : byte
    {
        Chat = 0b1,
        Dice = 0b10,
    }
}
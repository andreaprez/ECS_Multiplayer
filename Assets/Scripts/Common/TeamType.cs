﻿namespace ECS_Multiplayer.Client.Common
{
    public enum TeamType : byte
    {
        None = 0,
        Blue = 1,
        Red = 2,
        AutoAssign = byte.MaxValue
    }
}
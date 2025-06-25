using System;


[Serializable]
public class MazeSettings 
{
    public ushort width = 21;
    public ushort height = 21;
    public byte numberOfExits = 3;
    public byte minDistanceBetweenExits = 5;
    public ushort additionalPassages = 15;
    public ushort roomChance = 20;
    public bool useRandomSeed = true;
    public int seed = 12345;
}

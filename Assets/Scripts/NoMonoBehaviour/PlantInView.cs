
using UnityEngine;

public class PlantInView : Plant
{
    public Vector3 Position { get; set; }
    public PlantInView(Vector3 position, int[] dna) : base(dna)
    {
        Position = position;
    }

}
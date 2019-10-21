public class PlantInBag:Plant{
    
    public int Count { get; set; }
    public PlantInBag(int[] dna, int count):base(dna)
    {
        Count = count;
    }
    
}

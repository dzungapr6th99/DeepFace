namespace VectorDbObj
{
    public class FaceDbObject
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ReadOnlyMemory<float> Vector { get; set; }   
    }
}

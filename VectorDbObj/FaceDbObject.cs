using MilvusDA.CustomAttributes;

namespace VectorDbObj
{

    public class FaceDbObject
    {
        [DbField(IsAutoId = true)]
        public long Id { get; set; }
        public string FaceId { get; set; } = string.Empty;
        [DbField(IsVector = true)]
        public ReadOnlyMemory<float> EmbededVector { get; set; }   
    }
}

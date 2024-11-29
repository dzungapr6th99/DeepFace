using MilvusDA.CustomAttributes;

namespace VectorDbObj
{
    [DbTable(CollectionName = "FaceEmbeding")]
    public class FaceDbObject
    {
        [DbField(IsAutoId = true)]
        public long? Id { get; set; }
        public string FaceId { get; set; } = string.Empty;
        [DbField(IsVector = true)]
        public List<float> EmbededVector { get; set; }   
    }
}

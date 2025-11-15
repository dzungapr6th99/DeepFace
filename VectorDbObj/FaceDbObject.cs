using MilvusDA.CustomAttributes;

namespace VectorDbObj
{
    [DbTable(CollectionName = "FaceEmbeding")]
    public class FaceDbObject
    {
        [DbField(IsAutoId = true, FieldName = "id")]
        public long? Id { get; set; }
        [DbField(FieldName = "faceId")]
        public string FaceId { get; set; } = string.Empty;
        [DbField(IsVector = true, FieldName = "embededVector")]
        public List<float> EmbededVector { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilvusDA.CustomAttributes
{
    public class DbTableAttribute : System.Attribute
    {
        public string CollectionName { get; set; } = string.Empty;
        public string VectorFieldName { get; set; } = string.Empty;
    }

    public class DbFieldAttribute : System.Attribute
    {
        public string FieldName { get; set; } = string.Empty;
        /// <summary>
        /// Describe is this field is primary key (developer generate by them self)
        /// </summary>
        public bool IsKey { get; set; } = false;
        /// <summary>
        /// Describe is this field is primary key (milvus automatic genẻate the key)
        /// </summary>
        public bool IsAutoId { get; set; } = false;

        public bool IsVector { get; set; } = false;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorDbObj.CustomAttribute
{
    public class CollectionAttribute : Attribute
    {
        public string CollectionName { get; set; }  
        
    }

    public class FieldSchemaAttribute : Attribute
    {
        public string FieldName { get; set; }
        public bool IsVector { get; set; } = false;
        
    }


}

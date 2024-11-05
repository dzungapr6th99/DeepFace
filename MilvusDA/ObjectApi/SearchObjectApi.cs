using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MilvusDA.ObjectApi
{
    public class SearchObjectApi
    {
        /// <summary>
        /// The name of the database.
        /// </summary>
        public string DbName { get;set; }
        /// <summary>
        /// The name of the collection to which this operation applies.
        /// </summary>
        public string CollectionName { get;set; }
        /// <summary>
        /// A list of vector embeddings. Milvus searches for the most similar vector embeddings to the specified ones.
        /// </summary>
        public List<object> Data { get; set; }
        /// <summary>
        /// The name of the vector field.
        /// </summary>
        public string AnnsField { get; set; }
        /// <summary>
        /// The filter used to find matches for the search.
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// The total number of entities to return. You can use this parameter in combination with offset in param to enable pagination. 
        /// The sum of this value and offset in param should be less than 16,384. 
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// The number of records to skip in the search result. You can use this parameter in combination with limit to enable pagination. 
        /// The sum of this value and limit should be less than 16,384. 
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// The name of the field that serves as the aggregation criteria.
        /// </summary>
        public string GroupingField { get; set; }
        /// <summary>
        /// An array of fields to return along with the search results.
        /// </summary>
        public List<string> OutputFields { get; set; }
        public SearchParameter SearchParams { get; set; }   
        public List<string> PartitionNames { get; set; }
       
    }
    /// <summary>
    /// The parameter settings specific to this operation.
    /// </summary>
    public class SearchParameter
    {
        public MertricType MertricType { get; set; }
        public Param Params { get;set; }
    }

    public enum MertricType
    {
        L2,
        IP,
        COSINE
    }
    /// <summary>
    /// Extra search parameters.
    /// </summary>
    public class Param
    {
        /// <summary>
        /// Determines the threshold of least similarity. When setting metrictype to L2, ensure that this value is greater than that of rangefilter. 
        /// Otherwise, this value should be lower than that of range_filter. 
        /// </summary>
        public int Radius { get; set; }
        /// <summary>
        /// Refines the search to vectors within a specific similarity range. 
        /// When setting metric_type to IP or COSINE, ensure that this value is greater than that of radius. 
        /// Otherwise, this value should be lower than that of radius. 
        /// </summary>
        public int Range_filter { get; set; }
    }
    /// <summary>
    /// Use when you have class mapping with item in collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SearchResponse<T> where T : class
    {
        public int Code { get; set; }   
        public string Message { get; set; }
        public List<T> Data { get; set; }

    }
    /// <summary>
    /// Use when you don't have class mapping with item in collection
    /// </summary>
    public class SearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
    }
}

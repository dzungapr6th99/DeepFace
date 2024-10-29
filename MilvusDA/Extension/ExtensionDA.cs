using Microsoft.Extensions.Logging;
using Milvus.Client;
using MilvusDA.CustomAttributes;
using MilvusDA.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MilvusDA.Extension
{
    public static class ExtensionDA
    {
        public static MilvusCollection? GetOrCreateCollection<T>(this IMilvusHelper milvusHelper) where T : class
        {
            string collectionName = GetCollectionName(typeof(T));
            if (string.IsNullOrEmpty(collectionName))
            {
                return milvusHelper.GetCollection(collectionName);
            }
            else
            {
                return null;
            }
        }


        public static bool Insert<T>(this IMilvusHelper milvusHelper, T data) where T : class
        {
            string collectionName = GetCollectionName(typeof(T));
            List<Tuple<string, object>> fieldValueNames = new List<Tuple<string, object>>();
            ReadOnlyMemory<float> vector = GetVector<T>(data, fieldValueNames, out string fieldVectorNames);
            if (!string.IsNullOrEmpty(fieldVectorNames))
            {
                milvusHelper.Insert(collectionName, new Tuple<string, ReadOnlyMemory<float>>(fieldVectorNames, vector), fieldValueNames.ToArray());
                return true;
            }
            return false;
        }

        public static T Search<T>(this IMilvusHelper milvusHelper,T entity)
        {
            milvusHelper.Search(entity,)
        } 

        private static string GetCollectionName(this Type type)
        {
            var dbTableAttribute = (DbTableAttribute)type.GetCustomAttributes(typeof(DbTableAttribute))?.FirstOrDefault();
            if (dbTableAttribute != null && !string.IsNullOrEmpty(dbTableAttribute.CollectionName))
            {
                return dbTableAttribute.CollectionName;
            }

            return type.Name;
        }

        private static ReadOnlyMemory<float> GetVector<T>(T entity, List<Tuple<string, object>> fieldValueNames, out string fieldVectorName) where T : class
        {
            var dbFieldAttribute = (DbFieldAttribute)typeof(T).GetCustomAttributes(typeof(DbFieldAttribute))?.FirstOrDefault();
            var paramProps = entity.GetType().GetProperties();

            foreach (var prop in paramProps)
            {
                var attribute = (DbFieldAttribute)prop.GetCustomAttributes(typeof(DbFieldAttribute))?.FirstOrDefault();
                if (attribute == null || !attribute.IsVector)
                {
                    fieldValueNames.Add(new Tuple<string, object>(prop.Name, prop.GetValue(entity)));
                }

                fieldVectorName = !string.IsNullOrEmpty(attribute.FieldName) ? attribute.FieldName : prop.Name;
                return (ReadOnlyMemory<float>)prop.GetValue(entity);
            }
            fieldVectorName = string.Empty;
            return new ReadOnlyMemory<float>();
        }

        private static bool IsVectorField(this Type type)
        {
            var dbFieldAttribute = (DbFieldAttribute)type.GetCustomAttributes(typeof(DbFieldAttribute))?.FirstOrDefault();
            if (dbFieldAttribute != null && dbFieldAttribute.IsVector)
            {
                return true;
            }
            return false;

        }


    }
}

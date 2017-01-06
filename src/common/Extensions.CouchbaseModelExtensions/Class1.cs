using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions.StringExtensions;
using System.Reflection;
using Extensions.StringExtensions;
using inflector_extension;
using Couchbase.Core;

namespace Extensions.CouchbaseModelExtensions
{
    public static class CouchbaseModelExtension//<T> where T : ModelBase
    {
        public static string ToForeignDocumentKeyId<T>(this String id)
        {
            return CouchbaseModelExtension.ToDocumentKeyId<T>(id);
        }

        private static string ToDocumentKeyId<T>(this String id)
        {
            return String.Format("{0}>{1}", typeof(T).Name.InflectTo().Underscored,
                                            id.InflectTo().Underscored).ToLower();
        }

        public static string BeutifyDocumentKeyId<T>(this String id)
        {
            return id.InflectTo()
                     .Underscored
                     .Replace(String.Format("{0}>", typeof(T).Name.InflectTo().Underscored.ToLower()), String.Empty);
        }

        public static string ReplaceBaseDocType<TDocType>(this String id)
        {
            var keys = id.Split('>').ToList();

            if (!keys.Any())
                return id;

            keys.RemoveAt(0);

            StringBuilder sb = new StringBuilder();

            var newDocType = typeof(TDocType).Name.InflectTo().Underscored.ToLower();

            sb.Append(newDocType);
            sb.Append(">");

            foreach (var key in keys)
            {
                sb.Append(String.Format("{0}>", key));
            }

            return sb.ToString();
        }

        public static string ToNonDocumentKeyId<T>(this String id)
        {
            var camel = id.BeutifyDocumentKeyId<T>()
                          .InflectTo()
                          .Camelized;
            var result = String.Format("{0}{1}", camel.Substring(0, 1).ToUpper(),
                                                 camel.Substring(1, camel.Length - 1));

            return result;
        }


        public static string ToPrimaryDocumentKeyId<T>(this String id)
        {
            return CouchbaseModelExtension.ToDocumentKeyId<T>(id);
        }

        public static string FromForeignDocumentKeyIdToPrimaryDocumentKeyId<T>(this String id)
        {
            return String.Format("{0}>{1}", typeof(T).Name.InflectTo().Underscored, id.InflectTo().Underscored).ToLower();
        }

        private static bool ContainsDocType(this String key, string docType)
        {
            if (key.IsNullOrEmptyOrWhiteSpace())
                return false;

            if (docType.Contains(key.Split('>').First()))
                return true;

            return false;
        }

        private static string ToCouchbaseDocyType(this String docType)
        {
            if (!docType.IsNullOrEmptyOrWhiteSpace())
                return String.Format("{0}>", docType.InflectTo().Underscored);

            return null;
        }

        private static string BeutifyKey(this String key, string keyType)
        {
            if (!key.IsNullOrEmptyOrWhiteSpace())
                return key.Replace(String.Format("{0}>", keyType.InflectTo().Underscored), String.Empty);

            return key;
        }

        private static void ToForeignKey(this IModel entity)
        {
            PrependDocType<ForeignKeyAttribute>(entity);
        }

        private static void PrependDocType<TKey>(this IModel entity) where TKey : IKey
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();

            var entityType = entity.GetType();

            if (typeof(TKey) == typeof(ForeignKeyAttribute))
                properties.AddRange(GetKeys<ForeignKeyAttribute>(entityType));

            if (typeof(TKey) == typeof(PrimaryKeyAttribute))
                properties.AddRange(GetKeys<PrimaryKeyAttribute>(entityType));

            if (typeof(TKey) == typeof(CompositePrimaryKeyAttribute))
                properties.AddRange(GetKeys<CompositePrimaryKeyAttribute>(entityType));

            if (!properties.Any())
                return;

            foreach (var property in properties)
            {
                string docType = null;
                var propVal = property.GetValue(entity, null);

                //don't append to a key that is not supplied
                if (propVal == null)
                    continue;

                //get doc type
                if (typeof(TKey) == typeof(PrimaryKeyAttribute))
                    docType = entityType.Name.ToCouchbaseDocyType();

                if (typeof(TKey) == typeof(CompositePrimaryKeyAttribute))
                {
                    var fkAttribute = property.GetCustomAttribute<CompositePrimaryKeyAttribute>(true);

                    docType = fkAttribute.KeyType == null ? String.Empty : fkAttribute.KeyType.Name.ToCouchbaseDocyType();
                }

                if (typeof(TKey) == typeof(ForeignKeyAttribute))
                {
                    var fkAttribute = property.GetCustomAttribute<ForeignKeyAttribute>(true);
                    docType = fkAttribute.KeyType == null ? String.Empty : fkAttribute.KeyType.Name.ToCouchbaseDocyType();
                }

                var formattedPropVal = String.Format("{0}", propVal).InflectTo().Underscored;

                if (String.Format("{0}", propVal).ContainsDocType(docType))
                    continue;

                property.SetValue(entity, String.Format("{0}{1}", docType,
                                                                   formattedPropVal));
            }
        }

        private static List<PropertyInfo> GetKeys<TKeyType>(Type entity) where TKeyType : IKey
        {
            var properties = entity.GetProperties()
                                 .Where(property => property.GetCustomAttributes(true)
                                                            .Where(attr => attr is TKeyType)
                                                            .Any())
                                 .ToList();

            return properties;
        }

        private static bool IsPropertyTypeOfKey<TKeyType>(PropertyInfo property) where TKeyType : IKey
        {
            var key = property.GetCustomAttributes(true)
                              .Any(attr => attr is TKeyType);

            return key;
        }

        private static bool EntityContainsKey<TKeyType>(Type entityType) where TKeyType : Attribute
        {
            return entityType.GetCustomAttributes<TKeyType>(true).Any();
        }

        private static void ToPrimaryKey(this IModel entity)
        {
            var entityType = entity.GetType();
            var docType = entityType.Name.ToCouchbaseDocyType();

            entity.PrependDocType<PrimaryKeyAttribute>();
            entity.PrependDocType<ForeignKeyAttribute>();

            var pkProperty = GetKeys<PrimaryKeyAttribute>(entity.GetType()).FirstOrDefault();

            if (pkProperty == null)
                return;

            if (entity.Id.ContainsDocType(docType))
                return;

            entity.Id = String.Format("{0}", pkProperty.GetValue(entity, null));
        }

        private static void ToCompositePrimaryKey(this IModel entity)
        {
            var entityType = entity.GetType();
            var docType = entityType.Name.ToCouchbaseDocyType();

            var properties = GetKeys<CompositePrimaryKeyAttribute>(entityType);

            Dictionary<CompositePrimaryKeyAttribute, string> pks = new Dictionary<CompositePrimaryKeyAttribute, string>();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<CompositePrimaryKeyAttribute>(true);

                if (attribute == null)
                    continue;

                pks.Add(attribute, String.Format("{0}", property.GetValue(entity, null)).InflectTo().Underscored.ToLower());
            }

            if (!pks.Any())
                return;

            var orderedPks = pks.OrderBy(i => i.Key.Order);

            StringBuilder sb = new StringBuilder();

            sb.Append(docType);

            foreach (var pk in orderedPks)
            {
                if (pk.Key.KeyType == null)
                    sb.Append(String.Format("{0}{1}", pk.Value, ">"));
                else
                    sb.Append(String.Format("{0}{1}", pk.Value.BeutifyKey(pk.Key.KeyType.Name), ">"));
            }

            entity.PrependDocType<CompositePrimaryKeyAttribute>();

            if (entity.Id.ContainsDocType(docType))
                return;

            var key = sb.ToString();

            //exclude tha last >
            entity.Id = key.Substring(0, key.Length - 1);
        }

        public static void Keyify(this IModel entity)
        {

            if (entity == null || !entity.GetType().GetCustomAttributes<EntityAttribute>(true).Any())
                return;

            entity.ToPrimaryKey();
            entity.ToCompositePrimaryKey();
            entity.ToForeignKey();
        }

        public static void DeKeyify(this IModel entity)
        {
            if (entity == null)
                return;

            if (!entity.GetType().GetCustomAttributes<EntityAttribute>(true).Any())
                return;

            entity.RemoveDocTypes();
        }

        private static void RemoveDocTypes(this IModel entity)
        {
            var entityType = entity.GetType();
            var docType = entityType.Name.ToCouchbaseDocyType();

            entity.RemoveDocType<PrimaryKeyAttribute>();
            entity.RemoveDocType<ForeignKeyAttribute>();
            entity.RemoveDocType<CompositePrimaryKeyAttribute>();
        }

        private static void RemoveDocType<TKey>(this IModel entity) where TKey : IKey
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();

            var entityType = entity.GetType();

            if (typeof(TKey) == typeof(ForeignKeyAttribute))
                properties.AddRange(GetKeys<ForeignKeyAttribute>(entityType));

            if (typeof(TKey) == typeof(PrimaryKeyAttribute))
                properties.AddRange(GetKeys<PrimaryKeyAttribute>(entityType));

            if (typeof(TKey) == typeof(CompositePrimaryKeyAttribute))
                properties.AddRange(GetKeys<CompositePrimaryKeyAttribute>(entityType));

            if (!properties.Any())
                return;

            foreach (var property in properties)
            {
                string docType = null;
                var propVal = property.GetValue(entity, null);

                //don't append to a key that is not supplied
                if (propVal == null)
                    continue;

                //get doc type
                if (typeof(TKey) == typeof(PrimaryKeyAttribute))
                    docType = entityType.Name.ToCouchbaseDocyType();

                if (typeof(TKey) == typeof(CompositePrimaryKeyAttribute))
                {
                    var fkAttribute = property.GetCustomAttribute<CompositePrimaryKeyAttribute>(true);

                    docType = fkAttribute.KeyType == null ? String.Empty : fkAttribute.KeyType.Name.ToCouchbaseDocyType();
                }

                if (typeof(TKey) == typeof(ForeignKeyAttribute))
                {
                    var fkAttribute = property.GetCustomAttribute<ForeignKeyAttribute>(true);
                    docType = fkAttribute.KeyType.Name.ToCouchbaseDocyType();
                }

                if (!String.Format("{0}", propVal).ContainsDocType(docType))
                {
                    //it's possible for a key to not have a doctype 
                    property.SetValue(entity, String.Format("{0}", propVal)
                                                .Replace("_", " "));
                    continue;
                }
                    

                property.SetValue(entity, String.Format("{0}", propVal)
                                                .Replace(docType, String.Empty)
                                                .Replace("_", " "));
            }
        }

        //public static string ToPrimaryKeysssss(this IModel entity)
        //{
        //    var entityType = entity.GetType();

        //    var properties = entityType.GetProperties()
        //                         .Where(property => property.GetCustomAttributes(true)
        //                                                    .Where(attr => (attr as IPrimaryKeyAttribute) != null || (attr as IForeignKeyAttribute) != null)
        //                                                    .Any())
        //                         .ToList();

        //    if (!properties.Any())
        //        throw new Exception(String.Format("The {0} or {1} is missing from the couchbase model class {2}", typeof(PrimaryKey).Name,
        //                                                                                                          typeof(ForeignKey).Name,
        //                                                                                                          entityType.Name));
        //    Dictionary<string, string> pks = new Dictionary<string, string>();
        //    Dictionary<int, string> fks = new Dictionary<int, string>();

        //    foreach (var property in properties)
        //    {
        //        var pk = property.GetCustomAttributes(true)
        //                         .Where(attr => (attr as PrimaryKey) != null)
        //                         .FirstOrDefault();

        //        var fk = property.GetCustomAttributes(true)
        //                          .Where(attr => (attr as ForeignKey) != null)
        //                          .Select(i => (i as ForeignKey))
        //                          .FirstOrDefault();

        //        if (pk != null)
        //            pks.Add(property.Name, String.Format("{0}>{1}", entityType.Name.InflectTo().Underscored,
        //                                                            property.GetValue(entity, null)).InflectTo().Underscored.ToLower());
        //        if (fk != null)
        //            fks.Add(fk.Order, String.Format("{0}", property.GetValue(entity, null))
        //                                    .InflectTo()
        //                                    .Underscored
        //                                    .Replace(String.Format("{0}>", fk.KeyType.Name.InflectTo().Underscored.ToLower()), String.Empty));
        //    }

        //    if (pks.Any() && fks.Any())
        //        throw new Exception(String.Format("Model class {2} cannot have both {0} and {1} applied to it", typeof(PrimaryKey).Name,
        //                                                                                                        typeof(ForeignKey).Name,
        //                                                                                                        entityType.Name));
        //    if (pks.Any())
        //        return pks.First().Value;

        //    var orderedFks = fks.OrderBy(i => i.Key);

        //    StringBuilder sb = new StringBuilder();

        //    sb.Append(String.Format("{0}{1}", entityType.Name.InflectTo().Underscored.ToLower(), ">"));

        //    foreach (var fk in orderedFks)
        //    {
        //        sb.Append(String.Format("{0}{1}", fk.Value, ">"));
        //    }

        //    var key = sb.ToString();

        //    //exclude tha last >
        //    return key.Substring(0, key.Length - 2);
        //}
    }
}

using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository.MongoDb
{
    class ObjectJsonSerializer : IBsonSerializer
    {
        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(context.Reader.ReadString());
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            context.Writer.WriteString(Newtonsoft.Json.JsonConvert.SerializeObject(value));
        }

        public Type ValueType
        {
            get { return typeof(object); }
        }
    }
}

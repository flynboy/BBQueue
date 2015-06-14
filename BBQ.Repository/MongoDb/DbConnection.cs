using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBQ.Repository.MongoDb
{
    public class DbConnection
    {
        protected static string _connString { get; set; }
        protected static IMongoClient _client { get; set; }
        protected static IMongoDatabase _db { get; set; }

        static DbConnection()
        {
            // Set up MongoDB conventions
            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("EnumStringConvention", pack, t => true);

            BsonClassMap.RegisterClassMap<Model.Account>(m =>
            {
                m.AutoMap();
                m.MapIdMember(c => c.ID).SetIdGenerator(GuidGenerator.Instance);
                m.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Model.Queue>(m =>
            {
                m.AutoMap();
                m.MapIdMember(c => c.ID).SetIdGenerator(GuidGenerator.Instance);
                m.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Model.Message>(m =>
            {
                m.AutoMap();
                m.MapIdMember(c => c.ID).SetIdGenerator(GuidGenerator.Instance);
                m.GetMemberMap(c => c.Payload).SetSerializer(new ObjectJsonSerializer());
                m.SetIgnoreExtraElements(true);
            });
        }

        public virtual bool Init(Guid AccountID, string ConnectionString = "")
        {
            this.AccountID = AccountID;
            if (string.IsNullOrEmpty(_connString) || _connString != ConnectionString)
            {
                _client = new MongoClient(ConnectionString);
                _db = _client.GetDatabase("BBQ");
                _connString = ConnectionString;
            }

            return true;
        }

        protected Guid AccountID { get; set; }

        protected IMongoDatabase Database
        {
            get
            {
                return _db;
            }
        }

        protected IMongoCollection<BBQ.Model.Account> Accounts
        {
            get
            {
                return Database.GetCollection<BBQ.Model.Account>("Account");
            }
        }

        protected IMongoCollection<BBQ.Model.Queue> Queues
        {
            get
            {
                return Database.GetCollection<BBQ.Model.Queue>("Queue_" + AccountID.ToString());
            }
        }

        protected IMongoCollection<BBQ.Model.Message> Messages
        {
            get
            {
                return Database.GetCollection<BBQ.Model.Message>("Message_" + AccountID.ToString());
            }
        }
    }
}

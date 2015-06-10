﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BBQ.Controllers
{
    [RoutePrefix("Stats")]
    public class StatsController : BaseController
    {
        protected static Dictionary<string,decimal> stats = new Dictionary<string,decimal>();

        public static Decimal getStat(string name)
        {
            if(!stats.ContainsKey(name.ToLower()))
            {
                stats[name.ToLower()]=0M;
                return 0M;
            }
            return stats[name.ToLower()];
        }

        public static void setStat(string name, Decimal value)
        {
            stats[name.ToLower()] = value;
        }

        [HttpGet, Route("")]
        public IEnumerable<Models.Statistic> Get()
        {
            var ret = new List<Models.Statistic>();

            //# of queues
            ret.Add(new Models.Statistic()
                {
                    Name = "QueueCount",
                    Value = QueueRepository.Count
                });

            //messages
            ret.Add(new Models.Statistic()
            {
                Name = "TotalMessages",
                Value = MessageRepository.Count
            });

            return ret;
        }
    }
}

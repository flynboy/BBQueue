using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BBQ;
using BBQ.Controllers;
using BBQ.Model;
using BBQ.Repository;

namespace BBQ.Tests.Controllers
{
    [TestClass]
    public class QueueControllerTestsMemory
    {
        public QueueController Controller
        {
            get
            {
                QueueController controller = new QueueController();
                controller.AccountID = Guid.NewGuid();

                var repo = new BBQ.Repository.Memory.QueueRepository();
                Assert.IsTrue(repo.Init(controller.AccountID));

                var repo2 = new BBQ.Repository.Memory.MessageRepository();
                Assert.IsTrue(repo2.Init(controller.AccountID));

                controller.QueueRepository = repo;
                controller.MessageRepository = repo2;

                return controller;
            }
        }

        [TestMethod]
        public void Get()
        {
            // Arrange
            var controller = Controller;

            //add a couple of queues
            controller.QueueRepository.Add(new Queue() { AccountID = controller.AccountID });
            controller.QueueRepository.Add(new Queue() { AccountID = controller.AccountID });

            // Act
            var result = controller.Get();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void GetById()
        {
            // Arrange
            var controller = Controller;

            //add a couple of queues
            var q = new Queue(controller.AccountID);
            controller.QueueRepository.Add(q);

            // Act
            var result = controller.Get(q.ID);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Post()
        {
            // Arrange
            var controller = Controller;

            // Act
            var q = new Queue();

            Assert.IsNotNull(controller.Add(q));

            // Assert
            var lq = controller.Get(q.ID);
            Assert.IsNotNull(lq);
        }

        [TestMethod]
        public void Put()
        {
            // Arrange
            var controller = Controller;

            var q = new Queue() { Name = "test q1" };
            Assert.IsNotNull(controller.Add(q));

            // Act
            q.Name = "test q2";
            Assert.IsTrue(controller.Save(q.ID, q));

            // Assert
            var q2 = controller.Get(q.ID);
            Assert.AreEqual(q2.Name, "test q2");
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            var controller = Controller;

            var q = new Queue();
            Assert.IsNotNull(controller.Add(q));

            // Act
            Assert.IsTrue(controller.Delete(q.ID));

            // Assert
            Assert.IsNull(controller.Get(q.ID));
        }

        #region messages

        [TestMethod]
        public void AddMessage()
        {
            var controller = Controller;

            var q = new Queue();
            Assert.IsNotNull(controller.Add(q));

            // Act
            var m = controller.AddMessage(q.ID, 42);            

            // Assert
            Assert.IsNotNull(m);
        }

        [TestMethod]
        public void AddMessageComplex()
        {
            var controller = Controller;

            var q = new Queue();
            Assert.IsNotNull(controller.Add(q));

            // Act
            var m = controller.AddMessage(q.ID, q); //heh, we'll add the queue as the message payload

            // Assert
            Assert.IsNotNull(m);
        }

        [TestMethod]
        public void GetMessage()
        {
            var controller = Controller;

            var q = new Queue();
            Assert.IsNotNull(controller.Add(q));

            // Act
            Assert.IsNotNull(controller.AddMessage(q.ID, 42));
            var m = controller.GetNextMessage(q.ID);

            // Assert
            Assert.IsNotNull(m);
            Assert.AreEqual(m.Payload, 42);
        }

        [TestMethod]
        public void RemoveMessage()
        {
            var controller = Controller;

            var q = new Queue();
            Assert.IsNotNull(controller.Add(q));

            // Act
            Assert.IsNotNull(controller.AddMessage(q.ID, 42));
            var m = controller.GetNextMessage(q.ID);
            Assert.IsTrue(controller.RemoveMessage(q.ID, m.ID));

            // Assert
            Assert.IsNull(controller.MessageRepository.Get(m.ID));
        }

        //test message timeouts and locks
        [TestMethod]
        public void message_lock_expiration()
        {
            var controller = Controller;
            controller.SuppressTimeoutProcessing = true; //allow for explicit processing

            var q = new Queue()
                {
                    LockTimeout = 1
                };
            Assert.IsNotNull(controller.Add(q));

            Assert.IsNotNull(controller.AddMessage(q.ID, 42));
            var m = controller.GetNextMessage(q.ID);
            Assert.IsNull(controller.GetNextMessage(q.ID));

            //now wait a second then process timeouts.  this should unlock the item and it should be available
            System.Threading.Thread.Sleep(1500);
            controller.ProcessTimeouts(q.ID);

            var m2 = controller.GetNextMessage(q.ID);
            Assert.IsNotNull(m2);

            Assert.AreEqual(m2.ID, m.ID);
        }

        [TestMethod]
        public void message_expiration()
        {
            var controller = Controller;
            controller.SuppressTimeoutProcessing = true; //allow for explicit processing

            var q = new Queue()
            {
                TimeToLive=1
            };
            Assert.IsNotNull(controller.Add(q));

            Assert.IsNull(controller.QueueRepository.GetByName("SYSTEM_EXPIRED"));

            Assert.IsNotNull(controller.AddMessage(q.ID, 42));

            //now wait a second then process timeouts.  this should expire the item and put it in the SYSTEM_EXPIRED queue
            System.Threading.Thread.Sleep(1500);
            controller.ProcessTimeouts(q.ID);
            
            Assert.IsNull(controller.GetNextMessage(q.ID));

            var expired_queue = controller.QueueRepository.GetByName("SYSTEM_EXPIRED");
            Assert.AreEqual(controller.MessageRepository.Count(expired_queue.ID), 1); 
        }

        [TestMethod]
        public void message_retry_exceeded()
        {
            var controller = Controller;
            controller.SuppressTimeoutProcessing = true; //allow for explicit processing

            var q = new Queue()
            {
                MaxAttempts = 0,
                LockTimeout = 1,
            };
            Assert.IsNotNull(controller.Add(q));

            Assert.IsNull(controller.QueueRepository.GetByName("SYSTEM_POISON"));

            Assert.IsNotNull(controller.AddMessage(q.ID, 42));
            Assert.IsNotNull(controller.GetNextMessage(q.ID));

            //now wait a second then process timeouts.  this should expire the item and put it in the SYSTEM_EXPIRED queue
            System.Threading.Thread.Sleep(1500);
            controller.ProcessTimeouts(q.ID);

            Assert.IsNull(controller.GetNextMessage(q.ID));

            var poison_queue = controller.QueueRepository.GetByName("SYSTEM_POISON");
            Assert.AreEqual(controller.MessageRepository.Count(poison_queue.ID), 1);
        }

        #endregion
    }
}

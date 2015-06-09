using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using QBert;
using QBert.Controllers;
using QBert.Model;
using QBert.Repository;

namespace QBert.Tests.Controllers
{
    [TestClass]
    public class QueueControllerTests
    {
        public QueueController Controller
        {
            get
            {
                QueueController controller = new QueueController();
                controller.AccountID = Guid.NewGuid();

                var repo = new QBert.Repository.Memory.QueueRepository();
                Assert.IsTrue(repo.Init(controller.AccountID));

                controller.QueueRepository = repo;

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

        //[TestMethod]
        //public void Put()
        //{
        //    // Arrange
        //    QueueController controller = new QueueController();

        //    // Act
        //    controller.Put(5, "value");

        //    // Assert
        //}

        //[TestMethod]
        //public void Delete()
        //{
        //    // Arrange
        //    QueueController controller = new QueueController();

        //    // Act
        //    controller.Delete(5);

        //    // Assert
        //}
    }
}

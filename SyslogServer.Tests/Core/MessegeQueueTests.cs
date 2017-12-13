using Moq;
using NUnit.Framework;
using SyslogServer.Core;
using SyslogServer.Core.Model;
using SyslogServer.Core.Storage;
using System;
using System.Collections.Generic;
using global::SyslogServer.Core.Protocol;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.UnitTests.Core
{
    [TestFixture]
    public class MessegeQueueTests : TestBase<MessageQueue>
    {
        Mock<IStorage> mockIStorageWrapper;
        ISyslogMessage syslogMessage;


        [SetUp]
        public override void SetUp()
        {
            syslogMessage = new SyslogMessage("test-machine", "test-facility", Severity.Debug, DateTime.Now, "test-tag", "test-message");
            base.SetUp();
        }

        [Test]
        public void Enqueue_Always_AddsToInternalCollection()
        {
            SystemUnderTest.Enqueue(syslogMessage);
            VerifyAllMocks();
            Assert.That(SystemUnderTest.GetHosts().Count, Is.EqualTo(1));
        }

        [Test]
        public void MessageLimitAndFlushNum_Always_Respected()
        {
            mockIStorageWrapper.Setup(s => s.Save(It.IsAny<IEnumerable<ISyslogMessage>>())).Verifiable();
            SystemUnderTest.Enqueue(syslogMessage);
            SystemUnderTest.Enqueue(syslogMessage);
            SystemUnderTest.Enqueue(syslogMessage);
            SystemUnderTest.Enqueue(syslogMessage);
            Assert.That(SystemUnderTest.Count, Is.EqualTo(2));
        }

        protected override void ConfigureConstructorMocks()
        {
        }

        protected override void CreateMocks()
        {
            mockIStorageWrapper = CreateMock<IStorage>();
        }

        protected override MessageQueue CreateSystemUnderTest()
        {
            return new MessageQueue(messageLimit: 2, flushNum: 1, flushInterval: 1, storageHandlers: new List<IStorage>() { mockIStorageWrapper.Object });
        }
    }
}

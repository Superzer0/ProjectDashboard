using System.Web.Http;
using System.Web.Http.Results;
using Autofac.Extras.Moq;
using Common.Logging;
using NUnit.Framework;

namespace UnitTests.Utils
{
    public abstract class BaseTestFixture
    {
        protected AutoMock AutoMock { get; set; }
        protected InMemoryLogger DummyLogger { get; set; }

        [SetUp]
        public virtual void SetUp()
        {
            AutoMock = AutoMock.GetLoose();
            DummyLogger = InMemoryLoggingAdapterFactory.CreateDummyLogger();
            LogManager.Adapter = new InMemoryLoggingAdapterFactory(DummyLogger);
        }

        [TearDown]
        public virtual void TearDown()
        {
            AutoMock.Dispose();
        }

        protected void AssertBadRequestMessage(IHttpActionResult httpActionResult, string message)
        {
            Assert.That(httpActionResult, Is.TypeOf<BadRequestErrorMessageResult>());
            var badActionResult = (BadRequestErrorMessageResult)httpActionResult;
            Assert.That(badActionResult.Message, Is.EqualTo(message));
        }

    }
}

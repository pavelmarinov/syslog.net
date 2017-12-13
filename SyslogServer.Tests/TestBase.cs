using Moq;
using NUnit.Framework;

namespace SyslogServer.UnitTests
{

    [TestFixture]
    public abstract class TestBase<T> where T : class
    {
        MockRepository mockRepository;

        [Test]
        public void Constructor_Always_PerformsExpectedWork()
        {
            VerifyAllMocks();
        }

        [SetUp]
        public virtual void SetUp()
        {
            CreateMockRepository();
            CreateMocks();
            ConfigureConstructorMocks();
            SystemUnderTest = CreateSystemUnderTest();
        }

        protected T SystemUnderTest { get; private set; }

        protected abstract void ConfigureConstructorMocks();

        protected Mock<TMock> CreateMock<TMock>() where TMock : class
        {
            return mockRepository.Create<TMock>();
        }

        protected Mock<TMock> CreateMock<TMock>(params object[] args) where TMock : class
        {
            return mockRepository.Create<TMock>(args);
        }

        protected abstract void CreateMocks();

        protected abstract T CreateSystemUnderTest();

        protected void VerifyAllMocks()
        {
            mockRepository.VerifyAll();
        }

        void CreateMockRepository()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
        }
    }
}
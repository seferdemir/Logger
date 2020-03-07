using Logger.Data;
using NUnit.Framework;

namespace LoggerUnitTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var context = new LoggerContext();
            var log = context.EventLogs.Find(1);
        }
    }
}
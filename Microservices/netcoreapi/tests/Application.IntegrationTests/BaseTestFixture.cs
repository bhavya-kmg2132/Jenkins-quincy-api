using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Application.IntegrationTests
{
    using static Testing;


    [TestFixture]
    public class BaseTestFixture
    {
        public Random rnd { get; set; }

        [SetUp]
        public async Task TestSetUp()
        {
            rnd = new Random();
            await ResetState();
            await RunAsDefaultUserAsync();
        }
    }
}

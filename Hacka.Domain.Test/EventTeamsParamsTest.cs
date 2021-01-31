using Xunit;

namespace Hacka.Domain.Test
{
    public class EventTeamsParamsTest
    {
        [Fact]
        public void Should_Instantiate_With_Success()
        {
            var instance = new EventTeamsParams()
            {
                host = "host",
                problemName = "problema",
                severity = "critical",
                value = 1,
            };

            Assert.IsType<EventTeamsParams>(instance);
            Assert.NotNull(instance);

            Assert.IsType<string>(instance.host);
            Assert.IsType<string>(instance.problemName);
            Assert.IsType<string>(instance.severity);
            Assert.IsType<int>(instance.value);
        }
    }
}

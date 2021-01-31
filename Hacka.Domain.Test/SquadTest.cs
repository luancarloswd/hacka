using Xunit;

namespace Hacka.Domain.Test
{
    public class SquadTest
    {
        [Fact]
        public void Should_Instantiate_With_Success()
        {
            var instance = new Squad()
            {
                Id = 0,
                ChannelTeams = "test",
                Name = "test"
            };

            Assert.IsType<Squad>(instance);
            Assert.NotNull(instance);

            Assert.IsType<long>(instance.Id);
            Assert.IsType<string>(instance.ChannelTeams);
            Assert.IsType<string>(instance.Name);
        }
    }
}

using Urho3DNet;
using Xunit;

namespace RbfxTemplate.Tests
{
    /// <summary>
    /// Sample set of unit tests.
    /// Replace this with your own unit tests.
    /// </summary>
    public class SampleUnitTestFixture
    {
        [Fact]
        public async Task SampleSceneTest()
        {
            await RbfxTestFramework.ToMainThreadAsync();

            SharedPtr<Scene> scene = new Scene(RbfxTestFramework.Context);

            Assert.Equal(0u, scene.Ptr.GetNumChildren());
        }

        [Fact]
        public async Task SampleEventTest()
        {
            await RbfxTestFramework.ToMainThreadAsync();

            SharedPtr<Scene> scene = new Scene(RbfxTestFramework.Context);
            scene.Ptr.SubscribeToEvent("TestEvent", scene.Ptr, map => { });
            scene.Ptr.SendEvent("TestEvent");
        }

        [Fact]
        public async Task SampleExceptionTest()
        {
            async Task TestMethod()
            {
                await RbfxTestFramework.ToMainThreadAsync();
                throw new ArgumentException("Expected exception");
            }

            await Assert.ThrowsAsync<ArgumentException>(TestMethod);
        }

        [Fact]
        public async Task SampleFailingTest()
        {
            await RbfxTestFramework.ToMainThreadAsync();

            Assert.True(false);
        }
    }
}

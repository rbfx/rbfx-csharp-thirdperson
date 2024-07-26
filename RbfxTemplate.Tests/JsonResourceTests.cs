// Copyright (c) 2024-2024 the rbfx project.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT> or the accompanying LICENSE file.

using RbfxTemplate.Tests;
using RbfxTemplate.Utils;
using Xunit;

namespace Urho3DNet.Tests
{
    public enum TestEnum
    {
        A,B,C
    }
    public class TestContainer
    {
        public Vector2 Vector2 { get; set; }
        public Vector3 Vector3 { get; set; }
        public Vector4 Vector4 { get; set; }
        public Quaternion Quaternion { get; set; }
        public IntVector2 IntVector2 { get; set; }
        public IntVector3 IntVector3 { get; set; }
        public BoundingBox BoundingBox { get; set; }
        public TestEnum Enum { get; set; }
        public Material? Material { get; set; }

        public TestContainer()
        {
        }

        public TestContainer(Material mat)
        {
            Vector2 = new Vector2(1, 2);
            Vector3 = new Vector3(1, 2, 3);
            Vector4 = new Vector4(1, 2, 3, 4);
            Quaternion = new Quaternion(10, Vector3.Down);
            IntVector2 = new IntVector2(1, 2);
            IntVector3 = new IntVector3(1, 2, 3);
            BoundingBox = new BoundingBox(new Vector3(-1, -2, -3), new Vector3(1, 2, 3));
            Enum = TestEnum.B;
            Material = mat;
        }
    }

    [ObjectFactory]
    public partial class TestResource : JsonResource<TestContainer>
    {
        public TestResource(Context context) : base(context)
        {
        }
    }
    public class JsonResourceTests
    {
        [Fact]
        public async Task SaveSimpleResource()
        {
            await RbfxTestFramework.Context.ToMainThreadAsync();

            var mat = new Material(RbfxTestFramework.Context);
            mat.Name = "TestResource.material";
            RbfxTestFramework.Context.ResourceCache.AddManualResource(mat);

            var res = new TestResource(RbfxTestFramework.Context);
            var testContainer = new TestContainer(mat);

            res.Value = testContainer;
            var fileIdentifier = new FileIdentifier("conf", "SaveJson.json");
            res.SaveFile(fileIdentifier);
            res.Value = default;
            res.LoadFile(fileIdentifier);
            Assert.NotNull(res.Value);
            Assert.Equal(testContainer.IntVector2, res.Value.IntVector2);
            Assert.Equal(testContainer.IntVector3, res.Value.IntVector3);
            Assert.Equal(testContainer.Vector2, res.Value.Vector2);
            Assert.Equal(testContainer.Vector3, res.Value.Vector3);
            Assert.Equal(testContainer.Vector4, res.Value.Vector4);
            Assert.Equal(testContainer.Quaternion, res.Value.Quaternion);
            Assert.Equal(testContainer.BoundingBox, res.Value.BoundingBox);
            Assert.Equal(testContainer.Enum, res.Value.Enum);
            Assert.Equal(testContainer.Material, res.Value.Material);
            var path = RbfxTestFramework.Context.VirtualFileSystem.GetAbsoluteNameFromIdentifier(fileIdentifier);
            System.IO.File.Delete(path);
        }

        [Fact]
        public async Task SaveConfigResource()
        {
            await RbfxTestFramework.Context.ToMainThreadAsync();

            var mat = new Material(RbfxTestFramework.Context);
            mat.Name = "TestResource2.material";
            RbfxTestFramework.Context.ResourceCache.AddManualResource(mat);

            using var conf1 = new ConfigFileContainer<TestContainer>(RbfxTestFramework.Context);
            conf1.Value = new TestContainer(mat);
            conf1.SaveConfig();

            using var conf2 = ConfigFileContainer<TestContainer>.LoadConfig(RbfxTestFramework.Context);

            Assert.Equal(conf1.Value.IntVector2, conf2.Ptr.Value.IntVector2);
            Assert.Equal(conf1.Value.IntVector3, conf2.Ptr.Value.IntVector3);
            Assert.Equal(conf1.Value.Vector2, conf2.Ptr.Value.Vector2);
            Assert.Equal(conf1.Value.Vector3, conf2.Ptr.Value.Vector3);
            Assert.Equal(conf1.Value.Vector4, conf2.Ptr.Value.Vector4);
            Assert.Equal(conf1.Value.Quaternion, conf2.Ptr.Value.Quaternion);
            Assert.Equal(conf1.Value.BoundingBox, conf2.Ptr.Value.BoundingBox);
            Assert.Equal(conf1.Value.Enum, conf2.Ptr.Value.Enum);
            Assert.Equal(conf1.Value.Material, conf2.Ptr.Value.Material);
            var path = RbfxTestFramework.Context.VirtualFileSystem.GetAbsoluteNameFromIdentifier(new FileIdentifier("conf", conf2.Ptr.Name));
            System.IO.File.Delete(path);
        }
    }
}

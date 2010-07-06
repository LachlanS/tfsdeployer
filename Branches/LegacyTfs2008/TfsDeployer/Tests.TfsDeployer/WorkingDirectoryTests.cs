using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsDeployer;

namespace Tests.TfsDeployer
{
    [TestClass]
    public class WorkingDirectoryTests
    {
        [TestMethod]
        public void Should_return_empty_folder()
        {
            using (var workingDirectory = new WorkingDirectory())
            {
                var childCount = workingDirectory.DirectoryInfo.GetFileSystemInfos().Length;
                Assert.AreEqual(0, childCount);
            }
        }

        [TestMethod]
        public void Should_remove_folder_and_nested_readonly_contents_after_dispose()
        {
            DirectoryInfo info;
            using (var workingDirectory = new WorkingDirectory())
            {
                info = workingDirectory.DirectoryInfo;
                var subDir = new DirectoryInfo(Path.Combine(info.FullName, "subDir"));
                subDir.Create();
                var testFile = new FileInfo(Path.Combine(subDir.FullName, "test.txt"));
                using (var writer = File.CreateText(testFile.FullName))
                {
                    writer.Write("hello");
                }
                testFile.IsReadOnly = true;
            }
            Assert.IsFalse(info.Exists);
        }
    }
}

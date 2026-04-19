using System.IO;
using NUnit.Framework;

namespace EmberCrpg.Tests.EditMode.Core
{
    public sealed class AsmdefBoundaryTests
    {
        [Test]
        public void DomainDoesNotContainUnityEngineUsage()
        {
            AssertFolderDoesNotContain("Assets/Scripts/Domain", "UnityEngine");
        }

        [Test]
        public void SimulationDoesNotContainUnityEngineUsage()
        {
            AssertFolderDoesNotContain("Assets/Scripts/Simulation", "UnityEngine");
        }

        private static void AssertFolderDoesNotContain(string folder, string forbiddenText)
        {
            var projectRoot = Directory.GetCurrentDirectory();
            var absoluteFolder = Path.Combine(projectRoot, folder);

            foreach (var file in Directory.GetFiles(absoluteFolder, "*.cs", SearchOption.AllDirectories))
            {
                var text = File.ReadAllText(file);
                Assert.IsFalse(
                    text.Contains(forbiddenText),
                    $"{file} must not contain {forbiddenText}.");
            }
        }
    }
}
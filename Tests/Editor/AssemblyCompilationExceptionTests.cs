using NUnit.Framework;
using Unity.ProjectAuditor.Editor;
using Unity.ProjectAuditor.Editor.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityEditor.ProjectAuditor.EditorTests
{
    public class AssemblyCompilationExceptionTests
    {
        private ScriptResource m_ScriptResource;

        [OneTimeSetUp]
        public void SetUp()
        {
            m_ScriptResource = new ScriptResource("MyClass.cs", @"
class MyClass {
#if !UNITY_EDITOR
    asd
#endif
}
");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            m_ScriptResource.Delete();
        }

        [Test]
        [ExplicitAttribute]
        public void ExceptionIsThrownOnCompilationError()
        {
            LogAssert.ignoreFailingMessages = true;

            var exceptionThrown = false;
            try
            {
                using (var compilationHelper = new AssemblyCompilationHelper())
                {
                    compilationHelper.Compile();
                }
            }
            catch (AssemblyCompilationException)
            {
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);

            LogAssert.Expect(LogType.Error,
                "Assets/ProjectAuditor-Temp/MyClass.cs(6,1): error CS1519: Invalid token '}' in class, struct, or interface member declaration");
            LogAssert.Expect(LogType.Error, "Failed to compile player scripts");
            LogAssert.ignoreFailingMessages = false;
        }
    }
}

using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SortComponents.SortComponents.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace SortComponents.SortComponents.Tests.Editor
{
    public class TestSortComponents
    {
        [UnityTest]
        public IEnumerator TestSort()
        {
            var path = GetScenePath("TestSort");
            Assert.True(!string.IsNullOrEmpty(path));

            EditorSceneManager.OpenScene(path);
            var test = Object.FindObjectsOfType<Transform>()
                .FirstOrDefault(t => CompareString(t.name, "test"));

            Assert.NotNull(test);
            var sorter = new Sorter();
            var g = test.gameObject;

            var expected = sorter.GetComponents(g)
                .OrderByDescending(c => sorter.IsEngineComponent(c))
                .ThenBy(c => c.GetType().Name)
                .ToArray();

            sorter.Sort(g);

            var sorted = sorter.GetComponents(g);
            Assert.True(expected.Length == sorted.Length);
            
            for (int i = 0; i < expected.Length; i++)
            {
                var e = expected[i];
                var s = sorted[i];
                Assert.True(string.Equals(e.GetType().Name, s.GetType().Name));
            }

            yield return null;
        }

        public static string GetScenePath(string name)
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                var sceneName = Path.GetFileNameWithoutExtension(scene.path);
                if (CompareString(name, sceneName))
                {
                    return scene.path;
                }
            }

            return null;
        }

        private static bool CompareString(string name, string sceneName)
        {
            return string.Equals(sceneName, name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
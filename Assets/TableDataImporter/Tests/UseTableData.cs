using UnityEditor;
using NUnit.Framework;

namespace Tests {
    public class UseTableData
    {
        private TableDataImporter.TableData data;

        [OneTimeSetUp]
        public void Setup() {
            data = AssetDatabase.LoadAssetAtPath<TableDataImporter.TableData>("Assets/TableDataImporter/Tests/example.asset");
            Assert.IsNotNull(data);
        }

        [Test]
        public void Test() {
            var table = data.GetTable("BNpc");
            Assert.IsNotNull(table);
            {
                var entry = table.GetEntry("slime");
                Assert.IsNotNull(entry);
                Assert.AreEqual("Slime", entry.GetString("name"));
                Assert.AreEqual(5, entry.GetInt("hp"));
            }
            {
                var entry = table.GetEntry("goblin");
                Assert.IsNotNull(entry);
                Assert.AreEqual("Goblin", entry.GetString("name"));
                Assert.AreEqual(10, entry.GetInt("hp"));
            }
        }

        [Test]
        public void TestNull() {
            Assert.IsNull(data.GetTable("Foobar"));
        }
    }
}

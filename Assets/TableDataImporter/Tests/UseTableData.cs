using UnityEditor;
using NUnit.Framework;

namespace Tests {
    public class UseTableData {
        private TableDataImporter.TableData data;

        [OneTimeSetUp]
        public void Setup() {
            data = AssetDatabase.LoadAssetAtPath<TableDataImporter.TableData>("Assets/TableDataImporter/Tests/example.asset");
            Assert.IsNotNull(data);
        }

        [Test]
        public void TestNull() {
            Assert.IsNull(data.GetTable("Foobar"));
        }

        [Test]
        public void Test() {
            var table = data.GetTable("BNpc");
            Assert.IsNotNull(table);
            {
                var entry = table.GetEntry("slime");
                Assert.IsNotNull(entry);
                Assert.AreEqual("Bubbly Slime", entry.GetString("name"));
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
        public void TestMultiTag() {
            var table = data.GetTable("BNpc");
            {
                var entry = table.GetEntry("slime");
                Assert.IsNotNull(entry);
                Assert.AreEqual("Bubbly Slime", entry.GetString("name"));
                Assert.AreEqual("Bubbly Slime", entry.GetString("name", 0));
                Assert.AreEqual("Slime", entry.GetString("name", 1));
            }
        }

        [Test]
        public void TestMultiEntry() {
            var table = data.GetTable("AI");
            Assert.IsNotNull(table);
            Assert.AreEqual(2, table.GetCount("goblin"));
            string[] ACTIONS = {
                "Attack",
                "Goblin Punch",
            };
            for (int i = 0, n = table.GetCount("goblin").Value; i < n; ++i) {
                var entry = table.GetEntry("goblin", i);
                Assert.IsNotNull(entry);
                Assert.AreEqual(ACTIONS[i], entry.GetString("action"));
            }
        }
    }
}

using System;
using System.Linq;
using com.csutil.model;
using UltraLiteDB;
using Xunit;

namespace com.csutil.tests.model {

    public class LiteDbInheritanceTests {

        private class ClassA {
            public string id { get; set; }
            public string a { get; set; }
        }

        private class ClassA1 : ClassA { public string name { get; set; } }

        private class ClassA2 : ClassA { public int age { get; set; } }

        [Fact]
        void ExampleUsage1() {

            var dbFile = EnvironmentV2.instance.GetOrAddTempFolder("tests.io.db").GetChild("TestDB_" + NewId());

            // Open database (or create if doesn't exist)
            using (var db = new UltraLiteDatabase(dbFile.OpenOrCreateForReadWrite(), disposeStream: true)) {

                var elems = db.GetCollection<ClassA>("users");
                {
                    var ele = new ClassA { id = NewId(), a = "abc" };
                    elems.Insert(ele);
                    Assert.Equal("abc", elems.FindById(ele.id).a);
                }
                {
                    var ele = new ClassA1 { id = NewId(), a = "abc", name = "A1" };
                    elems.Insert(ele);
                    var r = elems.FindById(ele.id) as ClassA1;
                    Assert.Equal("abc", r.a);
                    Assert.Equal("A1", r.name);
                }
                {
                    var ele = new ClassA2 { id = NewId(), a = "abc", age = 99 };
                    elems.Insert(ele);
                    var r = elems.FindById(ele.id) as ClassA2;
                    Assert.Equal("abc", r.a);
                    Assert.Equal(99, r.age);
                }

            }
            Assert.True(dbFile.IsNotNullAndExists());
            dbFile.DeleteV2(); // cleanup after the test
        }

        private static string NewId() { return GuidV2.NewGuid().ToString(); }

    }

}
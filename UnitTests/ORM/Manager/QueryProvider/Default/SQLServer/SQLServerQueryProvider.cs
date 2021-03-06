﻿/*
Copyright (c) 2014 <a href="http://www.gutgames.com">James Craig</a>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

using System.Linq;
using Utilities.ORM.BaseClasses;
using Utilities.ORM.Interfaces;
using Utilities.ORM.Manager.Mapper.Interfaces;
using Utilities.ORM.Manager.QueryProvider.Interfaces;
using Xunit;

namespace UnitTests.ORM.Manager.QueryProvider.Default.SQLServer
{
    public class SQLServerQueryProvider : DatabaseBaseClass
    {
        [Fact]
        public void Batch()
        {
            var Temp = new Utilities.ORM.Manager.QueryProvider.Default.SQLServer.SQLServerQueryProvider();
            var Batch = Temp.Batch(TestDatabaseSource);
            Assert.Equal(0, Batch.CommandCount);
            Assert.Equal(0, Batch.Execute().First().Count());
            Assert.Equal(typeof(Utilities.ORM.Manager.QueryProvider.Default.DatabaseBatch), Batch.GetType());
        }

        [Fact]
        public void Create()
        {
            var Temp = new Utilities.ORM.Manager.QueryProvider.Default.SQLServer.SQLServerQueryProvider();
            Assert.NotNull(Temp);
            Assert.Equal("System.Data.SqlClient", Temp.ProviderName);
        }

        [Fact]
        public void Generate()
        {
            var Temp = new Utilities.ORM.Manager.QueryProvider.Default.SQLServer.SQLServerQueryProvider();
            var Generator = Temp.Generate<TestClass>(TestDatabaseSource, new Utilities.ORM.Manager.Mapper.Manager(Utilities.IoC.Manager.Bootstrapper.ResolveAll<IMapping>())[typeof(TestClass), TestDatabaseSource], null);
            Assert.Equal(typeof(Utilities.ORM.Manager.QueryProvider.Default.SQLServer.SQLServerGenerator<TestClass>), Generator.GetType());
        }

        public class TestClass
        {
            public int ID { get; set; }
        }

        public class TestClassDatabase : IDatabase
        {
            public bool Audit
            {
                get { return false; }
            }

            public string Name
            {
                get { return "Data Source=localhost;Initial Catalog=TestDatabase2;Integrated Security=SSPI;Pooling=false"; }
            }

            public int Order
            {
                get { return 0; }
            }

            public bool Readable
            {
                get { return true; }
            }

            public bool Update
            {
                get { return false; }
            }

            public bool Writable
            {
                get { return false; }
            }
        }

        public class TestClassMapping : MappingBaseClass<TestClass, TestClassDatabase>
        {
            public TestClassMapping()
            {
                ID(x => x.ID).SetFieldName("ID").SetAutoIncrement();
            }
        }
    }
}
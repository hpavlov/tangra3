using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tangra.Model.Helpers;

namespace Tangra.Tests.Helpers
{
    [TestFixture]
    public class TestCommandLineParser
    {
        [Test]
        public void Test1()
        {
            var res = CommandLineParser.Parse(new[] {"-a", "-F", "Boza Mazna", "-k", "-d", "23", "-r"});
            Assert.IsTrue(res.ContainsKey("a"));
            Assert.IsTrue(res.ContainsKey("f"));
            Assert.AreEqual("Boza Mazna", res["f"]);

            Assert.IsTrue(res.ContainsKey("k"));
            Assert.IsTrue(res.ContainsKey("d"));
            Assert.AreEqual("23", res["d"]);

            Assert.IsTrue(res.ContainsKey("r"));
        }
    }
}

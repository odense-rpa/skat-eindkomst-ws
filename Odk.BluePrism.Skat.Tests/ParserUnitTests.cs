using Microsoft.VisualStudio.TestTools.UnitTesting;
using Odk.BluePrism.Skat.Parsers;
using System;

namespace Odk.BluePrism.Skat.Tests
{
    [TestClass]
    public class ParserUnitTests
    {
        private eIndkomstParser parser;

        [TestInitialize]
        public void Initialize()
        {
            parser = new eIndkomstParser();
        }

        [TestMethod]
        public void TestNullArgumentThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var result = parser.ParseResultToDataTable(null);
            });

        }
    }
}

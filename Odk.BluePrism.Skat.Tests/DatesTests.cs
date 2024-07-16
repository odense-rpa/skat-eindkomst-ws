using Microsoft.VisualStudio.TestTools.UnitTesting;
using Odk.BluePrism.Skat.Utils;
using System;

namespace Odk.BluePrism.Skat.Tests
{
    [TestClass]
    public class DatesTests
    {

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void TestBasisMonthFromDate()
        {
            Assert.AreEqual("202311", Dates.GetBasisMonthFromDate(DateTime.Parse("1/11/2023")));
        }

        [TestMethod]
        public void TestBasisMonthFromDate2()
        {
            Assert.AreEqual("202301", Dates.GetBasisMonthFromDate(DateTime.Parse("1/1/2023")));
        }

        [TestMethod]
        public void TestBasisMonthFromDate3()
        {
            Assert.AreEqual("202304", Dates.GetBasisMonthFromDate(DateTime.Parse("1/4/2023")));
        }

        [TestMethod]
        public void TestBasisMonthFromDate4()
        {
            Assert.AreEqual("202301", Dates.GetBasisMonthFromDate(DateTime.Parse("4/1/2023")));
        }

        [TestMethod]
        public void TestBasisMonthFromDate5()
        {
            Assert.AreEqual("202301", Dates.GetBasisMonthFromDate(DateTime.Parse("11/1/2023")));
        }

        [TestMethod]
        public void TestBasisMonthFromDate6()
        {
            Assert.AreEqual("202301", Dates.GetBasisMonthFromDate(DateTime.Parse("2/1/2023")));
        }

        [TestMethod]
        public void TestBasisMonthFromDateMinvalueTest()
        {
            Assert.AreEqual("000101", Dates.GetBasisMonthFromDate(DateTime.MinValue));
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rogowski_Hierarchy_Project;
using System.Data;
using System.IO;

namespace CompanyTest
{
    /// <summary>
    /// Unit Tests for Company class methods
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        static Company company;

        [ClassInitialize()]
        public static void UnitTestsInitialize(TestContext testContext)
        {
            company = new Company();
            company.DeleteAllData();
            company.AddSampleData();
        }

        [ClassCleanup()]
        public static void UnitTestsCleanup()
        {
            company.DeleteAllData();
        }


        [TestMethod]
        public void AA_LoadAllEmployeesTest_ShouldHaveCount11()
        {
            //'AA_' is added at the beginning of the test name
            //because of order execution violation
            //that may give wrong results
            company.LoadAllEmployees();
            Assert.AreEqual(11, company.GetEmployeeList().Count);
        }

        [TestMethod]
        public void AddNewEmployeeTest_Right()
        {
            company.AddNewEmployee("/5/", "Katy Perry", "test", 2008, 10000);
            company.LoadAllEmployees();
            Assert.AreEqual(12, company.GetEmployeeList().Count);
            Assert.AreEqual("Katy Perry", company.GetEmployeeList()[11].Name);
            Assert.AreEqual("test", company.GetEmployeeList()[11].Position);
            Assert.AreEqual(2008, company.GetEmployeeList()[11].Start_Year);
            Assert.AreEqual(10000, company.GetEmployeeList()[11].Salary);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Data.SqlClient.SqlException))]
        public void AddNewEmployeeTest_ShouldThrowSqlException()
        {
            company.AddNewEmployee("/5", "Katy Perry", "test", 2008, 10000);
        }

        [TestMethod]
        public void Z_DeleteEmployeeTest()
        {
            //'Z_' is added at the beginning of the test name
            //because of order execution violation
            //that may give wrong results
            company.DeleteEmployee("/5/");
            company.LoadAllEmployees();
            Assert.AreEqual(11, company.GetEmployeeList().Count);
        }
        
        [TestMethod]
        public void DeleteEmployeeTest_ShouldThrowExceptionMessage()
        {
            try
            {
                company.DeleteEmployee("/89/");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Such employee does not exist", e.Message);
            }
        }

        [TestMethod]
        public void FindEmployeeTest_ExistingEmployee()
        {
            company.FindEmployee("/1/");
            Assert.AreEqual(1, company.GetEmployeeList().Count);
            Assert.AreEqual("Billie Eilish", company.GetEmployeeList()[0].Name);
            Assert.AreEqual("Financial Manager", company.GetEmployeeList()[0].Position);
            Assert.AreEqual(2015, company.GetEmployeeList()[0].Start_Year);
            Assert.AreEqual(8000, company.GetEmployeeList()[0].Salary);
        }

        [TestMethod]
        public void FindEmployeeTest_NonExistingEmployee()
        {
            company.FindEmployee("/11/");
            Assert.AreEqual(0, company.GetEmployeeList().Count);
        }

        [TestMethod]
        public void FindEmployeeWithSubordinatesTest_ExistingEmployee()
        {
            company.FindEmployeeWithSubordinates("/4/");
            Assert.AreEqual(3, company.GetEmployeeList().Count);
        }

        [TestMethod]
        public void FindEmployeeWithSubordinatesTest_NonExistingEmployee()
        {
            company.FindEmployeeWithSubordinates("/42/");
            Assert.AreEqual(0, company.GetEmployeeList().Count);
        }

        [TestMethod]
        public void DisplayStatisticsForPathTest()
        {
            //redirection console output to the variable
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                company.DisplayStatisticsForPath("/4/");
                string expected = string.Format(
                    "Minimum salary: 4000\nMaximum salary: 7000\nAverage salary: 5666,67{0}",
                    Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void DisplayStatisticsForLevelTest()
        {
            //redirection console output to the variable
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                company.DisplayStatisticsForLevel(0);
                string expected = string.Format(
                    "Minimum salary: 13000\nMaximum salary: 13000\nAverage salary: 13000{0}",
                    Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void DisplayEmployeesWorkingForTest_ShouldHaveCount1()
        {
            company.DisplayEmployeesWorkingFor('=', 11);
            Assert.AreEqual(1, company.GetEmployeeList().Count);
        }

        [TestMethod]
        public void DisplayEmployeesWorkingForTest_ShouldHaveCount3()
        {
            company.DisplayEmployeesWorkingFor('<', 6);
            Assert.AreEqual(3, company.GetEmployeeList().Count);
        }

        [TestMethod]
        public void DisplayEmployeesWorkingForTest_ShouldHaveCount0()
        {
            company.DisplayEmployeesWorkingFor('>', 20);
            Assert.AreEqual(0, company.GetEmployeeList().Count);
        }
    }
}

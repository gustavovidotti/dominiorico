using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Queries;
using PaymentContext.Domain.ValueObjects;

namespace PaymentContext.Tests.Queries
{
    [TestClass]
    public class StudentQueriesTests
    {
        private IList<Student> _students;

        public StudentQueriesTests()
        {
            _students = new List<Student>();
            for (int i = 0; i < 10; i++)
            {
                _students.Add(
                    new Student(new Name("Aluno", i.ToString()),
                    new Document("1111111111" + 1.ToString(), EDocumentType.CPF),
                    new Email(i.ToString() + "@tste.com")));
            }
        }

        [TestMethod]
        public void SholdReturnNullWhenDocumentNotExist()
        {
            var exp = StudentQueries.GetStudentInfo("123");
            var student = _students.AsQueryable().Where(exp).FirstOrDefault();
            Assert.AreEqual(null, student);
        }

        [TestMethod]
        public void SholdReturnStudentWhenDocumentExist()
        {
            var exp = StudentQueries.GetStudentInfo("11111111111");
            var student = _students.AsQueryable().Where(exp).FirstOrDefault();
            Assert.AreNotEqual(null, student);
        }
    }
}
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.ValueObjects;

namespace PaymentContext.Tests
{
    [TestClass]
    public class StudentTests
    {
        private readonly Name _name;
        private readonly Email _email;
        private readonly Document _document;
        private readonly Address _address;
        private readonly Student _student;

        public StudentTests()
        {
            _name = new Name("Gustavo", "Vidotti");
            _document = new Document("34225545806", EDocumentType.CPF);
            _email = new Email("teste@teste.com");
            _address = new Address("Rua", "1", "Bairro", "Cidade", "Estado", "Pais", "CEP");
            _student = new Student(_name, _document, _email);

        }

        [TestMethod]
        public void AdicionarAssinatura()
        {
            var subscription = new Subscription(null);
            var student = new Student(new Name("Gustavo", "Redoan"), new Document("123456789", EDocumentType.CPF), new Email("gustavo.vidotti@gmail.com"));
            student.AddSubscription(subscription);
        }

        [TestMethod]
        public void ShouldReturnErrorWhenHadActiveSubscription()
        {
            var payment = new PayPalPayment("123", DateTime.Now, DateTime.Now.AddDays(5), 10, 10, _document, "Gustavo", _address, _email);
            var subscription = new Subscription(null);

            subscription.AddPayment(payment);
            _student.AddSubscription(subscription);
            _student.AddSubscription(subscription);
            Assert.IsTrue(_student.Invalid);
        }

        [TestMethod]
        public void ShouldReturnErrorWhenHadSubscriptionHasNsoPayment()
        {
            var subscription = new Subscription(null);
            _student.AddSubscription(subscription);
            Assert.IsTrue(_student.Invalid);
        }

        [TestMethod]
        public void ShouldReturnSuccessWhenAddSubscription()
        {
            var payment = new PayPalPayment("123", DateTime.Now, DateTime.Now.AddDays(5), 10, 10, _document, "Gustavo", _address, _email);
            var subscription = new Subscription(null);

            subscription.AddPayment(payment);
            _student.AddSubscription(subscription);

            Assert.IsTrue(_student.Valid);
        }
    }
}
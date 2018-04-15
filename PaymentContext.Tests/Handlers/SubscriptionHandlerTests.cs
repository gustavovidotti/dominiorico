using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Handlers;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Tests.Mocks;

namespace PaymentContext.Tests
{
    [TestClass]
    public class SubscriptionHandlerTests
    {
        [TestMethod]
        public void ShouldReturnErrorWhenDocumentExists()
        {
            var handler = new SubscriptionHandler(new FakeStudentRepository(), new FakeEmailService());
            var command = new CreateBoletoSubscriptionCommand();
            command.FirstName = "Gustavo";
            command.LastName = "Vidotti";
            command.Document = "99999999999";
            command.Email = "teste@teste.com";
            command.BarCode = "123";
            command.BoletoNumber = "123";
            command.PaymentNumber = "123";
            command.PaidDate = DateTime.Now;
            command.ExpireDate = DateTime.Now.AddMonths(1);
            command.Total = 60;
            command.TotalPaid = 60;
            command.Payer = "123";
            command.PayerDocument = "123";
            command.PayerDocumentType = EDocumentType.CPF;
            command.PayerEmail = "teste@teste.com";
            command.Street = "123";
            command.Number = "123";
            command.Neighborhood = "123";
            command.City = "123";
            command.State = "123";
            command.Country = "123";
            command.ZipCode = "123";

            handler.Handle(command);
            Assert.AreEqual(false, handler.Valid);

        }
    }
}
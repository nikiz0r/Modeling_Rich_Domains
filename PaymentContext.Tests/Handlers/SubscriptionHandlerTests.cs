using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Handlers;
using PaymentContext.Tests.Mocks;

namespace PaymentContext.Tests.Handlers
{
    [TestClass]
    public class SubscriptionHandlerTests
    {
        [TestMethod]
        public void ShouldReturnErrorWhenDocumentExists()
        {
            var handler = new SubscriptionHandler(new FakeStudentRepository(), new FakeEmailService());

            var command = new CreateBoletoSubscriptionCommand();

            command.FirstName = "Rumenigue";
            command.LastName = "Silva";
            command.Document = "99999999999";
            command.Email = "r@silva.com";
            command.BarCode = "123456789";
            command.BoletoNumber = "123654987";
            command.PaymentNumber = "123321";
            command.PaidDate = DateTime.Now;
            command.ExpireDate = DateTime.Now.AddMonths(1);
            command.Total = 60;
            command.TotalPaid = 60;
            command.Payer = "RSILVA";
            command.PayerDocument = "12345678911";
            command.PayerDocumentType = EDocumentType.CPF;
            command.PayerEmail = "r@silva.com";
            command.Street = "woeijq";
            command.Number = "123";
            command.Neighborhood = "weqwegweg";
            command.City = "as";
            command.State = "as";
            command.Country = "as";
            command.ZipCode = "12345687";

            handler.Handle(command);
            Assert.AreEqual(false, handler.Valid);
        }
    }
}
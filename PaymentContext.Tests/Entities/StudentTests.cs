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
        private readonly Document _document;
        private readonly Email _email;
        private readonly Address _address;
        private readonly Student _student;
        private readonly Subscription _subscription;

        public StudentTests()
        {
            _name = new Name("Rumenigue", "Silva");
            _document = new Document("63693173907", EDocumentType.CPF);
            _email = new Email("r@silva.com");
            _address = new Address("Rua 1", "1234", "Bairro Legal", "Cidade mais legal ainda", "SP", "Brazil", "09892000");

            _student = new Student(_name, _document, _email);
            _subscription = new Subscription(null);
        }

        [TestMethod]
        public void ShouldReturnErrorWhenHadActiveSubscription()
        {
            var payment = new PayPalPayment("123321", DateTime.Now, DateTime.Now.AddDays(5), 10, 10, "Nogueira Enterprise", _document, _address, _email);
            _subscription.AddPayment(payment);

            _student.AddSubscription(_subscription);
            _student.AddSubscription(_subscription);

            Assert.IsTrue(_student.Invalid);
        }

        [TestMethod]
        public void ShouldReturnErrorWhenSubscriptionHasNoPayment()
        {
            _student.AddSubscription(_subscription);

            Assert.IsTrue(_student.Invalid);
        }

        [TestMethod]
        public void ShouldReturnSuccessWhenAddSubscription()
        {
            var payment = new PayPalPayment("123321", DateTime.Now, DateTime.Now.AddDays(5), 10, 10, "Nogueira Enterprise", _document, _address, _email);
            _subscription.AddPayment(payment);

            _student.AddSubscription(_subscription);

            Assert.IsTrue(_student.Valid);
        }
    }
}

using System;
using Flunt.Notifications;
using Flunt.Validations;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Repositories;
using PaymentContext.Domain.Services;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Commands;
using PaymentContext.Shared.Handlers;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler :
        Notifiable,
        IHandler<CreateBoletoSubscriptionCommand>,
        IHandler<CreatePayPalSubscriptionCommand>
    {
        private readonly IStudentRepository _repository;
        private readonly IEmailService _emailService;

        public SubscriptionHandler(IStudentRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            // Fail fast validations
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Subscription were not created");
            }

            // Validates if Document exists
            if (_repository.DocumentExists(command.Document))
                AddNotification("Document", "This CPF is already in use");

            // Validates if Email exists
            if (_repository.EmailExists(command.Email))
                AddNotification("Email", "This E-mail is already in use");

            // Create VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            // Create Entities
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(command.BarCode, command.BoletoNumber, command.PaidDate, command.ExpireDate, command.Total, command.TotalPaid,
                command.Payer, new Document(command.PayerDocument, command.PayerDocumentType), address, new Email(command.PayerEmail));

            // Relationships
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            // Aggregate notifications
            AddNotifications(name, document, email, address, subscription, student, payment);

            // Check notifications
            if (Invalid)
                return new CommandResult(false, "It was not possible to create your subscription");

            // Save information
            _repository.CreateSubscription(student);

            // Send email            
            _emailService.Send(student.Name.ToString(), student.Email.Address, "Welcome to this site", "Subscription created successfully");           

            // Returns
            return new CommandResult(true, "Subscription created successfully");
        }

        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {
            // Fail fast validations
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Subscription were not created");
            }

            // Validates if Document exists
            if (_repository.DocumentExists(command.Document))
                AddNotification("Document", "This CPF is already in use");

            // Validates if Email exists
            if (_repository.EmailExists(command.Email))
                AddNotification("Email", "This E-mail is already in use");

            // Create VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            // Create Entities
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new PayPalPayment(command.TransactionCode, command.PaidDate, command.ExpireDate, command.Total, command.TotalPaid,
                command.Payer, new Document(command.PayerDocument, command.PayerDocumentType), address, new Email(command.PayerEmail));

            // Relationships
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            // Aggregate notifications
            AddNotifications(name, document, email, address, subscription, student, payment);

            // Check notifications
            if (Invalid)
                return new CommandResult(false, "It was not possible to create your subscription");

            // Save information
            _repository.CreateSubscription(student);

            // Send email            
            _emailService.Send(student.Name.ToString(), student.Email.Address, "Welcome to this site", "Subscription created successfully");           

            // Returns
            return new CommandResult(true, "Subscription created successfully");
        }
    }
}
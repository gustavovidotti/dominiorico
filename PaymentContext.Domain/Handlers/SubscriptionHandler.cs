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
            //Fail Fast Validations
            command.Validate();

            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Nao foi possivel realizar sua assinatura");
            }
            //Verificar se documento ja esta cadastrado
            if (_repository.DocummentExists(command.Document))
            {
                AddNotification("Document", "Este CPF ja esta em uso");
            }

            //Verificar se o email ja esta cadastrado
            if (_repository.EmailExists(command.Email))
            {
                AddNotification("Email", "Este Email ja esta em uso");
            }
            //Gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            //Gerar as Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(command.BarCode, command.BoletoNumber, command.PaidDate, command.ExpireDate,
                                            command.Total, command.TotalPaid, new Document(command.PayerDocument, command.PayerDocumentType),
                                            command.Payer, address, email);

            //Relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            //Agrupar as validacoes
            AddNotifications(name, document, email, address, student, subscription, payment);

            if (Invalid)
                return new CommandResult(false, "Nao foi possivel realizar sua assinatura");

            //Salvar as informacoes
            _repository.CreateSubscription(student);

            //Enviar email boas vindas
            _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo", "Assinatura Criada");

            //Retornar informacoes
            return new CommandResult(true, "Assinatura realizada com sucesso");
        }

        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {
            //Verificar se documento ja esta cadastrado
            if (_repository.DocummentExists(command.Document))
            {
                AddNotification("Document", "Este CPF ja esta em uso");
            }

            //Verificar se o email ja esta cadastrado
            if (_repository.EmailExists(command.Email))
            {
                AddNotification("Email", "Este Email ja esta em uso");
            }
            //Gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            //Gerar as Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new PayPalPayment(command.TransactionCode, command.PaidDate, command.ExpireDate,
                                            command.Total, command.TotalPaid, new Document(command.PayerDocument, command.PayerDocumentType),
                                            command.Payer, address, email);

            //Relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            //Agrupar as validacoes
            AddNotifications(name, document, email, address, student, subscription, payment);

            //Salvar as informacoes
            _repository.CreateSubscription(student);

            //Enviar email boas vindas
            _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo", "Assinatura Criada");

            //Retornar informacoes
            return new CommandResult(true, "Assinatura realizada com sucesso");
        }
    }
}
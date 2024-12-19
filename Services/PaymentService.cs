using System.Collections.Generic;
using VIS_projekt.Gateways;
using VIS_projekt.TableModule;
using VIS_projekt.Infrastructure;
using Microsoft.Data.SqlClient;

namespace VIS_projekt.Services
{
    public class PaymentService
    {
        private readonly SqlConnectionStringBuilder _connectionBuilder;
        private readonly PaymentGateway _paymentGateway;

        public PaymentService(SqlConnectionStringBuilder connectionBuilder, PaymentGateway paymentGateway)
        {
            _connectionBuilder = connectionBuilder;
            _paymentGateway = paymentGateway;
        }


        public void ProcessPayment(int userId, string paymentType, decimal amount)
        {
            using (var uow = new UnitOfWork(_connectionBuilder))
            {
                try
                {
                    _paymentGateway.ProcessUserPayment(uow, userId, paymentType, amount);
                    uow.Commit();
                }
                catch
                {
                    uow.Rollback();
                    throw;
                }
            }
        }


        public List<Payment> GetPaymentsByUserId(int userId)
        {
            using (var uow = new UnitOfWork(_connectionBuilder))
            {
                var payments = _paymentGateway.GetPaymentsByUserId(uow, userId);
                uow.Commit();
                return payments;
            }
        }


        public List<Payment> GetPaymentsByTrainerId(int trainerId)
        {
            using (var uow = new UnitOfWork(_connectionBuilder))
            {
                var payments = _paymentGateway.GetPaymentsByTrainerId(uow, trainerId);
                uow.Commit();
                return payments;
            }
        }
    }
}

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using VIS_projekt.TableModule;
using VIS_projekt.Infrastructure;

namespace VIS_projekt.Gateways
{
    public class PaymentGateway
    {
        private readonly CsvPaymentProvider _csvPaymentProvider;

        public PaymentGateway(CsvPaymentProvider csvPaymentProvider)
        {
            _csvPaymentProvider = csvPaymentProvider; 
        }


        public void ProcessUserPayment(UnitOfWork uow, int userId, string paymentType, decimal amount)
        {
            using (var command = new SqlCommand("ProcessUserPayment", uow.Connection, uow.Transaction))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@PaymentType", paymentType);
                command.Parameters.AddWithValue("@Amount", amount);
                command.ExecuteNonQuery();
            }

            var newPayment = new Payment
            {
                UserId = userId,
                Type = paymentType,
                Amount = amount,
                Date = DateTime.UtcNow
            };
            _csvPaymentProvider.AddPayment(newPayment);
        }


        public List<Payment> GetPaymentsByUserId(UnitOfWork uow, int userId)
        {
            var payments = new List<Payment>();

            using (var command = new SqlCommand("SELECT * FROM payment WHERE user_id_user = @UserId", uow.Connection, uow.Transaction))
            {
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payments.Add(new Payment
                        {
                            Id = (int)reader["id_payment"],
                            Type = reader["type"]?.ToString(),
                            Amount = (decimal)reader["amount"],
                            Date = (DateTime)reader["date"],
                            UserId = (int)reader["user_id_user"]
                        });
                    }
                }
            }

            var csvPayments = _csvPaymentProvider.GetAllPayments().FindAll(p => p.UserId == userId);
            payments.AddRange(csvPayments);

            return payments;
        }


        public List<Payment> GetPaymentsByTrainerId(UnitOfWork uow, int trainerId)
        {
            var payments = new List<Payment>();

            using (var command = new SqlCommand(@"
                SELECT p.* 
                FROM payment p
                JOIN [user] u ON p.user_id_user = u.id_user
                WHERE u.trainer_id_trainer = @TrainerId", uow.Connection, uow.Transaction))
            {
                command.Parameters.AddWithValue("@TrainerId", trainerId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payments.Add(new Payment
                        {
                            Id = (int)reader["id_payment"],
                            Type = reader["type"]?.ToString(),
                            Amount = (decimal)reader["amount"],
                            Date = (DateTime)reader["date"],
                            UserId = (int)reader["user_id_user"]
                        });
                    }
                }
            }

            return payments;
        }
    }
}

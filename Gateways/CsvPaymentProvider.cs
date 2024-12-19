using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using VIS_projekt.TableModule;

namespace VIS_projekt.Gateways
{
    public class CsvPaymentProvider
    {
        private readonly string _csvFilePath;
        private readonly CsvConfiguration _csvConfig;

        public CsvPaymentProvider(string csvFilePath)
        {
            _csvFilePath = csvFilePath;
            _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };

            if (!File.Exists(_csvFilePath))
            {
                using (var writer = new StreamWriter(_csvFilePath))
                using (var csv = new CsvWriter(writer, _csvConfig))
                {
                    csv.WriteHeader<Payment>();
                    csv.NextRecord();
                }
            }
        }

        public List<Payment> GetAllPayments()
        {
            using (var reader = new StreamReader(_csvFilePath))
            using (var csv = new CsvReader(reader, _csvConfig))
            {
                return csv.GetRecords<Payment>().ToList();
            }
        }

        public Payment? GetPaymentById(int id)
        {
            return GetAllPayments().FirstOrDefault(p => p.Id == id);
        }

        public void AddPayment(Payment payment)
        {
            var payments = GetAllPayments();
            payment.Id = payments.Count > 0 ? payments.Max(p => p.Id) + 1 : 1;
            payment.Date = DateTime.UtcNow;
            payments.Add(payment);
            SaveAllPayments(payments);
        }

        public void UpdatePayment(Payment updatedPayment)
        {
            var payments = GetAllPayments();
            var index = payments.FindIndex(p => p.Id == updatedPayment.Id);
            if (index != -1)
            {
                payments[index] = updatedPayment;
                SaveAllPayments(payments);
            }
        }

        public void DeletePayment(int id)
        {
            var payments = GetAllPayments();
            var paymentToRemove = payments.FirstOrDefault(p => p.Id == id);
            if (paymentToRemove != null)
            {
                payments.Remove(paymentToRemove);
                SaveAllPayments(payments);
            }
        }

        private void SaveAllPayments(List<Payment> payments)
        {
            using (var writer = new StreamWriter(_csvFilePath))
            using (var csv = new CsvWriter(writer, _csvConfig))
            {
                csv.WriteHeader<Payment>();
                csv.NextRecord();
                csv.WriteRecords(payments);
            }
        }
    }
}

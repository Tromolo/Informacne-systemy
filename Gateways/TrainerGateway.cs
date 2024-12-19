using Microsoft.Data.SqlClient;
using System;
using VIS_projekt.DTOs;

namespace VIS_projekt.Gateways
{
    public class TrainerGateway
    {
        private readonly SqlConnectionStringBuilder _connectionBuilder;

        public TrainerGateway(SqlConnectionStringBuilder connectionBuilder)
        {
            _connectionBuilder = connectionBuilder;
        }

        public Trainer? Find(int trainerId)
        {
            using (var connection = new SqlConnection(_connectionBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM trainer WHERE id_trainer = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", trainerId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Trainer
                            {
                                Id = (int)reader["id_trainer"],
                                Name = reader["name"]?.ToString(),
                                Surname = reader["surname"]?.ToString(),
                                Email = reader["email"]?.ToString(),
                                Phone = reader["phone"]?.ToString(),
                                Specialisation = reader["specialisation"]?.ToString(),
                                Skills = reader["skills"]?.ToString(),
                                PasswordHash = reader["PasswordHash"]?.ToString(),
                                DeletedAt = reader["deleted_at"] != DBNull.Value ? (DateTime?)reader["deleted_at"] : null
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void RegisterTrainer(Trainer trainer, string password)
        {
            using (var connection = new SqlConnection(_connectionBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "INSERT INTO trainer (name, surname, email, phone, specialisation, skills, PasswordHash, deleted_at) " +
                    "VALUES (@Name, @Surname, @Email, @Phone, @Specialisation, @Skills, @PasswordHash, @DeletedAt)",
                    connection))
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                    command.Parameters.AddWithValue("@Name", (object?)trainer.Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Surname", (object?)trainer.Surname ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Email", (object?)trainer.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", (object?)trainer.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Specialisation", (object?)trainer.Specialisation ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Skills", (object?)trainer.Skills ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    command.Parameters.AddWithValue("@DeletedAt", (object?)trainer.DeletedAt ?? DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        public bool VerifyTrainerCredentials(string email, string password)
        {
            var trainer = FindByEmail(email);
            if (trainer == null || string.IsNullOrEmpty(trainer.PasswordHash))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, trainer.PasswordHash);
        }

        public Trainer? FindByEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM trainer WHERE email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Trainer
                            {
                                Id = (int)reader["id_trainer"],
                                Name = reader["name"]?.ToString(),
                                Surname = reader["surname"]?.ToString(),
                                Email = reader["email"]?.ToString(),
                                Phone = reader["phone"]?.ToString(),
                                Specialisation = reader["specialisation"]?.ToString(),
                                Skills = reader["skills"]?.ToString(),
                                PasswordHash = reader["PasswordHash"]?.ToString(),
                                DeletedAt = reader["deleted_at"] != DBNull.Value ? (DateTime?)reader["deleted_at"] : null
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}

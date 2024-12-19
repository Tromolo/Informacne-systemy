using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using VIS_projekt.DTOs;

namespace VIS_projekt.Gateways
{
    public class UserGateway
    {
        private readonly SqlConnectionStringBuilder _connectionBuilder;

        public UserGateway(SqlConnectionStringBuilder connectionBuilder)
        {
            _connectionBuilder = connectionBuilder;
        }

        public void Register(User user, string password)
        {
            using (var connection = new SqlConnection(_connectionBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "INSERT INTO [user] (trainer_id_trainer, name, surname, email, phone, role, PasswordHash, deleted_at) " +
                    "VALUES (@TrainerId, @Name, @Surname, @Email, @Phone, @Role, @PasswordHash, @DeletedAt)",
                    connection))
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                    command.Parameters.AddWithValue("@TrainerId", user.TrainerId.HasValue ? (object)user.TrainerId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Name", (object?)user.Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Surname", (object?)user.Surname ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Email", (object?)user.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", (object?)user.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    command.Parameters.AddWithValue("@DeletedAt", (object?)user.DeletedAt ?? DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        public User? FindByEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM [user] WHERE email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = (int)reader["id_user"],
                                TrainerId = reader["trainer_id_trainer"] != DBNull.Value ? (int?)reader["trainer_id_trainer"] : null,
                                Name = reader["name"]?.ToString(),
                                Surname = reader["surname"]?.ToString(),
                                Email = reader["email"]?.ToString(),
                                Phone = reader["phone"]?.ToString(),
                                Role = Convert.ToChar(reader["role"]),
                                PasswordHash = reader["PasswordHash"]?.ToString(),
                                DeletedAt = reader["deleted_at"] != DBNull.Value ? (DateTime?)reader["deleted_at"] : null
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool VerifyUserCredentials(string email, string password)
        {
            var user = FindByEmail(email);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public void AssignTrainerToUser(int userId, int trainerId)
        {
            using (var connection = new SqlConnection(_connectionBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("UPDATE [user] SET trainer_id_trainer = @TrainerId WHERE id_user = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@TrainerId", trainerId);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<User> GetUsersByTrainerId(int trainerId)
        {
            var users = new List<User>();
            using (var connection = new SqlConnection(_connectionBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM [user] WHERE trainer_id_trainer = @TrainerId", connection))
                {
                    command.Parameters.AddWithValue("@TrainerId", trainerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id = (int)reader["id_user"],
                                TrainerId = reader["trainer_id_trainer"] != DBNull.Value ? (int?)reader["trainer_id_trainer"] : null,
                                Name = reader["name"]?.ToString(),
                                Surname = reader["surname"]?.ToString(),
                                Email = reader["email"]?.ToString(),
                                Phone = reader["phone"]?.ToString(),
                                Role = Convert.ToChar(reader["role"]),
                                DeletedAt = reader["deleted_at"] != DBNull.Value ? (DateTime?)reader["deleted_at"] : null
                            });
                        }
                    }
                }
            }
            return users;
        }

        public List<User> GetUsersWithoutTrainer()
        {
            var users = new List<User>();
            using (var connection = new SqlConnection(_connectionBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT id_user, name, surname FROM [user] WHERE trainer_id_trainer IS NULL", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id = (int)reader["id_user"],
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString()
                            });
                        }
                    }
                }
            }
            return users;
        }

        public Trainer? GetTrainerForUser(int userId)
        {
            using (var connection = new SqlConnection(_connectionBuilder.ConnectionString))
            {
                connection.Open();
                var query = @"
                    SELECT t.id_trainer, t.name, t.surname
                    FROM [user] u
                    JOIN trainer t ON u.trainer_id_trainer = t.id_trainer
                    WHERE u.id_user = @UserId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Trainer
                            {
                                Id = (int)reader["id_trainer"],
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}

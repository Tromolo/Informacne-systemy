using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace VIS_projekt.Domain
{
    public class TrainingPlan
    {
        private static string _connectionString;

        public static void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Id { get; private set; }
        public int UserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? Description { get; set; }
        public bool Active { get; private set; }


        public TrainingPlan(int userId, string? description, bool active = true)
        {
            UserId = userId;
            Description = description;
            Active = active;
            CreatedAt = DateTime.UtcNow;
        }


        private TrainingPlan() { }


        public void Save()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                if (Id == 0)
                {
                    if (Active)
                    {
                        using (var deactivateCommand = new SqlCommand(
                            "UPDATE training_plan SET active = 0 WHERE user_id_user = @UserId AND active = 1",
                            connection))
                        {
                            deactivateCommand.Parameters.AddWithValue("@UserId", UserId);
                            deactivateCommand.ExecuteNonQuery();
                        }
                    }

                    using (var command = new SqlCommand(
                        "INSERT INTO training_plan (user_id_user, created_at, description, active) " +
                        "OUTPUT INSERTED.id_training_plan " +
                        "VALUES (@UserId, @CreatedAt, @Description, @Active)",
                        connection))
                    {
                        command.Parameters.AddWithValue("@UserId", UserId);
                        command.Parameters.AddWithValue("@CreatedAt", CreatedAt);
                        command.Parameters.AddWithValue("@Description", (object?)Description ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Active", Active);

                        Id = (int)command.ExecuteScalar();
                    }
                }
                else
                {

                    using (var command = new SqlCommand(
                        "UPDATE training_plan SET description = @Description, active = @Active WHERE id_training_plan = @Id",
                        connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        command.Parameters.AddWithValue("@Description", (object?)Description ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Active", Active);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Delete()
        {
            if (Id == 0)
                throw new InvalidOperationException("Cannot delete a plan that hasn't been saved.");

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "DELETE FROM training_plan WHERE id_training_plan = @Id",
                    connection))
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    command.ExecuteNonQuery();
                }
            }

            Id = 0;
        }

        public static TrainingPlan? FindById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM training_plan WHERE id_training_plan = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return Map(reader);
                    }
                }
            }
            return null;
        }

        public static List<TrainingPlan> FindByUserId(int userId)
        {
            var plans = new List<TrainingPlan>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM training_plan WHERE user_id_user = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            plans.Add(Map(reader));
                    }
                }
            }
            return plans;
        }

        public static List<TrainingPlan> FindByTrainerId(int trainerId)
        {
            var plans = new List<TrainingPlan>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT tp.id_training_plan, tp.user_id_user, tp.created_at, tp.description, tp.active " +
                    "FROM training_plan tp " +
                    "JOIN [user] u ON tp.user_id_user = u.id_user " +
                    "WHERE u.trainer_id_trainer = @TrainerId",
                    connection))
                {
                    command.Parameters.AddWithValue("@TrainerId", trainerId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            plans.Add(Map(reader));
                    }
                }
            }
            return plans;
        }

        private static TrainingPlan Map(SqlDataReader reader)
        {
            return new TrainingPlan
            {
                Id = (int)reader["id_training_plan"],
                UserId = (int)reader["user_id_user"],
                CreatedAt = (DateTime)reader["created_at"],
                Description = reader["description"] as string,
                Active = reader["active"] != DBNull.Value && (bool)reader["active"]
            };
        }

        public void Activate()
        {
            Active = true;
        }

        public void Deactivate()
        {
            Active = false;
        }
    }
}

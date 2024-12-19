using Microsoft.Data.SqlClient;

namespace VIS_projekt.Infrastructure
{
    public class UnitOfWork : IDisposable
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _transaction;

        public UnitOfWork(SqlConnectionStringBuilder connectionStringBuilder)
        {
            _connection = new SqlConnection(connectionStringBuilder.ConnectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public SqlConnection Connection => _connection;
        public SqlTransaction Transaction => _transaction;

        public void Commit()
        {
            _transaction?.Commit();
            _transaction = null;
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction = null;
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }

            _connection.Close();
            _connection.Dispose();
        }
    }
}

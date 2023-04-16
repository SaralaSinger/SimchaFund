using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class Manager
    {
        private string _connectionString;
        public Manager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddContributor(Contributor c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributors VALUES (@firstName, @lastName, @cellNumber, @alwaysInclude, @dateCreated) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@firstName", c.FirstName);
            cmd.Parameters.AddWithValue("@lastName", c.LastName);
            cmd.Parameters.AddWithValue("@cellNumber", c.CellNumber);
            cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            cmd.Parameters.AddWithValue("@dateCreated", c.DateCreated);
            connection.Open();
            c.Id = (int)(decimal)cmd.ExecuteScalar();
            connection.Close();
        }
        public void AddSimcha(Simcha s)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Simchas VALUES (@simchaName, @date) ";
            cmd.Parameters.AddWithValue("@simchaName", s.SimchaName);
            cmd.Parameters.AddWithValue("@date", s.Date);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }
        public void AddDeposit(Payment p)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Deposits VALUES (@contributorId, @depositAmount, @date) ";
            cmd.Parameters.AddWithValue("@contributorId", p.ContributorId);
            cmd.Parameters.AddWithValue("@depositAmount", p.Amount);
            cmd.Parameters.AddWithValue("@date", p.Date);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }
        public void AddContributions(List<Payment> contributions)
        {
            foreach (var p in contributions)
            {
                AddContribution(p);
            }
        }
        public void AddContribution(Payment p)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            if (p.YesContribute)
            {
                cmd.CommandText = "INSERT INTO SimchasContributors VALUES (@simchaId, @contributorId, @amount) ";
                cmd.Parameters.AddWithValue("@simchaId", p.SimchaId);
                cmd.Parameters.AddWithValue("@contributorId", p.ContributorId);
                cmd.Parameters.AddWithValue("@amount", -p.Amount);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public List<Simcha> GetAllSimchas()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Simchas ORDER BY Date DESC";
            var list = new List<Simcha>();
            connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Simcha
                {
                    Id = (int)reader["Id"],
                    SimchaName = (string)reader["SimchaName"],
                    Date = (DateTime)reader["Date"],
                });
            }
            connection.Close();
            return list;
        }
        public string GetContributorName(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT FirstName, LastName FROM Contributors WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            string name = "";
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                name += (string)reader["FirstName"];
                name += " ";
                name += (string)reader["LastName"];
            }

            connection.Close();
            return name;
        }
        public string GetSimchaName(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SimchaName FROM Simchas WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            string name = "";
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                name = (string)reader["SimchaName"];
            }
            connection.Close();
            return name;
        }
        public List<Contributor> GetAllContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributors";
            var list = new List<Contributor>();
            connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    DateCreated = (DateTime)reader["DateCreated"],
                });
            }
            connection.Close();
            return list;
        }
        public decimal YesContribute(int conId, int simId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Amount FROM SimchasContributors WHERE ContributorId = @conId AND SimchaId = @simId";
            cmd.Parameters.AddWithValue("@conId", conId);
            cmd.Parameters.AddWithValue("@simId", simId);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                decimal result = -(decimal)reader["Amount"];
                return result;
            }
            return 0;  
        }
        public void ClearContributions(int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM SimchasContributors WHERE SimchaId = @simchaId";
            cmd.Parameters.AddWithValue("@simchaId", simchaId);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }
        public List<Payment> GetHistory(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT s.SimchaName, sc.Amount, s.Date FROM Simchas s " +
                "LEFT JOIN SimchasContributors sc ON s.Id = sc.SimchaId " +
                //"LEFT JOIN Contributors c ON c.Id = sc.ContributorId " +
                "WHERE sc.ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            var list = new List<Payment>();
            connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Payment
                {
                    SimchaName = (string)reader["SimchaName"],
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"],
                });
            }
            connection.Close();
            cmd.CommandText = "SELECT Date, DepositAmount FROM Deposits WHERE ContributorId = @contribId";
            cmd.Parameters.AddWithValue("@contribId", id);
            connection.Open();
            var re = cmd.ExecuteReader();
            while (re.Read())
            {
                list.Add(new Payment
                {
                    Amount = (decimal)re["DepositAmount"],
                    Date = (DateTime)re["Date"],
                });
            }
            connection.Close();
            return list;
        }
        public decimal GetBalance(int id)
        {
            decimal balance = 0;
            var list = GetHistory(id);
            foreach (var p in list)
            {
                balance += p.Amount;
            }
            return balance;
        }
        public int GetContributorCount(int simId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM SimchasContributors WHERE SimchaId = @simId";
            cmd.Parameters.AddWithValue("@simId", simId);
            connection.Open();        
            return (int)cmd.ExecuteScalar();
        }
        public int GetTotalContributorCount()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
            connection.Open();
            return (int)cmd.ExecuteScalar();
        }
        public decimal GetTotalPerSimcha(int simId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SUM(Amount) FROM SimchasContributors WHERE SimchaId = @simId";
            cmd.Parameters.AddWithValue("@simId", simId);
            connection.Open();
            decimal total = 0;
            if(cmd.ExecuteScalar() == DBNull.Value)
            {
                return 0;
            }
            total = -(decimal)cmd.ExecuteScalar();
            return total;
        }
        public void UpdateContributor(Contributor c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Contributors SET FirstName = @firstName, LastName = @lastName, " +
                "CellNumber = @cellNumber, AlwaysInclude = @alwaysInclude, DateCreated = @dateCreated WHERE Id = @id";
            cmd.Parameters.AddWithValue("@firstName", c.FirstName);
            cmd.Parameters.AddWithValue("@lastName", c.LastName);
            cmd.Parameters.AddWithValue("@cellNumber", c.CellNumber);
            cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            cmd.Parameters.AddWithValue("@dateCreated", c.DateCreated);
            cmd.Parameters.AddWithValue("@id", c.Id);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }
        public decimal GetGrandTotal(List<Contributor> contributors)
        {
            decimal grandTotal = 0;
            foreach(var c in contributors)
            {
                grandTotal += GetBalance(c.Id);
            }
            return grandTotal;
        }
    }
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellNumber { get; set; }
        public bool AlwaysInclude { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal Balance { get; set; }
        public int Index { get; set; }
        public decimal YesContribute { get; set; }
    }
    public class Simcha
    {
        public int Id { get; set; }
        public string SimchaName { get; set; }
        public DateTime Date { get; set; }
        public int ContributorCount { get; set; }
        public decimal Total { get; set; }
    }
    public class Payment
    {
        public int ContributorId { get; set; }
        public bool YesContribute { get; set; }
        public int SimchaId { get; set; }
        public string SimchaName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}

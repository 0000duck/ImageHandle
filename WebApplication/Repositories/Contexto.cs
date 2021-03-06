﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Specialized;

namespace WebApplication.Repositories
{
    public class Contexto : IDisposable
    {
        public MySqlConnection connection;

        public Contexto() 
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MysqlDB"].ConnectionString;
            connection = new MySqlConnection(connectionString);
        }

        public int ExecuteCommand(string sql, Dictionary<string, object> paramsQSL){
            var result = 0; 
            try 
            {
                OpenConnection();
                var command = CreateConnection(sql, paramsQSL);
                result = command.ExecuteNonQuery();
            }
            finally 
            {
                CloseConnection();
            }
            return result;
        }

        public List<Dictionary<string, object>> ExecuteCommandSQL(string sql, Dictionary<string, object> paramsSQL = null)
        {
            List<Dictionary<string, object>> lines = new List<Dictionary<string, object>>();

            if(string.IsNullOrEmpty(sql))
            {
                throw new ArgumentException("SQL string can not be null");
            }
            try 
            {
                OpenConnection();
                var command = CreateConnection(sql, paramsSQL);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var line = new Dictionary<string, object>();

                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var nameColumn = reader.GetName(i);
                            var valueColumn = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            line.Add(nameColumn, valueColumn);
                        }
                        lines.Add(line);
                    }
                    reader.Close();
                }
            }
            finally 
            {
                CloseConnection();
            }
            return lines;
        }

        private MySqlCommand CreateConnection(string sql, Dictionary<string, object> paramsSQL)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            AddParams(command, paramsSQL);
            return command;
        }

        public void AddParams(MySqlCommand commandSQL, Dictionary<string, object> paramsSQL)
        {
            if (paramsSQL == null)
                return;

            foreach (var item in paramsSQL) 
            {
                var param = commandSQL.CreateParameter();
                param.ParameterName = item.Key;
                param.Value = item.Value ?? DBNull.Value;
                commandSQL.Parameters.Add(param);
            }
        }

        public void OpenConnection() {
            if (connection.State == ConnectionState.Open) return;
            connection.Open();
        }

        public void CloseConnection() 
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        public void Dispose()
        {
            if (connection == null) return;

            connection.Dispose();
            connection = null;
        }

        public void ExecuteCommandWithTransaction(string sql, string sql2)
        {
            try
            {
                using (connection)
                {
                    OpenConnection();
                    MySqlTransaction transaction = connection.BeginTransaction();
                    MySqlCommand cmd = connection.CreateCommand();
                    cmd.Transaction = transaction;

                    try
                    {
                        cmd.CommandText = sql;
                        int x = cmd.ExecuteNonQuery();
                        cmd.CommandText = sql2;
                        int y = cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

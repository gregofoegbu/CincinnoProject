﻿using System;
using System.Reflection;
using Cincinno.Models;
using Npgsql;

namespace Cincinno.Services
{
    public class UserService
    {
        private readonly NpgsqlConnection _dbconnection;

        public UserService(IConfiguration configuration)
        {
            _dbconnection = new NpgsqlConnection(configuration.GetConnectionString("CincinnoCon"));
        }

        public bool SaveUser(UserModel user)
        {
            int a;
            _dbconnection.Open();

            using (var cmd = new NpgsqlCommand("INSERT INTO users (user_id, user_name, address) VALUES (@user_id, @user_name, @address)", _dbconnection))
            {
                cmd.Parameters.AddWithValue("user_id", user.UserId);
                cmd.Parameters.AddWithValue("user_name", user.Username);
                cmd.Parameters.AddWithValue("address", user.Address);

                a = cmd.ExecuteNonQuery();
            }

            _dbconnection.Close();

            return a != 0;
        }

        public bool DeleteUser(Guid userId)
        {
            int a, a1;
            _dbconnection.Open();

            using (var cmd = new NpgsqlCommand("DELETE FROM users WHERE user_id = @id", _dbconnection))
            {
                cmd.Parameters.AddWithValue("id", userId);
                a1 = cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand("DELETE FROM auth WHERE user_id = @id", _dbconnection))
            {
                cmd.Parameters.AddWithValue("id", userId);
                a = cmd.ExecuteNonQuery();
            }

            _dbconnection.Close();
            return a != 0 && a1 != 0;
        }

        public UserModel GetUser(Guid userId)
        {
            var user = new UserModel();
            _dbconnection.Open();

            using (var cmd = new NpgsqlCommand("SELECT * FROM users WHERE user_id = @id", _dbconnection))
            {
                cmd.Parameters.AddWithValue("id", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel
                        {
                            UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
                            Username = reader.GetString(reader.GetOrdinal("user_name")),
                            Address = reader.GetString(reader.GetOrdinal("address")),
                            Phone = reader.GetString(reader.GetOrdinal("phone")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Fullname = reader.GetString(reader.GetOrdinal("fullname")),
                            RegisteredDevices = reader.GetInt32(reader.GetOrdinal("registered_devices")),
                            DeviceThreshold = reader.GetInt32(reader.GetOrdinal("threshold"))
                        };
                    }
                }
            }
            _dbconnection.Close();
            return user;
        }

        public bool UpdateThreshold(Guid userId, int threshold)
        {
            int a;
            _dbconnection.Open();

            using (var command = new NpgsqlCommand("UPDATE users SET threshold = @threshold WHERE user_id = @id", _dbconnection))
            {
                command.Parameters.AddWithValue("@threshold", threshold);
                command.Parameters.AddWithValue("@id", userId);
                a = command.ExecuteNonQuery();
            }

            return a != 0;
        }

        public Guid? GetUserId(int device_id)
        {
            Guid? userID = null;
            _dbconnection.Open();

            using (var cmd = new NpgsqlCommand("SELECT * FROM devices WHERE device_id = @device_id", _dbconnection))
            {
                cmd.Parameters.AddWithValue("device_id", device_id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userID = reader.GetGuid(reader.GetOrdinal("user_id"));
                    }
                }
            }
            _dbconnection.Close();
            return userID;
        }

        public bool AddHouseholdMember(Guid userId, string memberName)
        {
            int a;
            _dbconnection.Open();

            using (var cmd = new NpgsqlCommand("INSERT INTO user_members (user_id, member_name) VALUES (@user_id, @member_name)", _dbconnection))
            {
                cmd.Parameters.AddWithValue("user_id", userId);
                cmd.Parameters.AddWithValue("member_name", memberName);

                a = cmd.ExecuteNonQuery();
            }

            _dbconnection.Close();
            return a != 0;
        }

        public List<string> GetHouseholdMembers(Guid userId)
        {
            _dbconnection.Open();
            var names = new List<string>();

            using (var cmd = new NpgsqlCommand("SELECT * FROM user_members WHERE user_id = @user_id", _dbconnection))
            {
                cmd.Parameters.AddWithValue("user_id", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        var Name = reader.GetString(reader.GetOrdinal("member_name"));

                        names.Add(Name);
                    }
                }
            }

            _dbconnection.Close();

            return names;
        }

        public bool DeleteMember(Guid userId, string membername)
        {
            int a;
            _dbconnection.Open();

            using (var cmd = new NpgsqlCommand("DELETE FROM user_members WHERE user_id = @id AND member_name = @membername", _dbconnection))
            {
                cmd.Parameters.AddWithValue("id", userId);
                cmd.Parameters.AddWithValue("membername", membername);

                a = cmd.ExecuteNonQuery();
            }

            _dbconnection.Close();
            return a != 0;
        }

        public bool AddActivityLog(ActivityModel activity)
        {
            string status;
            if (activity.Status == LogStatus.AccessGranted)
            {
                status = "Access Granted";
            } else { status = "Access Denied"; }
            int a;
            _dbconnection.Open();

            using (var cmd = new NpgsqlCommand("INSERT INTO activity (user_id, member_name, access_time, log_status) VALUES (@user_id, @member_name, @access_time, @status)", _dbconnection))
            {
                cmd.Parameters.AddWithValue("user_id", activity.UserId);
                cmd.Parameters.AddWithValue("member_name", activity.MemberName);
                cmd.Parameters.AddWithValue("access_time", activity.AccessTime);
                cmd.Parameters.AddWithValue("status", status);

                a = cmd.ExecuteNonQuery();
            }

            _dbconnection.Close();
            return a != 0;
        }

        public bool DeleteActivityLog(int logId)
        {
            int a;
            _dbconnection.Open();

            using (var cmd = new NpgsqlCommand("DELETE FROM activity WHERE id = @id", _dbconnection))
            {
                cmd.Parameters.AddWithValue("id", logId);

                a = cmd.ExecuteNonQuery();
            }

            _dbconnection.Close();
            return a != 0;
        }

        public List<ActivityModel> GetUserLog(Guid userId)
        {
            var logs = new List<ActivityModel>();
            _dbconnection.Open();

            using (var cmd = new NpgsqlCommand("SELECT * FROM activity WHERE user_id = @id", _dbconnection))
            {
                cmd.Parameters.AddWithValue("id", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var log = new ActivityModel
                        {
                            UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
                            MemberName = reader.GetString(reader.GetOrdinal("member_name")),
                            AccessTime = reader.GetDateTime(reader.GetOrdinal("access_time")),
                            Status = reader.GetString(reader.GetOrdinal("log_status")) == "Access Granted" ? LogStatus.AccessGranted : LogStatus.AccessDenied
                        };
                        logs.Add(log);
                    }
                }
            }
            _dbconnection.Close();
            return logs;
        }
    }
}


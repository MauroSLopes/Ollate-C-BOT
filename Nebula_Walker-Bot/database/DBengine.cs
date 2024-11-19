using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;

namespace Nebula_Walker_Bot.database
{
    public class DBengine
    {
        private string connectionString = "";

        // Functions used in events
        // Returns the COUNT of all users in database
        public async Task<int> TotalUsersAsync()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string query = "SELECT COUNT(*) FROM nebulashift.\"membroAtivo\";";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var userCount = await cmd.ExecuteScalarAsync();
                    await conn.CloseAsync();
                    return Convert.ToInt32(userCount);
                }
            }
        }

        // Returns the table userID using the discordID
        public async Task<int> GetUserID(ulong discordID)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string query = "SELECT userid FROM nebulashift.\"membroAtivo\" " +
                    $"WHERE discid = '{discordID}'";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    var result = await cmd.ExecuteScalarAsync();
                    int userID = Convert.ToInt32(result);
                    return Convert.ToInt32(userID);
                }
            }
        }

        // uses the userID to return the messages total
        public async Task<int> GetUserMensagesByID(int userID)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    // Returns the messages number
                    string query = "SELECT quantmensagens FROM nebulashift.\"membroAtivo\" WHERE " +
                        $"userid = '{userID}'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {

                        var result = await cmd.ExecuteScalarAsync();
                        await conn.CloseAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }

        }

        // Inserts a new user
        public async Task StoreNewUserAsync(DBuser user)
        {
            // Get All users in the DATA BASE

            int userID = await TotalUsersAsync();

            // Checks if the command was executed correctly

            if (userID == -1)
            {
                throw new Exception();
            }
            else
            {
                // Go to the next table row
                userID = userID + 1;
            }

            try { 
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();

                    // Insert a new user in the table last position
                string query = "INSERT INTO nebulashift.\"membroAtivo\" (userid, discid, quantmensagens, tempocall) "
                    + $"VALUES ('{userID}','{user.discordID}','{user.quantMensagem}','{user.tempoCall}')";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                await conn.CloseAsync();
            }
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // Adds +1 in the message
        public async Task<bool> IncrementMensages(int userID)
        {
            int totalMensages = await GetUserMensagesByID(userID);
            // Checks if the command gone wrong
            if (totalMensages == -1)
            {
                throw new Exception();
            }
            else
            {
                // Sum +1 in the message count
                totalMensages++;
            }
            try { 
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();

                    // Deliver to the database the new message total
                string query = "UPDATE nebulashift.\"membroAtivo\" " 
                    +$"SET quantMensagens = '{totalMensages}'" 
                    +$"WHERE userid = '{userID}'";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                await conn.CloseAsync();
                return true;
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }



        // Functions used in commands
        // Used to manually change the user total messages
        public async Task changeTotalMensages(int totalMensages, ulong discordID)
        {
            int userID = await GetUserID(discordID);

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE nebulashift.\"membroAtivo\" "
                    + $"SET quantMensagens = '{totalMensages}'"
                    + $"WHERE userid = '{userID}'";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                Console.WriteLine("Modificado.");
                await connection.CloseAsync();
            }
        }

        // Generates a DBuser using discord user id
        public async Task<DBuser> GetProfileInfo(ulong discordID)
        {
            int userID = await GetUserID(discordID);
            DBuser result;
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                await con.OpenAsync();

                string query = "SELECT userid, discid, quantmensagens, tempocall FROM nebulashift.\"membroAtivo\" "
                    + $"WHERE userid = {userID}";

                using(NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    await reader.ReadAsync();

                    result = new DBuser
                    {
                        ID = reader.GetInt32(0),
                        discordID = reader.GetString(1),
                        quantMensagem = reader.GetInt32(2),
                        tempoCall = Convert.ToUInt64(reader.GetInt64(3))
                    };
                }
                Console.WriteLine(result.ID);
                await con.CloseAsync();
                return result;
            }
        }

        // Resets a specific user messages and call time in database
        public async Task ResetUser(ulong discordid)
        {
            int id = await GetUserID(discordid);

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE nebulashift.\"membroAtivo\" "
                    + $"SET quantMensagens = '0', tempocall = '0'"
                    + $"WHERE userid = '{id}'";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                Console.WriteLine("Modificado.");
                await connection.CloseAsync();
            }
        }

        // Reset all users messages and call time in database
        public async Task ResetAll()
        {
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                await con.OpenAsync();

                string query = "UPDATE nebulashift.\"membroAtivo\" "
                    + $"SET quantMensagens = '0', tempocall = '0'";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query,con))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            
        }

        
    }
}

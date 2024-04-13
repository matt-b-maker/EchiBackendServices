using System.Data.SqlClient;
using System.Text.Json.Serialization;
using EchiBackendServices.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Newtonsoft.Json;

namespace EchiBackendServices.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class DbController(ConnectionStringStore connectionStringStore) : Controller
{
    [HttpPost]
    public IActionResult InsertClient(ClientModel client)
    {
        try
        {
            using var conn = new SqlConnection(connectionStringStore.SqlConnectionString);

            conn.Open();

            const string sql =
                $@"INSERT INTO CLIENTS (UserId, Guid, ClientName, SerializedClientAndInspection)  VALUES (@UserId, @Guid, @ClientFullName, @SerializedClientAndInspection)";

            conn.Execute(sql, client);

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public IActionResult GetClientsForUser(string userId)
    {
        using var conn = new SqlConnection(connectionStringStore.SqlConnectionString);

        conn.Open();

        const string sql = "SELECT * FROM CLIENTS WHERE UserId = @UserId";

        var clients = conn.Query<ClientModel>(sql, new { UserId = userId });

        return Ok(clients);
    }

    [HttpPost]
    public IActionResult UpdateClient(ClientModel client)
    {
        try
        {
            using var conn = new SqlConnection(connectionStringStore.SqlConnectionString);

            conn.Open();

            client.SerializedClientAndInspection = JsonConvert.SerializeObject(client);

            const string sql = @"UPDATE CLIENTS SET ClientName = @ClientFullName, SerializedClientAndInspection = @SerializedClientAndInspection WHERE UserId = @UserId AND Guid = @Guid";

            conn.Execute(sql, client);

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public IActionResult DeleteClient(string userId)
    {
        try
        {
            using var conn = new SqlConnection(connectionStringStore.SqlConnectionString);

            conn.Open();

            const string sql = @"DELETE FROM CLIENTS WHERE UserId = @UserId AND Guid = @Guid";

            var rowsAffected = conn.Execute(sql, new { UserId = userId });

            if (rowsAffected > 0)
            {
                return Ok();
            }
            return NotFound(); // Client with the specified UserId was not found
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}
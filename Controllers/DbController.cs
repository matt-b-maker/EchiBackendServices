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
    private readonly ConnectionStringStore _connectionStringStore = connectionStringStore;

    [HttpPost]
    public IActionResult GetClientsForUser()
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult InsertClient(ClientModel client)
    {
        try
        {
            using var conn = new SqlConnection(_connectionStringStore.SqlConnectionString);

            conn.Open();

            client.SerializedThis = JsonConvert.SerializeObject(client);
            client.Guid = Guid.NewGuid().ToString();


            const string sql =
                $@"INSERT INTO CLIENTS (UserId, Guid, ClientName, Inspection)  VALUES (@UserId, @Guid, @ClientFullName, @SerializedThis)";

            conn.Execute(sql, client);

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
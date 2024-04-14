using System.Data.SqlClient;
using System.Text.Json.Serialization;
using EchiBackendServices.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace EchiBackendServices.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class DbController(ConnectionStringStore connectionStringStore, ApplicationDbContext context) : Controller
{
    #region Client Calls

    [HttpPost]
    public async Task<IActionResult> InsertClient(ClientModel client)
    {
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(InsertClient), new {id = client.Id}, client);
    }

    [HttpPost]
    public ActionResult<List<ClientModel>> GetClientsForUser(string userId)
    {
        return context.Clients.Where(c => c.UserId == userId).ToList();
    }

    // PUT: api/clients/{guid}
    [HttpPut("{guid}")]
    public async Task<IActionResult> PutClient(string guid, ClientModel client)
    {
        if (string.IsNullOrEmpty(guid) || guid != client.Guid)
        {
            return BadRequest();
        }

        var existingClient = await context.Clients.FirstOrDefaultAsync(c => c.Guid == guid);

        if (existingClient == null)
        {
            return NotFound();
        }

        context.Entry(existingClient).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClientExists(guid))
            {
                return NotFound();
            }
        }

        return NoContent();
    }

    // DELETE: api/clients/{guid}
    [HttpDelete("{guid}")]
    public async Task<IActionResult> DeleteClient(string guid)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Guid == guid);

        if (client == null)
        {
            return NotFound();
        }

        context.Clients.Remove(client);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool ClientExists(string guid)
    {
        return context.Clients.Any(e => e.Guid == guid);
    }

    #endregion

    #region Agency Stuff

    [HttpPost]
    public ActionResult<List<Agency>> GetAgenciesForUser(string userId)
    {
        return context.Agencies.Where(c => c.UserId == userId).ToList();
    }

    [HttpPost]
    public async Task<IActionResult> InsertAgency(Agency agency)
    {
        context.Agencies.Add(agency);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(InsertAgency), new {id = agency.Id}, agency);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAgency(Agency agency)
    {
        context.Entry(agency).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (agency.AgencyName != null && !AgencyExists(agency.AgencyName))
            {
                return NotFound();
            }
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> InsertAgent(Agent agent)
    {
        context.Agents.Add(agent);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(InsertAgent), new {id = agent.Id}, agent);
    }

    //Put Agent
    [HttpPut]
    public async Task<IActionResult> PutAgent(Agent agent)
    {
        // Fetch the existing agent from the database
        var agentFromDb = await context.Agents.FirstOrDefaultAsync(a => a.AgentName == agent.AgentName);

        // Update the properties of the existing agent
        if (agentFromDb != null)
        {
            agentFromDb.UserId = agent.UserId;
            agentFromDb.AgencyName = agent.AgencyName;
            agentFromDb.AgentName = agent.AgentName;
            agentFromDb.AgentPhoneNumber = agent.AgentPhoneNumber;
            agentFromDb.AgentEmail = agent.AgentEmail;
        }
        else
        {
            return NotFound();
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AgentExists(agent.AgentName))
            {
                return NotFound();
            }
        }

        return Ok();
    }

    [HttpPost]
    public ActionResult<List<Agency>> GetAgentsFromAgency(Agency agency)
    {
        return context.Agencies.Where(c => c.AgencyName == agency.AgencyName).ToList();
    }

    private bool AgencyExists(string agencyName)
    {
        return context.Agencies.Any(e => e.AgencyName == agencyName);
    }

    private bool AgentExists(string agentName)
    {
        return context.Agents.Any(e => e.AgentName == agentName);
    }

    #endregion
}
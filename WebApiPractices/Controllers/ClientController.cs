using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPractices.DTOs;
using WebApiPractices.Entities;
using WebApiPractices.Repositories;

namespace WebApiPractices.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IClientRepository clientRepository;

        public ClientController(ApplicationDbContext context, IMapper mapper, IClientRepository clientRepository)
        {
            this.context = context;
            this.mapper = mapper;
            this.clientRepository = clientRepository;
        }

        [HttpGet]
        public async Task<List<ClientGetDTO>> ClientList()
        {
            var clients = await context.Client.ToListAsync();
            return mapper.Map<List<ClientGetDTO>>(clients);
        }

        [HttpGet("{id}", Name = "GetClientById")]
        public async Task<ActionResult<ClientGetDTO>> ClientById(Guid id)
        {
            var client = await context.Client.FirstOrDefaultAsync(client => client.Id == id);
            if (client == null) return NotFound();
            return mapper.Map<ClientGetDTO>(client);
        }

        [HttpPost]
        public async Task<ActionResult> ClientPost(ClientPostDTO client)
        {
            var clientDB = mapper.Map<Client>(client);
            context.Add(clientDB);
            await context.SaveChangesAsync();
            var clientDTO = mapper.Map<ClientGetDTO>(clientDB);
            return CreatedAtRoute("GetClientById", new { Id = clientDB.Id }, clientDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ClientPut(Guid id, ClientPostDTO client)
        {
            var existClient = await context.Client.AnyAsync(client => client.Id == id);
            if (!existClient) return NotFound();
            var clientDB = mapper.Map<Client>(client);
            clientDB.Id = id;
            context.Update(clientDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> ClientDelete(Guid id)
        {
            var existClient = await context.Client.AnyAsync(client => client.Id == id);
            if (!existClient) return NotFound();
            context.Remove(new Client { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}

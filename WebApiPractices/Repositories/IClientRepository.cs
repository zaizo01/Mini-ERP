using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPractices.DTOs;
using WebApiPractices.Entities;
using WebApiPractices.Utility;

namespace WebApiPractices.Repositories
{
    public interface IClientRepository
    {
        Task<List<ClientGetDTO>> GetClients();
        Task<Result<ClientGetDTO>> GetClient(Guid id);
        Task<ClientPostDTO> AddClient(ClientPostDTO client);
        Task<Result<ClientPostDTO>> UpdateClient(Guid id, ClientPostDTO client);
        Task<string> DeleteClient(Guid id);
    }
}

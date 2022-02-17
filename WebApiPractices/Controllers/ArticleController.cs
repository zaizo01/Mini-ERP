using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPractices.DTOs;

namespace WebApiPractices.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ArticleController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ArticleGetDTO>>> ArticleList()
        {
            var articles = await context.Article.ToListAsync();
            return mapper.Map<List<ArticleGetDTO>>(articles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleGetDTO>> ArticleById(Guid id)
        {
            var article = await context.Article.FirstOrDefaultAsync(article => article.Id == id);
            if (article == null) return NotFound();
            return mapper.Map<ArticleGetDTO>(article);
        }

        [HttpPost]
        public async Task<>
    }
}

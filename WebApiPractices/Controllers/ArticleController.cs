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

        [HttpGet("{id}", Name = "GetArticleById")]
        public async Task<ActionResult<ArticleGetDTO>> ArticleById(Guid id)
        {
            var article = await context.Article.FirstOrDefaultAsync(article => article.Id == id);
            if (article == null) return NotFound();
            return mapper.Map<ArticleGetDTO>(article);
        }

        [HttpPost]
        public async Task<ActionResult> ArticlePost(ArticlePostDTO article)
        {
            var articleDB = mapper.Map<Article>(article);
            context.Add(articleDB);
            await context.SaveChangesAsync();
            var articleDTO = mapper.Map<ArticleGetDTO>(articleDB);
            return CreatedAtRoute("GetArticleById", new { id = articleDB.Id}, articleDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ArticlePut(ArticlePostDTO article, Guid id)
        {
            var articleExist = await context.Article.AnyAsync(article => article.Id == id);
            if (!articleExist) return NotFound();
            var articleDB = mapper.Map<Article>(article);
            articleDB.Id = id;
            context.Update(articleDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> ArticleDelete(Guid id)
        {
            var articleExist = await context.Article.AnyAsync(article => article.Id == id);
            if (!articleExist) return NotFound();
            context.Remove(new Article { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}

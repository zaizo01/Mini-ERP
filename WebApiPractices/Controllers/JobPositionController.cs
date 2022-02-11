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
    public class JobPositionController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public JobPositionController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<JobPositionGetDTO>>> JobPositionList()
        {
            var JobPositions = await context.JobPosition.ToListAsync();
            return mapper.Map<List<JobPositionGetDTO>>(JobPositions);
        }

        [HttpGet("{id}", Name = "JobPositionById")]
        public async Task<ActionResult<JobPositionGetDTO>> JobPositionById(Guid id)
        {
            var JobPosition = await context.JobPosition.FirstOrDefaultAsync(JobPosition => JobPosition.Id == id);
            if (JobPosition == null) return NotFound();
            return mapper.Map<JobPositionGetDTO>(JobPosition);
        }

        [HttpPost]
        public async Task<ActionResult> JobPositionPost(JobPositionPostDTO JobPosition)
        {
            var jobPosition = mapper.Map<JobPosition>(JobPosition);
            context.Add(jobPosition);
            await context.SaveChangesAsync();
            var JobPositionDTO = mapper.Map<JobPositionPostDTO>(jobPosition);
            return CreatedAtRoute("JobPositionById", new { id = jobPosition.Id }, JobPositionDTO);
        }

        [HttpPut]
        public async Task<ActionResult> JobPositionPut(JobPositionPostDTO JobPosition, Guid id)
        {
            var JobPositionExist = await context.JobPosition.AnyAsync(JobPosition => JobPosition.Id == id);
            if (!JobPositionExist) return NoContent();
            var JobPositionDB = mapper.Map<JobPosition>(JobPosition);
            JobPositionDB.Id = id;
            context.Update(JobPositionDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> JobPositionDelete(Guid id)
        {
            var JobPositionExist = await context.JobPosition.AnyAsync(JobPosition => JobPosition.Id == id);
            if (!JobPositionExist) return NotFound();
            context.Remove(new JobPosition { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}

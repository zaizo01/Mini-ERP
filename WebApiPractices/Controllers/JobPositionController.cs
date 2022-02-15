using AutoMapper;
using LogDB;
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
        private readonly ILoggerManager logger;

        public JobPositionController(ApplicationDbContext context, IMapper mapper, ILoggerManager logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<JobPositionGetDTO>>> JobPositionList()
        {
            try
            {
                var JobPositions = await context.JobPosition.ToListAsync();
                return mapper.Map<List<JobPositionGetDTO>>(JobPositions);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the JobPositionList: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "JobPositionById")]
        public async Task<ActionResult<JobPositionGetDTO>> JobPositionById(Guid id)
        {
            try
            {
                var JobPosition = await context.JobPosition
                    .Include(x => x.Employees)
                    .ThenInclude(x => x.Department)
                    .FirstOrDefaultAsync(JobPosition => JobPosition.Id == id);
                if (JobPosition == null) return NotFound();
                return mapper.Map<JobPositionGetDTO>(JobPosition);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the JobPositionById: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<JobPositionWithEmployeesDTO>>> JobPositionListWithEmployees()
        {
            try
            {
                var JobPositions = await context.JobPosition
                    .Include(x => x.Employees)
                    .ThenInclude(x => x.Department)
                    .ToListAsync();
                return mapper.Map<List<JobPositionWithEmployeesDTO>>(JobPositions);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the JobPositionListWithEmployees: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobPositionWithEmployeesDTO>> JobPositionWithEmployeesById(Guid id)
        {
            try
            {
                var JobPosition = await context.JobPosition
                    .Include(x => x.Employees)
                    .ThenInclude(x => x.Department)
                    .FirstOrDefaultAsync();
                if (JobPosition == null) return NotFound();
                return mapper.Map<JobPositionWithEmployeesDTO>(JobPosition);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the JobPositionWithEmployeesById: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> JobPositionPost(JobPositionPostDTO JobPosition)
        {
            try
            {
                var jobPosition = mapper.Map<JobPosition>(JobPosition);
                context.Add(jobPosition);
                await context.SaveChangesAsync();
                var JobPositionDTO = mapper.Map<JobPositionPostDTO>(jobPosition);
                return CreatedAtRoute("JobPositionById", new { id = jobPosition.Id }, JobPositionDTO);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the JobPositionPost: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<ActionResult> JobPositionPut(JobPositionPostDTO JobPosition, Guid id)
        {
            try
            {
                var JobPositionExist = await context.JobPosition.AnyAsync(JobPosition => JobPosition.Id == id);
                if (!JobPositionExist) return NoContent();
                var JobPositionDB = mapper.Map<JobPosition>(JobPosition);
                JobPositionDB.Id = id;
                context.Update(JobPositionDB);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the JobPositionPut: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> JobPositionDelete(Guid id)
        {
            try
            {
                var JobPositionExist = await context.JobPosition.AnyAsync(JobPosition => JobPosition.Id == id);
                if (!JobPositionExist) return NotFound();
                context.Remove(new JobPosition { Id = id });
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the JobPositionDelete: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

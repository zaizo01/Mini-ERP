using AutoMapper;
using LoggerService;
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
    [Route("api/Department/[action]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ILoggerManager logger;

        public DepartmentController(ApplicationDbContext context, IMapper mapper, ILoggerManager logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<List<DepartmentGetDTO>>> DepartmentList()
        {
            try
            {
                var departments = await context.Department.ToListAsync();
                return mapper.Map<List<DepartmentGetDTO>>(departments);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the DepartmentList: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "DeparmentById")]
        public async Task<ActionResult<DepartmentGetDTO>> DepartmentById(Guid id)
        {
            try
            {
                var department = await context.Department.FirstOrDefaultAsync(department => department.Id == id);
                if (department == null) return NotFound();
                return mapper.Map<DepartmentGetDTO>(department);  
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the DepartmentById: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<DepartmentWithEmployeesDTO>>> DepartmentListWithEmployees()
        {
            try
            {
                var departments = await context.Department
                    .Include(x => x.Employees)
                    .ThenInclude(x => x.JobPosition)
                    .ToListAsync();
                return mapper.Map<List<DepartmentWithEmployeesDTO>>(departments);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the DepartmentListWithEmployees: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentWithEmployeesDTO>> DepartmentWithEmployeesById(Guid id)
        {
            try
            {
                var department = await context.Department
                    .Include(x => x.Employees)
                    .ThenInclude(x => x.JobPosition)
                    .FirstOrDefaultAsync();
                if (department == null) return NotFound();
                return mapper.Map<DepartmentWithEmployeesDTO>(department);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the DepartmentWithEmployeesById: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DepartmentPost(DepartmentPostDTO department)
        {
            try
            {
                var departmentDB = mapper.Map<Department>(department);
                context.Add(departmentDB);
                await context.SaveChangesAsync();
                var departmentDTO = mapper.Map<DepartmentGetDTO>(departmentDB);
                return CreatedAtRoute("DeparmentById", new { id = departmentDB.Id }, departmentDTO);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the DepartmentPost: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<ActionResult> DepartmentPut(DepartmentPostDTO department, Guid id)
        {
            try
            {
                var departmentExist = await context.Department.AnyAsync(department => department.Id == id);
                if (!departmentExist) return NotFound();
                var departmentDB = mapper.Map<Department>(department);
                departmentDB.Id = id;
                context.Update(departmentDB);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the DepartmentPut: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DepartmentDelete(Guid id)
        {
            try
            {
                var departmentExist = await context.Department.AnyAsync(department => department.Id == id);
                if (!departmentExist) return NotFound();
                context.Remove(new Department { Id = id });
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the DepartmentDelete: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

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
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ILoggerManager logger;

        public EmployeeController(ApplicationDbContext context, IMapper mapper, ILoggerManager logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeGetDTO>>> EmployeeList()
        {
            try
            {
                var employees = await context.Employee
                    .Include(x => x.Department)
                    .Include(x => x.JobPosition)
                    .ToListAsync();
                return mapper.Map<List<EmployeeGetDTO>>(employees);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the EmployeeList: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "GetEmployeeById")]
        public async Task<ActionResult<EmployeeGetDTO>> EmployeeById(Guid id)
        {
            try
            {
                var employee = await context.Employee.FirstOrDefaultAsync(employee => employee.Id == id);
                if (employee == null) return NotFound();
                return mapper.Map<EmployeeGetDTO>(employee); 
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the EmployeeById: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EmployeePost(EmployeePostDTO employee)
        {
            try
            {
                var employeeDB = mapper.Map<Employee>(employee);
                context.Add(employeeDB);
                await context.SaveChangesAsync();
                var employeeDTO = mapper.Map<EmployeeGetDTO>(employeeDB);
                return CreatedAtRoute("GetEmployeeById", new { Id = employeeDB.Id }, employeeDTO);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the EmployeePost: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> EmployeePut(EmployeePostDTO employee, Guid id)
        {
            try
            {
                var employeeExist = await context.Employee.AnyAsync(employee => employee.Id == id);
                if (!employeeExist) return NotFound();
                var employeeDB = mapper.Map<Employee>(employee);
                employeeDB.Id = id;
                context.Update(employeeDB);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside the EmployeePut: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> EmployeeDelete(Guid id)
        {
            try
            {
                var employeeExist = await context.Employee.AnyAsync(employee => employee.Id == id);
                if (!employeeExist) return NotFound();
                context.Remove(new Employee { Id = id });
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {

                logger.LogError($"Something went wrong inside the EmployeeDelete: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

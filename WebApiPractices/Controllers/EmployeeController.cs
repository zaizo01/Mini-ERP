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
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public EmployeeController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeGetDTO>>> EmployeeList()
        {
            var employees = await context.Employee
                .Include(x => x.Department)
                .Include(x => x.JobPosition)
                .ToListAsync();
            return mapper.Map<List<EmployeeGetDTO>>(employees);
        }

        [HttpGet("{id}", Name = "GetEmployeeById")]
        public async Task<ActionResult<EmployeeGetDTO>> EmployeeById(Guid id)
        {
            var employee = await context.Employee.FirstOrDefaultAsync(employee => employee.Id == id);
            if (employee == null) return NotFound();
            return mapper.Map<EmployeeGetDTO>(employee); 
        }

        [HttpPost]
        public async Task<ActionResult> EmployeePost(EmployeePostDTO employee)
        {
            var employeeDB = mapper.Map<Employee>(employee);
            context.Add(employeeDB);
            await context.SaveChangesAsync();
            var employeeDTO = mapper.Map<EmployeeGetDTO>(employeeDB);
            return CreatedAtRoute("GetEmployeeById", new { Id = employeeDB.Id }, employeeDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> EmployeePut(EmployeePostDTO employee, Guid id)
        {
            var employeeExist = await context.Employee.AnyAsync(employee => employee.Id == id);
            if (!employeeExist) return NotFound();
            var employeeDB = mapper.Map<Employee>(employee);
            employeeDB.Id = id;
            context.Update(employeeDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> EmployeeDelete(Guid id)
        {
            var employeeExist = await context.Employee.AnyAsync(employee => employee.Id == id);
            if (!employeeExist) return NotFound();
            context.Remove(new Employee { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}

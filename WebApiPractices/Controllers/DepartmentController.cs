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
    [Route("api/Department/[action]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public DepartmentController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<DepartmentGetDTO>>> DepartmentList()
        {
            var departments = await context.Department.ToListAsync();
            return mapper.Map<List<DepartmentGetDTO>>(departments);
        }

        [HttpGet("{id}", Name = "DeparmentById")]
        public async Task<ActionResult<DepartmentGetDTO>> DepartmentById(Guid id)
        {
            var department = await context.Department.FirstOrDefaultAsync(department => department.Id == id);
            if (department == null) return NotFound();
            return mapper.Map<DepartmentGetDTO>(department);  
        }

        [HttpPost]
        public async Task<ActionResult> DepartmentPost(DepartmentPostDTO department)
        {
            var deparment = mapper.Map<Department>(department);
            context.Add(deparment);
            await context.SaveChangesAsync();
            var departmentDTO = mapper.Map<DepartmentGetDTO>(deparment);
            return CreatedAtRoute("", new { id = deparment.Id }, departmentDTO);
        }

        [HttpPut]
        public async Task<ActionResult> DepartmentPut(DepartmentPostDTO department, Guid id)
        {
            var departmentExist = await context.Department.AnyAsync(department => department.Id == id);
            if (!departmentExist) return NoContent();
            var departmentDB = mapper.Map<Department>(department);
            departmentDB.Id = id;
            context.Update(departmentDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DepartmentDelete(Guid id)
        {
            var departmentExist = await context.Department.AnyAsync(department => department.Id == id);
            if (!departmentExist) return NotFound();
            context.Remove(new Department { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPractices.DTOs;
using WebApiPractices.Entities;

namespace WebApiPractices.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<IdentityUser, UserGetDTO>().ReverseMap();

            CreateMap<Department, DepartmentGetDTO>().ReverseMap();
            CreateMap<Department, DepartmentEmployeeDTO>().ReverseMap();
            CreateMap<Department, DepartmentWithEmployeesDTO>().ReverseMap();
            CreateMap<Department, DepartmentPostDTO>().ReverseMap();

            CreateMap<JobPosition, JobPositionGetDTO>().ReverseMap();
            CreateMap<JobPosition, JobPositionEmployeeDTO>().ReverseMap();
            CreateMap<JobPosition, JobPositionWithEmployeesDTO>().ReverseMap();
            CreateMap<JobPosition, JobPositionPostDTO>().ReverseMap();

            CreateMap<Employee, EmployeeGetDTO>().ReverseMap();
            CreateMap<Employee, EmployeeJobPostionDTO>().ReverseMap();
            CreateMap<Employee, EmployeeDepartmentDTO>().ReverseMap();
            CreateMap<Employee, EmployeePostDTO>().ReverseMap();

            CreateMap<Client, ClientGetDTO>().ReverseMap();
            CreateMap<Client, ClientPostDTO>().ReverseMap();
        }
    }
}

using api.DTO;
using api.Models;
using AutoMapper;

namespace api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Project, ProjectPreviewDTO>().ReverseMap();
            CreateMap<ProjectMember, ProjectMemberDTO>()
            .ForMember(n => n.Id, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(n => n.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(n => n.LastName, opt => opt.MapFrom(src => src.User.LastName));
            CreateMap<User, ProjectMemberDTO>().ReverseMap();
            CreateMap<ProjectDepartment, ProjectDepartmentDTO>()
            .ForMember(n => n.DepartmentId, opt => opt.MapFrom(src => src.Department.DepartmentId))
            .ForMember(n => n.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName))
            .ForMember(n => n.Color, opt => opt.MapFrom(src => src.Department.Color));
            CreateMap<Department, ProjectDepartmentDTO>().ReverseMap();


            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Department, DepartmentDTO>().ReverseMap();
        }
    }
}
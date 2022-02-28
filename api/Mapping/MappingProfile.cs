using api.DTO;
using api.Models;
using AutoMapper;

namespace api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // PROJECT DTO

            CreateMap<Project, ProjectPreviewDTO>();
            CreateMap<ProjectMember, ProjectMemberDTO>()
            .ForMember(n => n.UserId, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(n => n.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(n => n.LastName, opt => opt.MapFrom(src => src.User.LastName));
            CreateMap<User, ProjectMemberDTO>().ReverseMap();
            CreateMap<ProjectDepartment, ProjectDepartmentDTO>()
            .ForMember(n => n.DepartmentId, opt => opt.MapFrom(src => src.Department.DepartmentId))
            .ForMember(n => n.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName))
            .ForMember(n => n.Color, opt => opt.MapFrom(src => src.Department.Color));
            CreateMap<Department, ProjectDepartmentDTO>().ReverseMap();
            CreateMap<Project, ProjectDTO>().ReverseMap();

            // DEPARTMENT DTO

            CreateMap<Department, DepartmentPreviewDTO>().ReverseMap();
            CreateMap<Department, DepartmentDTO>().ReverseMap();
            CreateMap<DepartmentMember, DepartmentMemberDTO>()
            .ForMember(n => n.UserId, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(n => n.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(n => n.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(n => n.Email, opt => opt.MapFrom(src => src.User.Email));
            CreateMap<User, DepartmentMemberDTO>().ReverseMap();

            // CARD DTO

            CreateMap<Card, CardDTO>().ReverseMap();
            CreateMap<CardMember, CardMemberDTO>()
            .ForMember(n => n.UserId, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(n => n.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(n => n.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(n => n.Email, opt => opt.MapFrom(src => src.User.Email));
            CreateMap<User, CardMemberDTO>().ReverseMap();

            //
            
            CreateMap<Sprint, SprintDTO>().ReverseMap();
            CreateMap<SprintList, SprintListDTO>().ReverseMap();
            CreateMap<UserRegistrationModel, User>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();

        }
    }
}
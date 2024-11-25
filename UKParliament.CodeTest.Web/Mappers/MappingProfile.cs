
using AutoMapper;
using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Web.ViewModels;

namespace UKParliament.CodeTest.Services
{
 
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // note: in the PersonViewModel the DOB is a string, and DateOnly in Person model
            CreateMap<PersonViewModel, Person>()
               .ForMember(dest => dest.DOB, opt => opt.MapFrom(src =>
                   string.IsNullOrWhiteSpace(src.DOB) ? (DateOnly?)null : DateOnly.Parse(src.DOB)));

            CreateMap<Person, PersonViewModel>()
                .ForMember(dest => dest.DOB, opt => opt.MapFrom(src => src.DOB.ToString("yyyy-MM-dd")));

            CreateMap<Department, DepartmentViewModel>();
        }
    }
}

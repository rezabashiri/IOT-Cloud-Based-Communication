using AutoMapper;
using Broker.Application.MachineManagement.DTOs;
using Broker.Domain.Entities;

namespace Broker.Application.MachineManagement.Mappings;

public class MachineProfile : Profile
{
    public MachineProfile()
    {
        CreateMap<Machine, MachineDto>()
            .ConstructUsing(src => new MachineDto(src.Id, src.Name, src.Status))
            .ForAllMembers(x => x.Ignore());
    }
}
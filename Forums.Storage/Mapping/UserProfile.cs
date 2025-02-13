using AutoMapper;
using Forums.Domain.UseCases.SignIn;
using Forums.Storage.Entities;

namespace Forums.Storage.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, RecognisedUser>();
        CreateMap<Session, Domain.Authentication.Session>();
    }
}
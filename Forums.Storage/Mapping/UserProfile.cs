using AutoMapper;
using Forums.Domain.UseCases.SignIn;

namespace Forums.Storage.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, RecognisedUser>();
    }
}
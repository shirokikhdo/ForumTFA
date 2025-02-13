﻿using FluentValidation;
using FluentValidation.Results;
using Forums.Domain.Authentication;
using Forums.Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace Forums.Domain.UseCases.SignIn;

internal class SignInUseCase : ISignInUseCase
{
    private readonly IValidator<SignInCommand> _validator;
    private readonly ISignInStorage _storage;
    private readonly IPasswordManager _passwordManager;
    private readonly ISymmetricEncryptor _encryptor;
    private readonly AuthenticationConfiguration _configuration;

    public SignInUseCase(
        IValidator<SignInCommand> validator,
        ISignInStorage storage,
        IPasswordManager passwordManager,
        ISymmetricEncryptor encryptor,
        IOptions<AuthenticationConfiguration> options)
    {
        _validator = validator;
        _storage = storage;
        _passwordManager = passwordManager;
        _encryptor = encryptor;
        _configuration = options.Value;
    }

    public async Task<(IIdentity identity, string token)> Execute(
        SignInCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var recognisedUser = await _storage.FindUser(command.Login, cancellationToken);
        
        if (recognisedUser is null)
            throw new ValidationException(new ValidationFailure[]
            {
                new()
                {
                    PropertyName = nameof(command.Login),
                    ErrorCode = ValidationErrorCode.INVALID,
                    AttemptedValue = command.Login
                }
            });

        var passwordMatches = _passwordManager.ComparePasswords(
            command.Password, recognisedUser.Salt, recognisedUser.PasswordHash);
        
        if (!passwordMatches)
            throw new ValidationException(new ValidationFailure[]
            {
                new()
                {
                    PropertyName = nameof(command.Password),
                    ErrorCode = ValidationErrorCode.INVALID,
                    AttemptedValue = command.Password
                }
            });

        var sessionId = await _storage.CreateSession(
            recognisedUser.UserId, 
            DateTimeOffset.UtcNow + TimeSpan.FromHours(1), 
            cancellationToken);

        var token = await _encryptor.Encrypt(
            sessionId.ToString(), 
            _configuration.Key, 
            cancellationToken);

        return (new User(recognisedUser.UserId, sessionId), token);
    }
}
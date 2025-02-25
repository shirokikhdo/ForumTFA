﻿using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Forums.Domain.Authorization;
using Forums.Domain.Exceptions;

namespace Forums.API.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext httpContext,
        ILogger<ErrorHandlingMiddleware> logger,
        ProblemDetailsFactory problemDetailsFactory)
    {
        try
        {
            await _next.Invoke(httpContext);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Error has happened with {RequestPath}, the message is {ErrorMessage}",
                httpContext.Request.Path.Value, exception.Message);

            ProblemDetails problemDetails;

            switch (exception)
            {
                case IntentionManagerException intentionManagerException:
                    problemDetails = problemDetailsFactory.CreateFrom(httpContext, intentionManagerException);
                    break;
                case ValidationException validationException:
                    problemDetails = problemDetailsFactory.CreateFrom(httpContext, validationException);
                    logger.LogInformation(validationException, "Somebody sent invalid request, oops");
                    break;
                case DomainException domainException:
                    problemDetails = problemDetailsFactory.CreateFrom(httpContext, domainException);
                    logger.LogError(domainException, "Domain exception occured");
                    break;
                default:
                    problemDetails = problemDetailsFactory.CreateProblemDetails(
                        httpContext, StatusCodes.Status500InternalServerError, "Unhandled error! Please contact us.");
                    logger.LogError(exception, "Unhandled exception occured");
                    break;
            }

            httpContext.Response.StatusCode = problemDetails.Status 
                                              ?? StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, problemDetails.GetType());
        }
    }
}
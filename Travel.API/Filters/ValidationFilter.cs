using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Travel.BLL.DTOs.Common;

namespace Travel.API.Filters;

/// <summary>
/// Runs any registered FluentValidation IValidator&lt;T&gt; against matching
/// action arguments before the action executes, and short-circuits with a
/// 400 in the platform-wide ApiResponse envelope on failure. This keeps
/// controllers free of repeated "if (!ModelState.IsValid)" boilerplate.
/// </summary>
public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (_serviceProvider.GetService(validatorType) is not IValidator validator)
                continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                context.Result = new BadRequestObjectResult(
                    ApiResponse<object>.Fail("Validation failed.", errors));
                return;
            }
        }

        await next();
    }
}

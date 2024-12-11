using System.Linq;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.Core.Exceptions;

namespace Shared.Infrastructure.Interceptors
{
    public class ValidatorInterceptor : IValidatorInterceptor
    {
        private readonly IStringLocalizer<ValidatorInterceptor> _localizer;

        public ValidatorInterceptor(IStringLocalizer<ValidatorInterceptor> localizer)
        {
            _localizer = localizer;
        }

        public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
        {
            return commonContext;
        }

        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        {
            var failures = result.Errors.Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                var errorMessages = failures.Select(a => a.ErrorMessage).Distinct().ToList();
                throw new CustomValidationException(_localizer, errorMessages);
            }

            return result;
        }
    }
}
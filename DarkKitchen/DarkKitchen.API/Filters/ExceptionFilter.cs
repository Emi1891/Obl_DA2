using DarkKitchen.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.API.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly Dictionary<Type, IActionResult> errors = new()
    {
        {
            typeof(BadRequestException),
            new ObjectResult(new
            {
                Message = "An error occurred: "
            })
            {
                StatusCode = 400
            }
        },
        {
            typeof(UnauthorizedException),
            new ObjectResult(new
            {
                Message = "Unauthorized access: "
            })
            {
                StatusCode = 401
            }
        },
        {
            typeof(AccessDeniedException),
            new ObjectResult(new
            {
                Message = "Not allowed to access this resource: "
            })
            {
                StatusCode = 403
            }
        },
        {
            typeof(NotFoundException),
            new ObjectResult(new
            {
                Message = "Resource not found: "
            })
            {
                StatusCode = 404
            }
        }
    };

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var exceptrionType = exception.GetType();

        var error = errors.GetValueOrDefault(exceptrionType);
        if(error == null)
        {
            context.Result = new ObjectResult(new
            {
                Message = "Internal server error" + ": " + exception.Message
            })
            {
                StatusCode = 500
            };
        }
        else
        {
            var objectResult = (ObjectResult)error;
            context.Result = new ObjectResult(new
            {
                Message = ((dynamic)objectResult.Value!).Message + exception.Message
            })
            {
                StatusCode = objectResult.StatusCode
            };
        }
    }
}

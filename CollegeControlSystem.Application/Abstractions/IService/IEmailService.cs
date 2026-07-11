using Microsoft.AspNetCore.Http;

namespace CollegeControlSystem.Application.Abstractions.IService;

public interface IEmailService
{
    Task SendMailAsync(string mailTo, string subject, string body, IList<IFormFile>? files = null);

}
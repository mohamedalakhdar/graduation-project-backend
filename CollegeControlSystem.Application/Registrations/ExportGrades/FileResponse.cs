namespace CollegeControlSystem.Application.Registrations.ExportGrades
{
    public sealed record FileResponse(string FileName, string ContentType, byte[] Content);
}

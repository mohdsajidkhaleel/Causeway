namespace CargoManagement.Services.Extensions
{
    public static class HttpContextExtensions
    {
        public static string? GetClaim(this HttpContext context, string claim)
        {
            string? value= context.User.Claims.FirstOrDefault(c => c.Type == claim)?.Value;
            return value==""?null:value;
        }
    }
}

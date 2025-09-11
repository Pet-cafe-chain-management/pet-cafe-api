namespace PetCafe.Application.Services.Commons
{
    public interface IClaimsService
    {
        public Guid GetCurrentUser { get; }
        public string GetCurrentUserRole { get; }
    }
}

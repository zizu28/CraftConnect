using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace UserManagement.Domain.Entities
{
    public class IdentityDocument : Entity
    {
        public string Type { get; private set; }
        public string Number { get; private set; }
        public DateTime IssuedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public IdentityDocument(string type, string number, DateTime issuedAt, DateTime expiresAt)
        {
            Type = type;
            Number = number;
            IssuedAt = issuedAt;
            ExpiresAt = expiresAt;
        }
    }
}

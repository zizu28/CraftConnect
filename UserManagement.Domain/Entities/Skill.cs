using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace UserManagement.Domain.Entities
{
    public class Skill : Entity
    {
        public string Name { get; private set; }
        public int YearsOfExperience { get; private set; }
        public Skill(string name, int yearsOfExperience)
        {
            Name = name;
            YearsOfExperience = yearsOfExperience;
        }
    }
}

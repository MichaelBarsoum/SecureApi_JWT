using System.Runtime.Serialization;

namespace Secure_Api_Using_JWT.Helpers
{
    public enum UserRoles
    {
        [EnumMember(Value ="USER")]
        USER,
        [EnumMember(Value = "ADMIN")]
        ADMIN
    }
}

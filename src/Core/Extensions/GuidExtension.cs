namespace Common.Extensions
{
    public static class GuidExtension
    {
        public static Guid SystemUserId(this Guid guid)
        {
            return Guid.Parse("00000000-0000-0000-0000-000000000001");
        }
    }
}

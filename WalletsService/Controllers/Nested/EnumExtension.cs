using System.ComponentModel;

namespace WalletsService.Controllers.Nested
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum obj)
        {
            var fi = obj.GetType().GetField(obj.ToString());

            if (fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) 
                    is DescriptionAttribute[] attributes && attributes.Any())
            {
                return attributes.First().Description;
            }

            return obj.ToString();
        }
    }
}
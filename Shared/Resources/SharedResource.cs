using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace Shared.Resources
{
    public static class SharedResource
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager("Shared.Resources.SharedResource", typeof(SharedResource).Assembly);

        public static string GetString(string name)
        {
            return _resourceManager.GetString(name) ?? name;
        }

        public static string GetString(string name, params object[] args)
        {
            return string.Format(_resourceManager.GetString(name) ?? name, args);
        }
    }

    public class LocalizedRequiredAttribute : RequiredAttribute
    {
        public LocalizedRequiredAttribute(string resourceName)
        {
            ErrorMessage = SharedResource.GetString(resourceName);
        }
    }

    public class LocalizedMaxLengthAttribute : MaxLengthAttribute
    {
        public LocalizedMaxLengthAttribute(int length, string resourceName) : base(length)
        {
            ErrorMessage = SharedResource.GetString(resourceName, length);
        }
    }

    public class LocalizedRegularExpressionAttribute : RegularExpressionAttribute
    {
        public LocalizedRegularExpressionAttribute(string pattern, string resourceName) : base(pattern)
        {
            ErrorMessage = SharedResource.GetString(resourceName);
        }
    }

    public class LocalizedRangeAttribute : RangeAttribute
    {
        public LocalizedRangeAttribute(double minimum, double maximum, string resourceName) : base(minimum, maximum)
        {
            ErrorMessage = SharedResource.GetString(resourceName);
        }

        public LocalizedRangeAttribute(int minimum, int maximum, string resourceName) : base(minimum, maximum)
        {
            ErrorMessage = SharedResource.GetString(resourceName);
        }
    }
} 
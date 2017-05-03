namespace MyCC.Core.Settings
{
    public class Version
    {
        public readonly int Major;
        public readonly int Minor;
        public readonly int Build;

        public Version(string version)
        {
            var parts = version.Split('.');

            Major = int.Parse(parts[0]);
            Minor = int.Parse(parts[1]);
            Build = parts.Length > 2 ? int.Parse(parts[2]) : 0;
        }

        public Version(int major, int minor, int? build = null)
        {
            Major = major;
            Minor = minor;
            Build = build ?? 0;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}{(Build != 0 ? $".{Build}" : string.Empty)}";
        }

        public override bool Equals(object obj)
        {
            return obj is Version && ToString().Equals(obj.ToString());
        }

        public override int GetHashCode()
        {
            return Major.GetHashCode() + Minor.GetHashCode() + Build.GetHashCode();
        }

        public static bool operator >(Version v1, Version v2)
        {
            v1 = v1 ?? new Version(0, 0, 0);
            v2 = v2 ?? new Version(0, 0, 0);

            return v1.Major > v2.Major ||
                     v1.Major == v2.Major && v1.Minor > v2.Minor ||
                     v1.Major == v2.Major && v1.Minor == v2.Minor && v1.Build > v2.Build;
        }

        public static bool operator <(Version v1, Version v2)
        {
            v1 = v1 ?? new Version(0, 0, 0);
            v2 = v2 ?? new Version(0, 0, 0);

            return v1.Major < v2.Major ||
                     v1.Major == v2.Major && v1.Minor < v2.Minor ||
                     v1.Major == v2.Major && v1.Minor == v2.Minor && v1.Build < v2.Build;
        }

        public static bool operator ==(Version v1, Version v2)
        {
            return v1?.Major == v2?.Major && v1?.Minor == v2?.Minor && v1?.Build == v2?.Build;
        }

        public static bool operator !=(Version v1, Version v2)
        {
            return v1?.Major != v2?.Major || v1?.Minor != v2?.Minor || v1?.Build != v2?.Build;
        }
    }
}

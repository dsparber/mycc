namespace MyCC.Core.Settings
{
    public class CoreVersion
    {
        private readonly int _major;
        private readonly int _minor;
        private readonly int _build;

        public CoreVersion(string version)
        {
            var parts = version.Split('.');

            _major = int.Parse(parts[0]);
            _minor = int.Parse(parts[1]);
            _build = parts.Length > 2 ? int.Parse(parts[2]) : 0;
        }

        public CoreVersion(int major, int minor, int? build = null)
        {
            _major = major;
            _minor = minor;
            _build = build ?? 0;
        }

        public override string ToString()
        {
            return $"{_major}.{_minor}{(_build != 0 ? $".{_build}" : string.Empty)}";
        }

        public override bool Equals(object obj)
        {
            return obj is CoreVersion && ToString().Equals(obj.ToString());
        }

        public override int GetHashCode()
        {
            return _major.GetHashCode() + _minor.GetHashCode() + _build.GetHashCode();
        }

        public static bool operator >(CoreVersion v1, CoreVersion v2)
        {
            v1 = v1 ?? new CoreVersion(0, 0, 0);
            v2 = v2 ?? new CoreVersion(0, 0, 0);

            return v1._major > v2._major ||
                     v1._major == v2._major && v1._minor > v2._minor ||
                     v1._major == v2._major && v1._minor == v2._minor && v1._build > v2._build;
        }

        public static bool operator <(CoreVersion v1, CoreVersion v2)
        {
            v1 = v1 ?? new CoreVersion(0, 0, 0);
            v2 = v2 ?? new CoreVersion(0, 0, 0);

            return v1._major < v2._major ||
                     v1._major == v2._major && v1._minor < v2._minor ||
                     v1._major == v2._major && v1._minor == v2._minor && v1._build < v2._build;
        }

        public static bool operator ==(CoreVersion v1, CoreVersion v2)
        {
            return v1?._major == v2?._major && v1?._minor == v2?._minor && v1?._build == v2?._build;
        }

        public static bool operator !=(CoreVersion v1, CoreVersion v2)
        {
            return v1?._major != v2?._major || v1?._minor != v2?._minor || v1?._build != v2?._build;
        }
    }
}

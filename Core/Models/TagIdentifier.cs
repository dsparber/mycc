using Xamarin.Forms;

namespace MyCryptos.Core.Models
{
    public class TagIdentifier : Persistable<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TagIdentifier))
            {
                return false;
            }

            var identifier = (TagIdentifier)obj;

            return identifier.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Id: {Id}, Name:T {Name}]";
        }
    }
}


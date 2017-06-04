namespace MyCC.Core.Rates.Models
{
    public class RateDescriptor
    {
        public readonly string Id;

        public readonly string ReferenceCurrencyId;
        public readonly string SecondaryCurrencyId;

        public RateDescriptor(string referenceCurrencyId, string secondaryCurrencyId)
        {
            ReferenceCurrencyId = referenceCurrencyId;
            SecondaryCurrencyId = secondaryCurrencyId;
            Id = $"{ReferenceCurrencyId}/{secondaryCurrencyId}";
        }


        public override bool Equals(object obj) => Id.Equals((obj as RateDescriptor)?.Id);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Id;
    }
}
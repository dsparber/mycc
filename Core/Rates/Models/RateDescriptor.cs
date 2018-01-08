using MyCC.Core.Rates.ModelExtensions;

namespace MyCC.Core.Rates.Models
{
    public class RateDescriptor
    {
        internal readonly string Id;

        public readonly string ReferenceCurrencyId;
        public readonly string SecondaryCurrencyId;

        public RateDescriptor(string referenceCurrencyId, string secondaryCurrencyId)
        {
            ReferenceCurrencyId = referenceCurrencyId;
            SecondaryCurrencyId = secondaryCurrencyId;
            Id = $"{ReferenceCurrencyId}/{secondaryCurrencyId}";
        }

        public bool CurrenciesEqual(RateDescriptor rateDescriptor) =>
        (rateDescriptor.ReferenceCurrencyId.Equals(ReferenceCurrencyId) &&
         rateDescriptor.SecondaryCurrencyId.Equals(SecondaryCurrencyId)) || 
        (rateDescriptor.SecondaryCurrencyId.Equals(ReferenceCurrencyId) &&
         rateDescriptor.ReferenceCurrencyId.Equals(SecondaryCurrencyId));

        public override bool Equals(object obj) => Id.Equals((obj as RateDescriptor)?.Id);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Id;
    }
}
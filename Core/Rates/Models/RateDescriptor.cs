﻿using MyCC.Core.Rates.ModelExtensions;

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
            rateDescriptor.ContainsCurrency(ReferenceCurrencyId) &&
            rateDescriptor.ContainsCurrency(SecondaryCurrencyId);

        public override bool Equals(object obj) => Id.Equals((obj as RateDescriptor)?.Id);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Id;
    }
}
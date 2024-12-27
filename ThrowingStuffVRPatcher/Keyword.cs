// Autogenerated by https://github.com/Mutagen-Modding/Mutagen.Bethesda.FormKeys

using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.FormKeys.SkyrimSE;

public static partial class Skyrim
{
    public static partial class Keyword
    {
        private static FormLink<IKeywordGetter> Construct(uint id) => new FormLink<IKeywordGetter>(ModKey.MakeFormKey(id));
        public static FormLink<IKeywordGetter> ReusableSoulGem => Construct(0xed2f1);
        public static FormLink<IKeywordGetter> VendorItemSoulGem => Construct(0x937a3);
        public static FormLink<IKeywordGetter> VendorItemPoison => Construct(0x8cded);
        public static FormLink<IKeywordGetter> VendorItemPotion => Construct(0x8cdec);
        public static FormLink<IKeywordGetter> VendorItemIngredient => Construct(0x8cdeb);
        public static FormLink<IKeywordGetter> VendorItemFood => Construct(0x8cdea);
        public static FormLink<IKeywordGetter> ActorTypeDwarven => Construct(0x1397a);
        public static FormLink<IKeywordGetter> ActorTypeUndead => Construct(0x13796);
    }
}

public static partial class ThrowStuff
{
    public static partial class Keyword
    {
        private static FormLink<IKeywordGetter> Construct(uint id) => new FormLink<IKeywordGetter>(ModKey.MakeFormKey(id));

        public static FormLink<IKeywordGetter> onmoPurse => Construct(0x802);
    }
}

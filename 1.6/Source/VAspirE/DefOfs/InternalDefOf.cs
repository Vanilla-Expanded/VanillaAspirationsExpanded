using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace VAspirE
{

    [DefOf]
    public static class InternalDefOf
    {
        static InternalDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
        }

        public static TraitDef Beauty;
        public static ThoughtDef VAE_WitnessedFleshShaping;

        [MayRequire("vanillaracesexpanded.android")]
        public static GeneDef VREA_Power;
    }
}
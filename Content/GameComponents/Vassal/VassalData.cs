using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplomacy.Content.GameComponents.Vassal
{
    public struct VassalData
    {
        public VassalData(int taxationCooldown, int loyalty)
        {
            TaxationCooldown = taxationCooldown;

            Loyalty = loyalty;
        }

        public int TaxationCooldown { get; set; }

        public int Loyalty { get; set; }
    }
}

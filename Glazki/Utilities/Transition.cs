using Glazki.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Glazki.Utilities
{
    internal class Transition
    {
        public static Frame MainFrame { get; set; }

        private static GlazkiEntities context;
        public static GlazkiEntities Context
        {
            get
            {
                if (context == null)
                    context = new GlazkiEntities();

                return context;
            }
        }
    }
}

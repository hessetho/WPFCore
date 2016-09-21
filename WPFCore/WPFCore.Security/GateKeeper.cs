using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Security
{
    public static class GateKeeper
    {
        private static IPrincipal currentPrincipal;

        static GateKeeper()
        {
            var id = new GenericIdentity("unauthenticated", "");
            
            currentPrincipal = new GenericPrincipal(id, null);
            
        }

        public static IPrincipal CurrentPrincipal
        {
            get
            {
                return currentPrincipal;
            }
            set
            {
                currentPrincipal = value;
            }
        }
    }
}

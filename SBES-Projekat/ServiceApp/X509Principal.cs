using System.Security.Principal;

namespace ServiceApp
{
    internal class X509Principal: IPrincipal
    {
        private IIdentity identity;

        public X509Principal(IIdentity identity)
        {
            this.identity = identity;
        }

        public IIdentity Identity
        {
            get { return identity; }
        }
        public bool IsInRole(string role)
        {
            return identity.Name.Contains(role);
        }
    }
}
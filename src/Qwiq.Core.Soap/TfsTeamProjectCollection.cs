using System;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Server;

namespace Microsoft.Qwiq.Soap
{
    internal class TfsTeamProjectCollection : IInternalTfsTeamProjectCollection
    {
        private readonly Lazy<ICommonStructureService> _css;

        private readonly Lazy<IIdentityManagementService> _ims;

        private readonly TeamFoundation.Client.TfsTeamProjectCollection _tfs;

        internal TfsTeamProjectCollection(TeamFoundation.Client.TfsTeamProjectCollection tfs)
        {
            _tfs = tfs;

            _css = new Lazy<ICommonStructureService>(
                () => ExceptionHandlingDynamicProxyFactory.Create<ICommonStructureService>(
                    new CommonStructureService(_tfs?.GetService<ICommonStructureService4>())));
            _ims = new Lazy<IIdentityManagementService>(
                () => ExceptionHandlingDynamicProxyFactory.Create<IIdentityManagementService>(
                    new IdentityManagementService(_tfs?.GetService<IIdentityManagementService2>())));
        }

        public TfsCredentials AuthorizedCredentials => new TfsCredentials(_tfs?.ClientCredentials);

        public ITeamFoundationIdentity AuthorizedIdentity => new TeamFoundationIdentity(_tfs?.AuthorizedIdentity);

        public ICommonStructureService CommonStructureService => _css.Value;

        public bool HasAuthenticated => _tfs?.HasAuthenticated ?? false;

        public IIdentityManagementService IdentityManagementService => _ims.Value;

        public Uri Uri => _tfs.Uri;

        public TimeZone TimeZone => _tfs?.TimeZone ?? TimeZone.CurrentTimeZone;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public T GetService<T>()
        {
            return _tfs == null ? default(T) : _tfs.GetService<T>();
        }

        public T GetClient<T>()
        {
            return _tfs.GetClient<T>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) _tfs?.Dispose();
        }
    }
}
using Microsoft.Live;
using System;
using System.Threading.Tasks;
using WF.Player.Extensions;

namespace WF.Player.Providers
{
    public class OneDriveProvider : ProviderBase, ILinkable, ISyncable
    {
        #region Constants

        private static readonly int _LinkTimeout = 10000; // in ms

        private static readonly string[] _Scopes = new string[] { "wl.skydrive_update", "wl.offline_access", "wl.signin" };

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the client id used to connect to Microsoft Live services.
        /// </summary>
        public string ClientId { get; set; }

        #endregion

        #region Fields

        private object _syncRoot = new object();

        private LiveConnectClient _client;

        #endregion

        #region ILinkable

        #region LinkState
        private LinkState _linkState;
        public LinkState LinkState
        {
            get
            {
                lock (_syncRoot)
                {
                    return _linkState;
                }
            }

            private set
            {
                lock (_syncRoot)
                {
                    _linkState = value;
                }
            }
        } 
        #endregion

        #region LinkAsync()
        public async Task<LinkResult> LinkAsync()
        {
            // Succeeds if the ILinkable is already linked.
            if (!CheckLinkState(Providers.LinkState.Online))
            {
                return LinkResult.Online();
            }

            // We're now linking.
            LinkState = Providers.LinkState.Linking;

            // Makes an auth client using the user-defined client id.
            LiveAuthClient authClient = new LiveAuthClient(ClientId);

            // Starts initializing.
            LiveLoginResult result = await authClient.InitializeAsync(_Scopes);

            // Move on with user login or not depending on the result.
            if (result.Status == LiveConnectSessionStatus.Connected)
            {
                // We're connected. Let's make a client.
                MakeClientFromSession(result.Session);

                // We're not linking anymore.
                LinkState = Providers.LinkState.Online;

                // Returns.
                return LinkResult.Online();
            }
            else
            {
                // We're not connected. We need to ask the user for login.

#if WINDOWS_PHONE || WINDOWS_STORE
                throw new System.NotImplementedException();
#else
                // We do not have access to a UX to login the user.
                // Let's get a login URL to get user consent.
                string loginUrl = authClient.GetLoginUrl(_Scopes);

                // We're not linking anymore.
                LinkState = Providers.LinkState.Offline;

                // Returns.
                return LinkResult.Offline(loginUrl, true);
#endif
            }
        }

        /// <summary>
        /// Attempts at asynchronously linking this OneDriveProvider using an authentication code.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <remarks>If this ILinkable is already linked at the time the task is ran,
        /// the task will succeed seamlessly.
        /// This method never triggers UX events in the app.</remarks>
        public async Task<LinkResult> LinkAsync(string authCode)
        {
#if WINDOWS_PHONE || WINDOWS_STORE
            throw new System.NotImplementedException();
#else
            // Succeeds if the ILinkable is already linked.
            if (!CheckLinkState(Providers.LinkState.Online))
            {
                return LinkResult.Online();
            }

            // We're now linking.
            LinkState = Providers.LinkState.Linking;

            // Makes an auth client using the user-defined client id.
            LiveAuthClient authClient = new LiveAuthClient(ClientId);

            // Exchanges auth codes.
            return await authClient
                .ExchangeAuthCodeAsync(authCode)
                .TimeoutAfter(_LinkTimeout)
                .ContinueWith<LinkResult>(t =>
            {
                if (t.IsFaulted)
                {
                    // We're offline because of a timeout.
                    LinkState = Providers.LinkState.Offline;

                    return LinkResult.Offline(true);
                }
                else
                {
                    // The session is ready. Make a client out of it.
                    MakeClientFromSession(t.Result);

                    // We're online.
                    LinkState = Providers.LinkState.Online;

                    // Returns.
                    return LinkResult.Online();
                }
            });
#endif
        }

        private void MakeClientFromSession(LiveConnectSession session)
        {
            _client = new LiveConnectClient(session);
        }

        #endregion

        #region UnlinkAsync()
        public Task<LinkResult> UnlinkAsync()
        {
            return Task.Factory.StartNew<LinkResult>(() =>
            {
                // Checks state.
                if (!CheckLinkState(Providers.LinkState.Offline))
                {
                    return LinkResult.Offline();
                }

                // We're unlinking.
                LinkState = Providers.LinkState.Unlinking;

                // Kills the client.
                _client = null;

                // Prepares the link result.
                LinkResult result = null;

#if WINDOWS_PHONE || WINDOWS_STORE           
            // Nothing more to do.
            result = LinkResult.Offline();
#else
                // We're giving out the Uri for manual logout.
                result = LinkResult.Offline(new LiveAuthClient(ClientId).GetLogoutUrl(), false);
#endif

                // We're offline.
                LinkState = Providers.LinkState.Offline;

                return result;
            });
        } 
        #endregion

        private bool CheckLinkState(LinkState target)
        {
            LinkState ls = this.LinkState;
            if (ls == target)
            {
                // Nothing to do.
                return false;
            }

            // Fails if the ILinkable is linking.
            else if (ls == Providers.LinkState.Linking)
            {
                throw new InvalidOperationException("Another linking task is running.");
            }
            else if (ls == Providers.LinkState.Unlinking)
            {
                throw new InvalidOperationException("Another unlinking task is running.");
            }

            // We can link or unlink.
            return true;
        }

        #endregion

        #region ISyncable

        public event EventHandler<SyncEventArgs> SyncProgress;

        public bool CanSyncUp
        {
            get { return true; }
        }

        public bool CanSyncDown
        {
            get { return true; }
        }

        public Task SyncUp()
        {
            throw new NotImplementedException();
        }

        public Task SyncDown()
        {
            throw new NotImplementedException();
        }

        public Task Sync()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

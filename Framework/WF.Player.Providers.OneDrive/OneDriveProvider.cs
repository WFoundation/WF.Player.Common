using Microsoft.Live;
using System;
using System.Threading.Tasks;
using WF.Player.Extensions;

namespace WF.Player.Providers
{
    public class OneDriveProvider : ProviderBase, ILinkable
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
            if (!CheckStateForLinking())
            {
                return new LinkResult(Providers.LinkState.Online);
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
                return new LinkResult(Providers.LinkState.Online);
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
                return new LinkResult(Providers.LinkState.Offline, loginUrl);
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
            if (!CheckStateForLinking())
            {
                return new LinkResult(Providers.LinkState.Online);
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

                    return new LinkResult(Providers.LinkState.Offline, false, true);
                }
                else
                {
                    // The session is ready. Make a client out of it.
                    MakeClientFromSession(t.Result);

                    // We're online.
                    LinkState = Providers.LinkState.Online;

                    // Returns.
                    return new LinkResult(Providers.LinkState.Online);
                }
            });
#endif
        }

        private bool CheckStateForLinking()
        {
            LinkState ls = this.LinkState;
            if (ls == Providers.LinkState.Online)
            {
                return false;
            }

            // Fails if the ILinkable is linking.
            else if (ls == Providers.LinkState.Linking)
            {
                throw new InvalidOperationException("Another linking task is running.");
            }

            // We can link.
            return true;
        }

        private void MakeClientFromSession(LiveConnectSession session)
        {
            _client = new LiveConnectClient(session);
        }

        #endregion

        public Task<LinkResult> UnlinkAsync()
        {
            throw new System.NotImplementedException();
        } 

        #endregion


    }
}

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleMusicApi.Requests;
using GoogleMusicApi.Structure;

namespace GoogleMusicApi.Common
{
    /// <summary>
    /// An Easy to use Google Play Music Client, that can do everything but upload music.
    ///  
    /// </summary>
    public class MobileClient : Client<MobileSession>
    {
        /// <summary>
        /// Create a new <see cref="MobileClient"/>.
        /// </summary>
        public MobileClient()
        {
            
        }

        /// <summary>
        /// Create a <see cref="MobileClient"/> that has previously been logged in, using the specified Authorization Token.
        /// </summary>
        /// <param name="token">The Previous Authorization Token. </param>
        /// <remarks>
        /// To Check if the Authorization Token is still valid see: <seealso cref="LoginCheck"/> 
        /// </remarks>
        public MobileClient(string token)
        {

        }


        /// <summary>
        /// Login to Google Play Music with the specified email and password.
        /// This will make two requests to the Google Authorization Server, and collect an Authorization Token to use for all requests.
        /// </summary>
        /// <param name="email">The Email / Username of the google account</param>
        /// <param name="password">The Password / App Specific password (https://security.google.com/settings/security/apppasswords)</param>
        public sealed override bool Login(string email, string password)
        {
            Debug.WriteLine($"Attempting Login ({email})...");

            Session = new MobileSession();
            return Session.Login(email, password);
        }

        /// <summary>
        /// Check if the specified Authorization Token is still valid, and if it is collect required information to login.
        /// </summary>
        /// <param name="token">The Authorization Token, if left null will use the Authorization Token associated to this <see cref="MobileClient"/> (If specified in constructor)</param>
        /// <returns></returns>
        public bool LoginCheck(string token = null)
        {

            //TODO (Medium): Implement Token Check
            throw new NotSupportedException();
/*
            if (token == null && AuthorizationToken == null)
                throw new ArgumentException("Please specify an Authorization Token", nameof(token));
            if(token == null) token = AuthorizationToken;

            Debug.WriteLine($"Checking Token ({token})...");

            return false;
*/
        }

        private bool CheckSession()
        {
#if DEBUG
            if (Session == null)
                throw new InvalidOperationException("Session Not Set! Try logging in again.");
            if (Session.AuthorizationToken == null)
                throw new InvalidOperationException(
                    "Session does not contain an Authorization Token! Try logging in again.");
#else
            if (Session?.AuthorizationToken != null) 
                return true;
#endif
            return false;
        }

        private TReuqest MakeRequest<TReuqest>()
            where TReuqest : StructuredRequest, new()
        {
            return new TReuqest();
        }

        /// <summary>
        /// Get the Google Play Music configuration key / values
        /// </summary>
        /// <returns>Your current Google Play Music configuration</returns>
        public Config GetConfig()
        {
            if (!CheckSession())
                return null;
            var request = MakeRequest<ConfigRequest>();
            var data = request.Get(new GetRequest(Session));
            return data;
            
        } 

        /// <summary>
        /// Runs <seealso cref="GetConfig"/> Asynchronously.
        /// </summary>
        /// <returns>The value returned from <seealso cref="GetConfig"/></returns>
        public async Task<Config> GetConfigAsync()
        {
            return await Task.Factory.StartNew(GetConfig);
        }

        /// <summary>
        /// Gets the current Situations from Google Play Music.
        /// </summary>
        /// <param name="situationType">The situation types you wish to receive</param>
        /// <returns>
        /// This contains information like:
        ///  -"Its Friday Morning..." : "Play Music for..."
        ///     - "Todays Biggest Hits"
        ///         - "Today's Dance Smashes"
        ///         - "Today's Pop Charts"
        ///         - "..."
        ///     - "Waking Up Happy"
        ///         - "Star Guitars"
        ///             - "Air Guitar Heroes"
        ///             - "..."
        ///         - "..."
        /// All Data above will also have <see cref="ArtReference"/>'s for each station / situation
        /// </returns>
        //TODO (Low): Find out what situation types exist
        //TODO (Medium): Convert the int[] to a SituationType[]
        public ListListenNowSituationResponse GetListenNowSituations(params int[] situationType)
        {
            if (!CheckSession())
                return null;
            if (situationType == null)
            {
                situationType = new[] {1};
            }
            var requestData = new ListListenNowSituationsRequest(Session)
            {
                RequestSignals = new RequestSignal(RequestSignal.GetTimeZoneOffsetSecs()),
                SituationType = situationType
                
            };

            var request = MakeRequest<ListListenNowSituations>();
            var data = request.Get(requestData);
            return data;
        }
        /// <summary>
        /// Runs <seealso cref="GetListenNowSituations"/> Asynchronously.
        /// </summary>
        /// <param name="situationType">The situation types you wish to receive</param>
        /// <returns>The value returned from <seealso cref="GetListenNowSituations"/></returns>

        public async Task<ListListenNowSituationResponse> GetListenNowSituationsAsync(params int[] situationType)
        {
            return await Task.Factory.StartNew(() => GetListenNowSituations(situationType));
        } 


    }
}
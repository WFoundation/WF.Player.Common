// <copyright file="LinkResult.cs" company="Wherigo Foundation">
//   WF.Player - A Wherigo Player which use the Wherigo Foundation Core.
//   Copyright (C) 2012-2015  Brice Clocher (mangatome@gmail.com)
//   Copyright (C) 2012-2015  Dirk Weltz (mail@wfplayer.com)
// </copyright>

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace WF.Player.Providers
{
    public class LinkResult
    {
        /// <summary>
        /// Gets if further input from the user is required to complete the link operation.
        /// </summary>
        public bool RequiresUserInput { get; private set; }

        /// <summary>
        /// Gets if the link process timed out.
        /// </summary>
        public bool TimedOut { get; private set; }

        /// <summary>
        /// Gets an Uri that can be used to get user consent from the user in order to complete
        /// the link operation.
        /// </summary>
        /// <remarks>This property is to be used alongside with <code>RequiresUserInput == true</code>.</remarks>
        public Uri UserConsentUri { get; private set; }

        /// <summary>
        /// Gets the state of the link after the linking operation finished.
        /// </summary>
        public LinkState State { get; private set; }

        public LinkResult(LinkState finalState)
        {
            State = finalState;
        }

        public LinkResult(LinkState finalState, bool requiresUserInput, bool timedOut)
        {
            State = finalState;
            RequiresUserInput = requiresUserInput;
            TimedOut = timedOut;
        }

        public LinkResult(LinkState finalState, string userConsentUrl)
        {
            State = finalState;
            RequiresUserInput = true;
            UserConsentUri = new Uri(userConsentUrl, UriKind.Absolute);
        }
    }
}

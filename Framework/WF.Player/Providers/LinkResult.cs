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
        /// Gets if web navigation is required to further continue or complete the link operation.
        /// </summary>
        public bool RequiresWebNavigation { get; private set; }

        /// <summary>
        /// Gets if the link process timed out.
        /// </summary>
        public bool TimedOut { get; private set; }

        /// <summary>
        /// Gets an Uri that can be used to continue the link operation on the web.
        /// </summary>
        /// <remarks>This property is to be used alongside with <code>RequiresWebNavigation == true</code>.</remarks>
        public Uri WebNavigationUri { get; private set; }

        /// <summary>
        /// Gets the state of the link after the linking operation finished.
        /// </summary>
        public LinkState State { get; private set; }

        /// <summary>
        /// Constructs a simple result for a link state.
        /// </summary>
        /// <param name="ls"></param>
        public LinkResult(LinkState ls)
        {
            State = ls;
        }

        /// <summary>
        /// Makes a LinkResult representing an online link state.
        /// </summary>
        /// <returns></returns>
        public static LinkResult Online()
        {
            return new LinkResult(LinkState.Online);
        }

        /// <summary>
        /// Makes a LinkResult representing an offline link state.
        /// </summary>
        /// <returns></returns>
        public static LinkResult Offline()
        {
            return new LinkResult(LinkState.Offline);
        }

        /// <summary>
        /// Makes a LinkResult representing an offline link state that requires the client app
        /// to navigate to an external web Url that will ask for user input or not.
        /// </summary>
        /// <param name="webNavigationUrl"></param>
        /// <param name="requiresUserInput"></param>
        /// <returns></returns>
        public static LinkResult Offline(string webNavigationUrl, bool requiresUserInput)
        {
            return new LinkResult(LinkState.Offline)
            {
                RequiresWebNavigation = true,
                WebNavigationUri = new Uri(webNavigationUrl)
            };
        }

        /// <summary>
        /// Makes a LinkResult representing an offline state that timed out or not.
        /// </summary>
        /// <param name="timedOut"></param>
        /// <returns></returns>
        public static LinkResult Offline(bool timedOut)
        {
            return new LinkResult(LinkState.Offline)
            {
                TimedOut = timedOut
            };
        }
    }
}

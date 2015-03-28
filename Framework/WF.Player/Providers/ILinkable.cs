// <copyright file="ILinkable.cs" company="Wherigo Foundation">
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
      
using System.Threading.Tasks;

namespace WF.Player.Providers
{
    /// <summary>
    /// A provider that can be linked to a remote source.
    /// </summary>
    public interface ILinkable
    {
        /// <summary>
        /// Gets the link state of this ILinkable, indicating if it is linked to its
        /// associated remote source or not.
        /// </summary>
        LinkState LinkState { get; }

        /// <summary>
        /// Attempts at asynchronously linking this ILinkable to its associated source.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <remarks>If this ILinkable is already linked at the time the task is ran,
        /// the task will succeed seamlessly.
        /// This method is allowed to trigger UX events in the app (for instance, starting
        /// a login UX for the user to give permissions.)</remarks>
        Task<LinkResult> LinkAsync();

        /// <summary>
        /// Attempts at asynchronously unlinking this ILinkable from its associated source.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <remarks>If this ILinkable is already unlinked at the time the task is ran,
        /// the task will succeed seamlessly.</remarks>
        Task<LinkResult> UnlinkAsync();
    }
}

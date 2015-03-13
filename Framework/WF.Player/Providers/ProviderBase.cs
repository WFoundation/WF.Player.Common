// <copyright file="ProviderBase.cs" company="Wherigo Foundation">
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
      

namespace WF.Player.Providers
{
    /// <summary>
    /// A source of CartridgeTags that can cache them on the local file system.
    /// </summary>
    public abstract class ProviderBase : IProvider
    {
        /// <summary>
        /// Gets the path to the folder (in the local file system) where this provider
        /// stores cartridge tags.
        /// </summary>
        public string LocalPathToCache { get; protected set; }
    }
}

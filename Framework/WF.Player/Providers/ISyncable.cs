﻿// <copyright file="ISyncable.cs" company="Wherigo Foundation">
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
using System.Threading.Tasks;

namespace WF.Player.Providers
{
    /// <summary>
    /// A provider that can sync itself with a remote source.
    /// </summary>
    public interface ISyncable
    {
        event EventHandler<SyncEventArgs> SyncProgress;
        
        bool CanSyncUp { get; }

        bool CanSyncDown { get; }
        
        Task SyncUp();

        Task SyncDown();

        Task Sync();
    }
}

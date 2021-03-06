﻿/* 
    Jolt Environment
    Copyright (C) 2010 Jolt Environment Team

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RuneScape.Content.ClanChat;
using RuneScape.Content.Interfaces;
using RuneScape.Utilities;

namespace RuneScape.Content
{
    /// <summary>
    /// Defines a game contents.
    /// </summary>
    public class GameContent
    {
        #region Properties
        /// <summary>
        /// The interface manager maintaining all interfaces.
        /// </summary>
        public InterfaceManager InterfaceManager { get; private set; }
        /// <summary>
        /// The clanchat manager maintaining all clanchat content.
        /// </summary>
        public ClanChatManager ClanChat { get; private set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructs a new game content management.
        /// </summary>
        public GameContent()
        {
            this.InterfaceManager = new InterfaceManager();
            this.ClanChat = new ClanChatManager();
        }
        #endregion Constructors

        #region Methods
        #endregion Methods
    }
}

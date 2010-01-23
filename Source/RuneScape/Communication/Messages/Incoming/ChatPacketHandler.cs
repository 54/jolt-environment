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

using RuneScape.Model;
using RuneScape.Model.Characters;
using RuneScape.Utilities;

namespace RuneScape.Communication.Messages.Incoming
{
    /// <summary>
    /// Handler for the chat packet.
    /// </summary>
    public class ChatPacketHandler : IPacketHandler
    {
        #region Methods
        /// <summary>
        /// Handles the chat information sent from the client.
        /// </summary>
        /// <param name="character">The character to handle packet for.</param>
        /// <param name="packet">The packet containing handle data.</param>
        public void Handle(Character character, Packet packet)
        {
            int effects = packet.ReadShort() & 0xFFFF;
            int length = packet.ReadByte();
            string text = ChatUtilities.DecryptChat(packet, length);

            character.CurrentChatMessage = ChatMessage.Create(effects, length, text);
            character.UpdateFlags.ChatUpdateRequired = true;
        }
        #endregion Methods
    }
}

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

using RuneScape.Communication.Messages.Outgoing;
using RuneScape.Model.Characters;
using RuneScape.Model.Npcs;

namespace RuneScape.Communication.Messages.Incoming
{
    /// <summary>
    /// Handler for npc examine packet.
    /// </summary>
    public class NpcExaminePacketHandler : IPacketHandler
    {

        #region Methods
        /// <summary>
        /// Handles the npc examine packet.
        /// </summary>
        /// <param name="character">The character to handle packet for.</param>
        /// <param name="packet">The packet containing handle data.</param>
        public void Handle(Character character, Packet packet)
        {
            short id = packet.ReadShort();
            NpcDefinition def = GameEngine.World.NpcManager.GetDefinition(id);

            if (def != null)
            {
                character.Session.SendData(new MessagePacketComposer(def.Examine).Serialize());
            }
            else
            {
                character.Session.SendData(new MessagePacketComposer("It's a NPC.").Serialize());
            }
        }
        #endregion Methods
    }
}

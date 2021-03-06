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

using RuneScape.Model.Characters;

namespace RuneScape.Communication.Messages.Outgoing
{
    /// <summary>
    /// Composes a base configuration.
    /// </summary>
    public class ConfigPacketComposer : PacketComposer
    {
        #region Constructors
        /// <summary>
        /// Constructs a new base configuration packet.
        /// </summary>
        /// <param name="id">The id of the configuration.</param>
        /// <param name="value">The value of the configuratoin.</param>
        public ConfigPacketComposer(short id, int value)
        {
            if (value > -129 && value < 128)
            {
                SetOpcode(100);
                AppendShortA(id);
                AppendByteA((byte)value);
            }
            else
            {
                SetOpcode(161);
                AppendShort(id);
                AppendIntSecondary(value);
            }
        }
        #endregion Constructors
    }
}

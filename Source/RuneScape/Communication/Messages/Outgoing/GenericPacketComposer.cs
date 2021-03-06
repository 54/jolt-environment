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

namespace RuneScape.Communication.Messages.Outgoing
{
    /// <summary>
    /// Represents a general use packet composer.
    /// </summary>
    public class GenericPacketComposer : PacketComposer
    {
        #region Constructors
        /// <summary>
        /// Constructs a new general use composer with a default capacity base.
        /// </summary>
        public GenericPacketComposer() : base() { }

        /// <summary>
        /// Constructs a new general use composer with a specified capacity base.
        /// </summary>
        public GenericPacketComposer(int capacity) : base(capacity) { }
        #endregion Constructors
    }
}

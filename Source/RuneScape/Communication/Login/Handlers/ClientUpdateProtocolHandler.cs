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

namespace RuneScape.Communication.Login.Handlers
{
    /// <summary>
    /// Handles the client update protocol.
    /// 
    ///     <para>The runescape client requires a set of update
    ///     keys which are sent to verify revision authentity.</para>
    /// </summary>
    public class ClientUpdateProtocolHandler : IProtocolHandler
    {
        #region Fields
        /// <summary>
        /// The protocol id of this handle.
        /// </summary>
        public const int ProtocolId = 2;

        /// <summary>
        /// The update keys needed to update the 508 version client.
        /// </summary>
        private static byte[] updateKeys = 
        { 
            0xff, 0x00, 0xff, 0x00, 0x00, 0x00, 0x00, 0xd8, 
		    0x84, 0xa1, 0xa1, 0x2b, 0x00, 0x00, 0x00, 0xba, 
		    0x58, 0x64, 0xe8, 0x14, 0x00, 0x00, 0x00, 0x7b, 
		    0xcc, 0xa0, 0x7e, 0x23, 0x00, 0x00, 0x00, 0x48, 
		    0x20, 0x0e, 0xe3, 0x6e, 0x00, 0x00, 0x01, 0x88, 
		    0xec, 0x0d, 0x58, 0xed, 0x00, 0x00, 0x00, 0x71, 
		    0xb9, 0x4c, 0xc0, 0x50, 0x00, 0x00, 0x01, 0x8b, 
		    0x5b, 0x61, 0x79, 0x20, 0x00, 0x00, 0x00, 0x0c, 
		    0x0c, 0x69, 0xb1, 0xc8, 0x00, 0x00, 0x02, 0x31, 
		    0xc8, 0x56, 0x67, 0x52, 0x00, 0x00, 0x00, 0x69, 
		    0x78, 0x17, 0x7b, 0xe2, 0x00, 0x00, 0x00, 0xc3, 
		    0x29, 0x76, 0x27, 0x6a, 0x00, 0x00, 0x00, 0x05, 
		    0x44, 0xe7, 0x75, 0xcb, 0x00, 0x00, 0x00, 0x08, 
		    0x7d, 0x21, 0x80, 0xd5, 0x00, 0x00, 0x01, 0x58, 
		    0xeb, 0x7d, 0x49, 0x8e, 0x00, 0x00, 0x00, 0x0c, 
		    0xf4, 0xdf, 0xd6, 0x4d, 0x00, 0x00, 0x00, 0x18, 
		    0xec, 0x33, 0x31, 0x7e, 0x00, 0x00, 0x00, 0x01, 
		    0xf7, 0x7a, 0x09, 0xe3, 0x00, 0x00, 0x00, 0xd7, 
		    0xe6, 0xa7, 0xa5, 0x18, 0x00, 0x00, 0x00, 0x45, 
		    0xb5, 0x0a, 0xe0, 0x64, 0x00, 0x00, 0x00, 0x75, 
	    	0xba, 0xf2, 0xa2, 0xb9, 0x00, 0x00, 0x00, 0x5f, 
	    	0x31, 0xff, 0xfd, 0x16, 0x00, 0x00, 0x01, 0x48, 
	    	0x03, 0xf5, 0x55, 0xab, 0x00, 0x00, 0x00, 0x1e, 
	    	0x85, 0x03, 0x5e, 0xa7, 0x00, 0x00, 0x00, 0x23, 
	    	0x4e, 0x81, 0xae, 0x7d, 0x00, 0x00, 0x00, 0x18, 
	    	0x67, 0x07, 0x33, 0xe3, 0x00, 0x00, 0x00, 0x14, 
	    	0xab, 0x81, 0x05, 0xac, 0x00, 0x00, 0x00, 0x03, 
	    	0x24, 0x75, 0x85, 0x14, 0x00, 0x00, 0x00, 0x36
        };
        #endregion Fields

        #region Methods
        /// <summary>
        /// Handles the protocol request.
        /// </summary>
        /// <param name="request">The instance requesting the protcol handle.</param>
        public void Handle(LoginRequest request)
        {
            /* 
             * Note: I don't know why we're requiring 8 bytes, 
             * but im thinking that it indicates that the client 
             * is ready (as 4 bytes are sent twice, not at onces).
             */
            if (request.Buffer.RemainingAmount >= 8)
            {
                request.Connection.SendData(updateKeys);
                request.Remove = true;
            }
            else
            {
                byte[] data = new byte[4];
                request.Connection.Socket.Receive(data);

                if (request.Buffer.RemainingAmount >= 8)
                {
                    request.Connection.SendData(updateKeys);
                    request.Remove = true;
                }
            }
        }
        #endregion Methods
    }
}

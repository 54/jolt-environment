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

using RuneScape.Communication.Messages;
using RuneScape.Communication.Messages.Outgoing;

namespace RuneScape.Model.Characters
{
    /// <summary>
    /// Provides updates to render the character properties on the client.
    /// </summary>
    public partial class RenderUpdating
    {
        #region Methods
        /// <summary>
        /// Updates the specified character to show recently changed properties.
        /// </summary>
        /// <param name="character">The character to update.</param>
        public static void Update(Character character, List<Character> allChars)
        {
            if (character.UpdateFlags.MapRegionChanging)
            {
                Frames.SendMapRegion(character);
            }

            // The main packet. Data is written in bits.
            GenericPacketComposer mainPacket = new GenericPacketComposer();
            mainPacket.SetOpcode(216);
            mainPacket.SetType(PacketType.Short);
            mainPacket.InitializeBit();

            // The update block. Any updates that are pending will be added to this block.
            GenericPacketComposer updateBlock = new GenericPacketComposer();

            // Update this character's movement.
            UpdateThisMovement(character, mainPacket);

            // Update the character's masks if required.
            if (character.UpdateFlags.UpdateRequired)
            {
                /*
                 * No need to force appeal since appearance hasn't changed 
                 * (unless appearance update is requied in update flags).
                 */ 
                AppendUpdateMasks(character, updateBlock, false);
            }
            mainPacket.AppendBits(8, character.LocalCharacters.Count);

            // Loop though the character's current local characters list.
            character.LocalCharacters.ForEach((otherCharacter) =>
            {
                // We can only update this character is he meets local standards.
                if (GameEngine.World.CharacterManager.Contains(otherCharacter)
                    && !otherCharacter.UpdateFlags.Teleporting
                    && otherCharacter.Location.WithinDistance(character.Location))
                {
                    //Console.WriteLine(character.Username + " Updated.");
                    // Update the character's movement so the main character can see the new movement.
                    UpdateMovement(otherCharacter, mainPacket);

                    // Update the character's movement so the main character can see the new updates (only if there is any).
                    if (otherCharacter.UpdateFlags.UpdateRequired)
                    {
                        //Console.WriteLine(character.Username + " Update Required.");
                        AppendUpdateMasks(otherCharacter, updateBlock, false);
                    }
                }
                else
                {
                    //Console.WriteLine(character.Username + " Left.");

                    // Remove this character, as it doesn't meet local standards.
                    character.LocalCharacters.Remove(otherCharacter);
                    
                    // Signify the client that this character needs to be removed.
                    mainPacket.AppendBits(1, 1);
                    mainPacket.AppendBits(2, 3);
                    
                }
            });

            foreach (Character otherCharacter in allChars)
            {
                /*
                 * If there is no more space in the local area for the main character to 
                 * see, then we will not show those other characters untill some leave.
                 */ 
                if (character.LocalCharacters.Count > 255)
                {
                    break;
                }

                /*
                 * Character does not meet local standards, cannot be added.
                 */ 
                if (otherCharacter == character 
                    || character.LocalCharacters.Contains(otherCharacter) 
                    || !otherCharacter.Location.WithinDistance(character.Location))
                {
                    continue;
                }

                //Console.WriteLine(character.Username + " Added.");
                character.LocalCharacters.Add(otherCharacter);
                AddNewCharacter(character, otherCharacter, mainPacket);
                AppendUpdateMasks(otherCharacter, updateBlock, true);
            }

            /*
             * If there are masks that need updating, we will append a
             * special key identifying the start of the update masks, 
             * and append the serialized update block.
             */
            if (!updateBlock.Empty)
            {
                mainPacket.AppendBits(11, 2047);

                // Finish bit access so we can write bytes normally.
                mainPacket.FinishBit();

                mainPacket.AppendBytes(updateBlock.SerializeBuffer());
            }
            else
            {
                // Nothing else to write.
                mainPacket.FinishBit();
            }

            // Write the main packet to character's client.
            character.Session.SendData(mainPacket.Serialize());
        }

        /// <summary>
        /// Adds a new character to the current character's client.
        /// </summary>
        /// <param name="character">The current character.</param>
        /// <param name="other">The character to add.</param>
        /// <param name="mainPacket">The packet to write the data to.</param>
        private static void AddNewCharacter(Character character, Character other, GenericPacketComposer mainPacket)
        {
            mainPacket.AppendBits(11, other.Index);
            int xPos = other.Location.X - character.Location.X;
            int yPos = other.Location.Y - character.Location.Y;

            if (xPos < 0)
            {
                xPos += 32;
            }
            if (yPos < 0)
            {
                yPos += 32;
            }

            mainPacket.AppendBits(5, xPos);
            mainPacket.AppendBits(1, 1);
            mainPacket.AppendBits(3, 1);
            mainPacket.AppendBits(1, 1);
            mainPacket.AppendBits(5, yPos);
        }

        /// <summary>
        /// Updates the "other" character (The character from local players list).
        /// </summary>
        /// <param name="character">The "other" character to update.</param>
        /// <param name="mainPacket">The main packet for appending required data.</param>
        private static void UpdateMovement(Character character, GenericPacketComposer mainPacket)
        {
            if (character.Sprites.PrimarySprite == -1)
            {
                if (character.UpdateFlags.UpdateRequired)
                {
                    mainPacket.AppendBits(1, 1);
                    mainPacket.AppendBits(2, 0);
                }
                else
                {
                    mainPacket.AppendBits(1, 0);
                }
            }
            else if (character.Sprites.SecondarySprite == -1)
            {
                mainPacket.AppendBits(1, 1);
                mainPacket.AppendBits(2, 1);
                mainPacket.AppendBits(3, character.Sprites.PrimarySprite);
                mainPacket.AppendBits(1, character.UpdateFlags.UpdateRequired ? 1 : 0);
            }
            else
            {
                mainPacket.AppendBits(1, 1);
                mainPacket.AppendBits(2, 2);
                mainPacket.AppendBits(3, character.Sprites.PrimarySprite);
                mainPacket.AppendBits(3, character.Sprites.SecondarySprite);
                mainPacket.AppendBits(1, character.UpdateFlags.UpdateRequired ? 1 : 0);
            }
        }

        /// <summary>
        /// Updates "this" character (The character based upon for the local players list).
        /// </summary>
        /// <param name="character">The character to update.</param>
        /// <param name="mainPacket">The main packet for appending required data.</param>
        private static void UpdateThisMovement(Character character, GenericPacketComposer mainPacket)
        {
            if (character.UpdateFlags.Teleporting)
            {
                mainPacket.AppendBits(1, 1);
                mainPacket.AppendBits(2, 3);
                mainPacket.AppendBits(7, character.Location.GetLocalX(character.UpdateFlags.LastRegion));
                mainPacket.AppendBits(1, 1);
                mainPacket.AppendBits(2, character.Location.Z);
                mainPacket.AppendBits(1, character.UpdateFlags.UpdateRequired ? 1 : 0);
                mainPacket.AppendBits(7, character.Location.GetLocalY(character.UpdateFlags.LastRegion));
            }
            else
            {
                if (character.Sprites.PrimarySprite == -1)
                {
                    mainPacket.AppendBits(1, character.UpdateFlags.UpdateRequired ? 1 : 0);
                    if (character.UpdateFlags.UpdateRequired)
                    {
                        mainPacket.AppendBits(2, 0);
                    }
                }
                else
                {
                    if (character.Sprites.SecondarySprite != -1)
                    {
                        mainPacket.AppendBits(1, 1);
                        mainPacket.AppendBits(2, 2);
                        mainPacket.AppendBits(3, character.Sprites.PrimarySprite);
                        mainPacket.AppendBits(3, character.Sprites.SecondarySprite);
                        mainPacket.AppendBits(1, character.UpdateFlags.UpdateRequired ? 1 : 0);
                    }
                    else
                    {
                        mainPacket.AppendBits(1, 1);
                        mainPacket.AppendBits(2, 1);
                        mainPacket.AppendBits(3, character.Sprites.PrimarySprite);
                        mainPacket.AppendBits(1, character.UpdateFlags.UpdateRequired ? 1 : 0);
                    }
                }
            }
        }
        #endregion Methods
    }
}
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
using RuneScape.Model.Characters;

namespace RuneScape.Model.Items.Containers
{
    /// <summary>
    /// A container for character inventories.
    /// </summary>
    public class EquipmentContainer : Container
    {
        #region Fields
        /// <summary>
        /// The size of the inventory container..
        /// 
        ///     <remarks>This value cannot be changed, because 
        ///     this is the maximum amount required for the 
        ///     runescape client.</remarks>
        /// </summary>
        public const int Size = 14;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets whether the currently equipted weapon has a special attack.
        /// </summary>
        public bool SpecialWeapon { get; private set; }

        /// <summary>
        /// The stand animation according to the equipted weapon.
        /// </summary>
        public short StandAnimation { get; private set; }
        /// <summary>
        /// The walk animation according to the equipted weapon.
        /// </summary>
        public short WalkAnimation { get; private set; }
        /// <summary>
        /// The run animation according to the equipted weapon.
        /// </summary>
        public short RunAnimation { get; private set; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructs a new equipment container.
        /// </summary>
        public EquipmentContainer(Character character)
            : base(EquipmentContainer.Size, ContainerType.Standard, character)
        {
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Refreshes the character's equipment appearance.
        /// </summary>
        public override void Refresh()
        {
            this.character.UpdateFlags.AppearanceUpdateRequired = true;
            this.character.Session.SendData(new InterfaceItemsPacketComposer(387, 28, 94, this).Serialize());
            // TODO: bonuses.
        }

        /// <summary>
        /// Updates the attack style tab interface according to the 
        /// </summary>
        public void UpdateAttackStyle()
        {
            // The character is unarmed (no weapons equipted).
            if (this[3] == null)
            {
                Frames.SendTab(this.character, 73, 92);
                this.character.Session.SendData(new StringPacketComposer("Unarmed", 92, 0).Serialize());
                this.SpecialWeapon = false;
                return;
            }

            short itemId = this[3].Id;
            string weaponName = this[3].Name;

            if (EquipmentItems.WeaponInterface.ContainsKey(itemId))
            {
                short childId = EquipmentItems.WeaponInterface[itemId];
                Frames.SendTab(character, (short)(character.Preferences.Hd ? 87 : 73), childId);
                character.Session.SendData(new StringPacketComposer(weaponName, childId, 0).Serialize());
            }
            else
            {
                Frames.SendTab(character, (short)(character.Preferences.Hd ? 87 : 73), 82);
                character.Session.SendData(new StringPacketComposer(weaponName, 82, 0).Serialize());
            }
        }
        #endregion Methods
    }
}
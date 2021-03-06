﻿/* 
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
using System.Security.Permissions;

using RuneScape.Communication.Messages;

namespace RuneScape
{
    /// <summary>
    /// A command line interpreter allowing us to execute commands 
    /// sumultaneously while the application is running.
    /// </summary>
    public static class CommandLine
    {
        #region Methods
        /// <summary>
        /// Handles a given command.
        /// </summary>
        /// <param name="command">Command to handle.</param>
        /// <param name="arguments">Arguments to handle.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void Handle(string command, string[] arguments)
        {
            try
            {
                switch (command.ToLower())
                {
                    case "count":
                        Console.WriteLine(GameEngine.LoginWorker.service.Count);
                        break;
                    case "running":
                        Console.WriteLine(GameEngine.LoginWorker.Running);
                        Console.WriteLine(GameEngine.LoginWorker.State);
                        break;
                    case "about": // Displays information about jolt environment.
                        Console.WriteLine(" Jolt Environment [" + Program.Version + " x" + (Environment.Is64BitProcess ? "64" : "32") + "-bit]");
                        Console.WriteLine(" Copyright (C) 2010 Jolt Environment Team");
                        Console.WriteLine(" http://www.ajravindiran.com/projects/jolt/");
                        break;
                    case "system": // Displays information about the system.
                        Console.WriteLine("Operating system: " + Environment.OSVersion);
                        Console.WriteLine("Processors/Cores: " + Environment.ProcessorCount);
                        Console.WriteLine("Logged in username: " + Environment.MachineName);
                        Console.WriteLine("You are running the " + (Environment.Is64BitProcess ? "x64" : "x86") + "-bit jolt version.");
                        break;
                    case "shutdown":
                        GameServer.IsRunning = false;
                        break;
                    case "restart":
                        GameServer.Terminate(true);
                        break;
                    case "update":
                        System.Diagnostics.Process.Start("http://jolte.codeplex.com/Release/ProjectReleases.aspx");
                        Program.Logger.WriteInfo("Successfully brang up downloads page.");
                        break;
                    case "systemupdate":
                        short time = short.Parse(arguments[0]);
                        bool restart = false;

                        if (arguments.Length == 2)
                        {
                            restart = bool.Parse(arguments[1]);
                        }
                        Frames.SendSystemUpdate(time, restart);
                        Program.Logger.WriteInfo("System update countdown started... (" + ((time * 600) / 1000) + " seconds till update)");
                        break;
                    default:
                        Program.Logger.WriteWarn("Unknown command \"" + command + "\".");
                        break;
                }
            }
            catch (Exception ex)
            {
                Program.Logger.WriteException(ex);
            }
        }
        #endregion Methods
    }
}

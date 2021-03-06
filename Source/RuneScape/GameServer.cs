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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Permissions;
using System.Threading;

using JoltEnvironment;
using JoltEnvironment.Debug;
using JoltEnvironment.Storage.Sql;
using RuneScape.Network;
using RuneScape.Scripting;
using RuneScape.Utilities;

namespace RuneScape
{
    /// <summary>
    /// Represents an emulator of the MMORPG game, RuneScape.
    /// 
    ///     <para>Handles the game engine, and all the managers.</para>
    /// </summary>
    public static class GameServer
    {
        #region Fields
        /// <summary>
        /// A manager which manages all database related executions.
        /// </summary>
        private static SqlDatabaseManager databaseManager;
        /// <summary>
        /// A manager which manages all tcp related connections.
        /// </summary>
        private static ConnectionManager connectionManager;
        /// <summary>
        /// A manager which manages all remote connections.
        /// </summary>
        private static RemoteManager remoteManager;
        /// <summary>
        /// A manager which manages all scripts.
        /// </summary>
        private static ScriptManager scriptManager;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets whether the server is running or not.
        /// </summary>
        public static bool IsRunning { get; set; }

        /// <summary>
        /// Gets the database manager.
        /// </summary>
        public static SqlDatabaseManager Database
        {
            get { return databaseManager; }
        }

        /// <summary>
        /// Gets the tcp connection manager.
        /// </summary>
        public static ConnectionManager TcpConnection
        {
            get { return connectionManager; }
        }

        /// <summary>
        /// Gets the remote connection manager.
        /// </summary>
        public static RemoteManager RemoteConnection
        {
            get { return remoteManager; }
        }

        /// <summary>
        /// Gets the scripting manager.
        /// </summary>
        public static ScriptManager Scripting
        {
            get { return scriptManager; }
        }

        /// <summary>
        /// Gets or sets the initial server configurations.
        /// </summary>
        public static Configuration Configuration
        {
            get;
            private set;
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Initialize the gaming server environment.
        /// </summary>
        public static void Initialize()
        {
            Program.Logger.WriteInfo("Initializing a new gaming environment...");
            try
            {
                try
                {
                    // Load "general" settings from runescape.ini.
                    Configuration = Configuration.Load(@"..\data\runescape.ini");
                }
                catch (Exception) // Most likely error in config file or no file found.
                {
                    throw;
                }

                // Initialize the database server.
                SqlDatabaseServer databaseServer = new SqlDatabaseServer(
                    Configuration["Master.Database.Host"],
                    Configuration["Master.Database.Port"],
                    Configuration["Master.Database.User"],
                    Configuration["Master.Database.Pass"]);

                // Initialize a database from the database server.
                SqlDatabase database = new SqlDatabase(
                    Configuration["Master.Database.Name"],
                    Configuration["Master.Database.MinPoolSize"],
                    Configuration["Master.Database.MaxPoolSize"]);

                //  Initialize the database manager and test connection.
                databaseManager = new SqlDatabaseManager(database, databaseServer);
                databaseManager.SetClientAmount(10);
                databaseManager.ReleaseClient(databaseManager.GetClient().Handle);
                databaseManager.StartMonitor();

                // Initialize the tcp connection manager and start listening.
                connectionManager = new ConnectionManager(
                    Configuration["TcpConnection.LocalIP"],
                    Configuration["TcpConnection.Port"],
                    Configuration["TcpConnection.MaxConnections"]);

                // Start the connection manager's core listener with user-specified logging/checking.
                connectionManager.Listener.Start(Configuration["TcpConnection.CheckBlacklist"]);

                // Initialize the remote connection manager and start listening.
                remoteManager = new RemoteManager(
                    Configuration["RemoteConnection.LocalIP"], 
                    Configuration["RemoteConnection.Port"]);
                remoteManager.Listener.Start(false);

                // Initialize the scripting manager.
                scriptManager = new ScriptManager();
                scriptManager.Initialize();

                // Check to make sure database verion is valid.
                if (!(bool)Database.Execute(new DatabaseVersionCheck()))
                {
                    throw new Exception("The jolt database you are using, is outdated.");
                }

                /*
                 *  Engine is now able to run cause the connections have been initialized 
                 *  with no errors (yet!), so we will initialize the game engine.
                 */
                IsRunning = true;

                // Initilize the game engine.
                GameEngine.Initialize();

                Program.Logger.WriteInfo("Initialized a new runescape gaming environment.");
            }
            catch (Exception ex)
            {
                Program.Logger.WriteException(ex);
                Program.Logger.WriteError("Could not set up server correctly."
                    + "\nPlease referr to the error message above. Shutting down...");
                Thread.Sleep(5000);
                Environment.Exit(0);
                Terminate(false);
            }
        }

        /// <summary>
        /// Terminates the environment.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void Terminate(bool restart)
        {
            Program.Logger.WriteInfo("Shutting down game server...");
            Program.Logger.LogPriority = LogPriority.Warn;
            IsRunning = false; // Make sure we don't throw the engine off.
            GameEngine.Terminate(); // Terminate the engine, world, and workers.

            if (TcpConnection != null) // Terminate the tcp management and safely release all connections.
            {
                TcpConnection.Listener.Stop();
                TcpConnection.Destroy();
            }

            Console.WriteLine("Please wait 10 seconds for I/O completions.");
            Thread.Sleep(10000);

            if (restart)
            {
                Process.Start("runescape.exe");
            }
            Environment.Exit(0);
        }
        #endregion Methods
    }
}

<?php
/* 
 *     jWeb
 *     Copyright (C) 2010 Jolt Environment Team
 * 
 *     This program is free software: you can redistribute it and/or modify
 *     it under the terms of the GNU General Public License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 * 
 *     This program is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU General Public License for more details.
 * 
 *     You should have received a copy of the GNU General Public License
 *     along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

/**
 * Executes the given query.
 * @global database $database The database giving us connect to the database.
 * @param string $query The query to execute.
 * @return returns false if not executed, resource is value returned.
 */
function dbquery($query) {
    global $database;
    return $database->execute_query($query);
}

/**
 * Evaluates the given query.
 * @global database $database The database giving us connect to the database.
 * @param string $query The query to execute.
 * @param int $default_value
 * @return returns false if not executed, resource is value returned.
 */
function dbevaluate($query, $default_value = 0) {
    global $database;
    return $database->evaluate_query($query, $default_value);
}

?>

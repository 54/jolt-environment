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
define('ACP_TITLE', 'Admin Notes');
define('ACP_TAB', 1);
require_once("adminglobal.php");
check_rights();

$show = 50;

if (isset($_GET['page'])) {
    $page = $_GET['page'];
} else {
    $page = 1;
}

if (isset($_GET['viewall'])) {
    $total_logs = dbevaluate("SELECT COUNT(id) FROM web_acp_notes;");
    $total_pages = ceil($total_logs / $show);
    $tmp_key = $_POST['key'];
    $sp = "<span>";

    for ($i = 1; $i <= $total_pages; $i++) {
        if ($i == $page) {
            $sp .= "<strong>$page</strong><span class='page-sep'>,</span>";
        } else {
            $sp .= "<a href='adminnotes.php?viewall&page=$i'>$i</a>";
        }
    }
    if ($page > $total_pages) {
        $page = 1;
    }
    $next_page = $page + 1;
    if ($next_page > $total_pages) {
        $next_page = $total_pages;
    }
    $sp .= "&nbsp;&nbsp;<a href='adminnotes.php?viewall&page=" . $next_page . "'>Next</a></span>";
} else if (isset($_GET['key'])) {
    $key = $_GET['key'];
    $key = str_replace(' ', '_', strtolower(trim($key)));

    if ($key == "") {
        header("Location: adminlogs.php?viewall");
    }
    
    $total_logs = dbevaluate("SELECT COUNT(id) FROM web_acp_notes WHERE user = '$key';");
    $total_pages = ceil($total_logs / $show);
    $tmp_key = $_POST['key'];
    $sp = "<span>";

    for ($i = 1; $i <= $total_pages; $i++) {
        if ($i == $page) {
            $sp .= "<strong>$page</strong><span class='page-sep'>,</span>";
        } else {
            $sp .= "<a href='adminnotes.php?key=$key&page=$i'>$i</a>";
        }
    }

    $next_page = $page + 1;
    if ($next_page > $total_pages) {
        $next_page = $total_pages;
    }
    $sp .= "&nbsp;&nbsp;<a href='adminnotes.php?key=$key&page=" . $next_page . "'>Next</a></span>";
}

require_once("header.php");
?>
<h1>Admin Logs</h1><hr>
<p>Staff activity is always logged in the AdminCP. You can see all logged activity here.</p><br />

<?php if (isset($_GET['viewall'])) { ?>

<form method="get">
    <fieldset class="display-options" style="float: left">
	Search by name:
        <input type="text" name="key" value="" />&nbsp;
        <input type="submit" class="button2" value="Search" />
    </fieldset>
</form>

<div class="pagination" style="float: right; margin: 15px 0 2px 0">
        <?php echo "Page " . $page . " of " . $total_pages; ?>
    &bull;
        <?php echo $sp; ?>
</div>

<table cellspacing="1">
    <thead>
        <tr>
            <th>Username</th>
            <th>Date</th>
            <th>Message</th>
        </tr>
    </thead>
    <tbody>
            <?php
            $start_from = ($page - 1) * $show;
            $logs = dbquery("SELECT * FROM web_acp_notes ORDER BY id DESC LIMIT $start_from, $show");

            if (mysql_num_rows($logs) > 0) {
                while ($log = mysql_fetch_assoc($logs)) {
                    echo "<tr>
                <td><a href='dashboard.php?delete_note=" . $note['id'] . "'><img src='./images/icon_delete.gif' /></a> " . $users->format_name($log['user']) . "</td>
                <td>" . $log['note_date'] . "</td>
                <td>" . filter_for_outout($log['note_message'], true) . "</td>
                </tr>";
                }
            } else {
                echo "<tr><td colspan='5' style='text-align: center;'>No logs found.</td></tr>";
            }
            ?>
    <form method="get" action="dashboard.php">
        <td colspan='5' style='text-align: center;'>
            <input id="note" name="add_note" size="50"/>
            <input class="button1" type="submit" id="submit" value="Post Note" />
        </td>
    </form>
</tbody>
</table>
    <?php } else if (isset($_GET['key'])) { ?>
<form method="get">
    <fieldset class="display-options" style="float: left">
	Search by name:
        <input type="text" name="key" value="<?php echo $key ?>" />&nbsp;
        <input type="submit" class="button2" value="Search" />
    </fieldset>
</form>

<div class="pagination" style="float: right; margin: 15px 0 2px 0">
        <?php echo "Page " . $page . " of " . $total_pages; ?>
    &bull;
        <?php echo $sp; ?>
</div>

<table cellspacing="1">
    <thead>
        <tr>
            <th>Username</th>
            <th>User IP</th>
            <th>Time</th>
            <th>Action</th>
        </tr>
    </thead>
    <tbody>
            <?php
            $start_from = ($page - 1) * $show;
            $logs = dbquery("SELECT * FROM web_acp_notes WHERE user = '$key' ORDER BY id DESC LIMIT $start_from, $show");

            if (mysql_num_rows($logs) > 0) {
                while ($log = mysql_fetch_assoc($logs)) {
                    echo "<tr>
                <td><a href='dashboard.php?delete_note=" . $note['id'] . "'><img src='./images/icon_delete.gif' /></a> " . $users->format_name($log['user']) . "</td>
                <td>" . $log['note_date'] . "</td>
                <td>" . filter_for_outout($log['note_message'], true) . "</td>
                </tr>";
                }
            } else {
                echo "<tr><td colspan='5' style='text-align: center;'>No logs found.</td></tr>";
            }
            ?>
    <form method="get" action="dashboard.php">
        <td colspan='5' style='text-align: center;'>
            <input id="note" name="add_note" size="50"/>
            <input class="button1" type="submit" id="submit" value="Post Note" />
        </td>
    </form>
</tbody>
</table>
    <?php } ?>

<?php require_once("footer.php"); ?>
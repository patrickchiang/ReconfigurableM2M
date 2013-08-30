<?php
/*
 * This page saves the heartbeat to file
 * Author: Patrick Chiang
 */

$obj = file_get_contents("php://input");
echo file_put_contents("heartbeat.txt", $obj);
?>
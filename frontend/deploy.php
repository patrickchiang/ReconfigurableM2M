<?php
/*
 * This page saves JSON information to file
 * Author: Patrick Chiang
 */

$obj = file_get_contents("php://input");
echo file_put_contents("json.txt", $obj);
?>